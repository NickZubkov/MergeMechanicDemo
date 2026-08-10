using System;
using UnityEngine;

namespace MergeMechanic.Configs
{
    /// <summary>Один уровень цепочки. Уровень с непустой таблицей спавна и есть спавнер.</summary>
    [Serializable]
    public class ChainLevel
    {
        [SerializeField] private string _displayName = string.Empty;
        [SerializeField] private Color _color = Color.white;
        [SerializeField] private Sprite _sprite;
        [SerializeField] private SpawnTable _spawnTable = new SpawnTable();

        public string DisplayName => _displayName;
        public Color Color => _color;
        public Sprite Sprite => _sprite;
        public SpawnTable SpawnTable => _spawnTable;
    }
}
