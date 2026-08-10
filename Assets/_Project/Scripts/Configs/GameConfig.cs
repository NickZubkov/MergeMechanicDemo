using UnityEngine;

namespace MergeMechanic.Configs
{
    [CreateAssetMenu(menuName = "MergeMechanic/Game", fileName = "GameConfig")]
    public class GameConfig : ScriptableObject
    {
        [SerializeField] private BoardConfig _board;
        [SerializeField] private SpawnerTimerConfig _timer;

        public BoardConfig Board => _board;
        public SpawnerTimerConfig Timer => _timer;

        private void OnValidate()
        {
            if (_board == null)
                Debug.LogError($"[{name}] не задан BoardConfig.", this);

            if (_timer == null)
                Debug.LogError($"[{name}] не задан SpawnerTimerConfig.", this);
        }
    }
}
