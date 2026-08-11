using System.Collections.Generic;
using UnityEngine;

namespace MergeMechanic.Configs
{
    [CreateAssetMenu(menuName = "MergeMechanic/Merge Chain", fileName = "MergeChain")]
    public class MergeChainConfig : ScriptableObject
    {
        [SerializeField] private List<ChainLevel> _levels = new List<ChainLevel>();

        public int MaxLevel => _levels.Count;

        public bool HasLevel(int level) => level >= 1 && level <= _levels.Count;

        public ChainLevel GetLevel(int level) => _levels[level - 1];

        private void OnValidate()
        {
            for (int i = 0; i < _levels.Count; i++)
            {
                if (_levels[i] == null)
                    continue;

                ValidateTable(_levels[i].SpawnTable, i + 1);
                ValidateTable(_levels[i].ExtraSpawn?.Table, i + 1);
            }
        }

        private void ValidateTable(SpawnTable table, int level)
        {
            if (table == null || table.IsEmpty)
                return;

            foreach (SpawnEntry entry in table.Entries)
            {
                if (entry.Chain == null)
                {
                    Debug.LogError($"[{name}] уровень {level}: в таблице спавна не задана цепочка.", this);
                    continue;
                }

                if (!entry.Chain.HasLevel(entry.Level))
                    Debug.LogError(
                        $"[{name}] уровень {level}: цепочка '{entry.Chain.name}' не имеет уровня {entry.Level}.",
                        this);
            }
        }
    }
}
