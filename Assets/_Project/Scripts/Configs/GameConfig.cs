using UnityEngine;

namespace MergeMechanic.Configs
{
    /// <summary>Корневой конфиг. Единственная ссылка на данные, которую держит инсталлер.</summary>
    [CreateAssetMenu(menuName = "MergeMechanic/Game", fileName = "GameConfig")]
    public class GameConfig : ScriptableObject
    {
        [SerializeField] private BoardConfig _board;
        [SerializeField] private SpawnerTimerConfig _timer;

        public BoardConfig Board => _board;
        public SpawnerTimerConfig Timer => _timer;
    }
}
