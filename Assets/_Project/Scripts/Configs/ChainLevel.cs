using System;
using UnityEngine;

namespace MergeMechanic.Configs
{
    [Serializable]
    public class ChainLevel
    {
        [SerializeField] private string _displayName = string.Empty;
        [SerializeField] private Color _color = Color.white;
        [SerializeField] private Sprite _sprite;
        [SerializeField] private SpawnTable _spawnTable = new SpawnTable();
        [SerializeField] private ExtraSpawn _extraSpawn = new ExtraSpawn();

        public string DisplayName => _displayName;
        public Color Color => _color;
        public Sprite Sprite => _sprite;
        public SpawnTable SpawnTable => _spawnTable;
        public ExtraSpawn ExtraSpawn => _extraSpawn;
    }
}
