using System.Collections.Generic;
using MergeMechanic.Configs;
using MergeMechanic.Domain;
using MergeMechanic.Signals;
using UnityEngine;
using Zenject;

namespace MergeMechanic.Services
{
    /// <summary>Единственное место, где меняется состояние поля.</summary>
    public class GameBoardService : IGameBoard
    {
        private readonly IRandomProvider _random;
        private readonly SignalBus _signalBus;
        private readonly Board _board;

        private int _nextId = 1;

        public GameBoardService(GameConfig config, IRandomProvider random, SignalBus signalBus)
        {
            _random = random;
            _signalBus = signalBus;

            BoardConfig boardConfig = config.Board;
            _board = new Board(boardConfig.Width, boardConfig.Height);

            ApplyStartingLayout(boardConfig);
        }

        public int Width => _board.Width;
        public int Height => _board.Height;
        public IEnumerable<BoardObject> Objects => _board.Objects;
        public bool HasFreeCell => _board.HasFreeCell;

        public BoardObject GetAt(Vector2Int cell) => _board.Get(cell);

        public InteractionResult TryInteract(Vector2Int from, Vector2Int to)
        {
            BoardObject source = _board.Get(from);
            if (source == null)
                return InteractionResult.Rejected;

            if (from == to)
                return TrySpawnFrom(source);

            if (!_board.IsInside(to))
                return InteractionResult.Rejected;

            BoardObject target = _board.Get(to);

            if (target == null)
            {
                _board.Move(from, to);
                _signalBus.Fire(new BoardObjectMovedSignal(source, from, to));
                return InteractionResult.Moved;
            }

            if (!MergeRule.CanMerge(source, target))
                return InteractionResult.Rejected;

            int consumedId = source.Id;
            _board.Remove(from);
            target.Upgrade();
            _signalBus.Fire(new BoardObjectsMergedSignal(consumedId, target));
            return InteractionResult.Merged;
        }

        public BoardObject PlaceNew(MergeChainConfig chain, int level)
        {
            if (chain == null || !chain.HasLevel(level))
            {
                Debug.LogError($"PlaceNew: некорректная цепочка или уровень {level}.");
                return null;
            }

            if (!_board.TryGetRandomFreeCell(_random, out Vector2Int cell))
                return null;

            return Create(chain, level, cell);
        }

        private InteractionResult TrySpawnFrom(BoardObject spawner)
        {
            if (!spawner.CanSpawn)
                return InteractionResult.Rejected;

            if (!_board.TryGetRandomFreeCell(_random, out Vector2Int cell))
                return InteractionResult.Rejected;

            SpawnEntry entry = WeightedPicker.Pick(spawner.LevelData.SpawnTable.Entries, _random);

            if (entry == null || entry.Chain == null || !entry.Chain.HasLevel(entry.Level))
            {
                Debug.LogError(
                    $"Таблица спавна объекта {spawner.Id} содержит некорректную запись.");
                return InteractionResult.Rejected;
            }

            Create(entry.Chain, entry.Level, cell);
            return InteractionResult.Spawned;
        }

        private BoardObject Create(MergeChainConfig chain, int level, Vector2Int cell)
        {
            var created = new BoardObject(_nextId++, chain, level);
            _board.Place(created, cell);
            _signalBus.Fire(new BoardObjectSpawnedSignal(created));
            return created;
        }

        private void ApplyStartingLayout(BoardConfig boardConfig)
        {
            foreach (BoardConfig.StartingObject entry in boardConfig.StartingLayout)
            {
                if (entry.Chain == null || !entry.Chain.HasLevel(entry.Level))
                {
                    Debug.LogError($"Стартовая раскладка: некорректная запись в {entry.Position}.");
                    continue;
                }

                if (!_board.IsFree(entry.Position))
                {
                    Debug.LogError($"Стартовая раскладка: клетка {entry.Position} вне поля или занята.");
                    continue;
                }

                _board.Place(new BoardObject(_nextId++, entry.Chain, entry.Level), entry.Position);
            }
        }
    }
}
