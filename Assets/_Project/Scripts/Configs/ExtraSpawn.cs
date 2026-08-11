using System;
using UnityEngine;

namespace MergeMechanic.Configs
{
    [Serializable]
    public class ExtraSpawn
    {
        [SerializeField, Range(0f, 1f)] private float _chance;
        [SerializeField] private SpawnTable _table = new SpawnTable();

        public float Chance => _chance;
        public SpawnTable Table => _table;
        public bool IsEmpty => _table == null || _table.IsEmpty || _chance <= 0f;
    }
}
