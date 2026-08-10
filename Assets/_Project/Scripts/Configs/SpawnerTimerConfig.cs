using UnityEngine;

namespace MergeMechanic.Configs
{
    /// <summary>Настройки кнопки-таймера, выдающей спавнер.</summary>
    [CreateAssetMenu(menuName = "MergeMechanic/Spawner Timer", fileName = "SpawnerTimerConfig")]
    public class SpawnerTimerConfig : ScriptableObject
    {
        [SerializeField, Min(0.1f)] private float _durationSeconds = 30f;
        [SerializeField] private MergeChainConfig _chainToSpawn;
        [SerializeField, Min(1)] private int _levelToSpawn = 1;

        public float DurationSeconds => _durationSeconds;
        public MergeChainConfig ChainToSpawn => _chainToSpawn;
        public int LevelToSpawn => _levelToSpawn;

        private void OnValidate()
        {
            if (_chainToSpawn == null)
                Debug.LogError($"[{name}] не задана цепочка, которую выдаёт таймер.", this);
            else if (!_chainToSpawn.HasLevel(_levelToSpawn))
                Debug.LogError(
                    $"[{name}] цепочка '{_chainToSpawn.name}' не имеет уровня {_levelToSpawn}.", this);
        }
    }
}
