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
    }
}
