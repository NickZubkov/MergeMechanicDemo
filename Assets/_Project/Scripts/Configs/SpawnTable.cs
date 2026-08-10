using System;
using System.Collections.Generic;
using UnityEngine;

namespace MergeMechanic.Configs
{
    [Serializable]
    public class SpawnTable
    {
        [SerializeField] private List<SpawnEntry> _entries = new List<SpawnEntry>();

        public IReadOnlyList<SpawnEntry> Entries => _entries;
        public bool IsEmpty => _entries == null || _entries.Count == 0;
    }
}
