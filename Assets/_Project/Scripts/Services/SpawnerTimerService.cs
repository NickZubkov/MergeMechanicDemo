using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MergeMechanic.Configs;
using UnityEngine;

namespace MergeMechanic.Services
{
    public class SpawnerTimerService : ISpawnerTimer, IDisposable
    {
        private readonly SpawnerTimerConfig _config;
        private readonly IGameBoard _board;
        private readonly CancellationTokenSource _lifetime = new CancellationTokenSource();

        public TimerState State { get; private set; } = TimerState.Idle;
        public float Remaining { get; private set; }
        public bool CanStart => State == TimerState.Idle;

        public SpawnerTimerService(GameConfig config, IGameBoard board)
        {
            _config = config.Timer;
            _board = board;
        }

        public void StartCountdown()
        {
            if (!CanStart)
                return;

            RunAsync(_lifetime.Token).Forget();
        }

        private async UniTaskVoid RunAsync(CancellationToken cancellationToken)
        {
            State = TimerState.Counting;
            Remaining = _config.DurationSeconds;

            while (Remaining > 0f)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                Remaining -= Time.deltaTime;
            }

            Remaining = 0f;
            State = TimerState.WaitingForSpace;

            await UniTask.WaitUntil(() => _board.HasFreeCell, cancellationToken: cancellationToken);

            _board.PlaceNew(_config.ChainToSpawn, _config.LevelToSpawn);
            State = TimerState.Idle;
        }

        public void Dispose()
        {
            _lifetime.Cancel();
            _lifetime.Dispose();
        }
    }
}
