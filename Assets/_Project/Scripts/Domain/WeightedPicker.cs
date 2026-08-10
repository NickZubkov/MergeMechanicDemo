using System.Collections.Generic;
using MergeMechanic.Configs;
using UnityEngine;

namespace MergeMechanic.Domain
{
    /// <summary>Выбор записи таблицы спавна пропорционально весам.</summary>
    public static class WeightedPicker
    {
        public static SpawnEntry Pick(IReadOnlyList<SpawnEntry> entries, IRandomProvider random)
        {
            if (entries == null || entries.Count == 0)
                return null;

            float total = 0f;
            for (int i = 0; i < entries.Count; i++)
                total += Mathf.Max(0f, entries[i].Weight);

            if (total <= 0f)
                return entries[random.Range(0, entries.Count)];

            float roll = random.Value01() * total;
            float accumulated = 0f;

            for (int i = 0; i < entries.Count; i++)
            {
                accumulated += Mathf.Max(0f, entries[i].Weight);
                if (roll < accumulated)
                    return entries[i];
            }

            return entries[entries.Count - 1];
        }
    }
}
