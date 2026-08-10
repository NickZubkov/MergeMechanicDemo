using System;
using MergeMechanic.Configs;
using UnityEngine;

namespace MergeMechanic.Domain
{
    public class BoardObject
    {
        public int Id { get; }
        public MergeChainConfig Chain { get; }
        public int Level { get; private set; }
        public Vector2Int Position { get; private set; }
        public ChainLevel LevelData => Chain.GetLevel(Level);
        public bool CanSpawn => !LevelData.SpawnTable.IsEmpty;
        public bool IsMaxLevel => Level >= Chain.MaxLevel;

        public BoardObject(int id, MergeChainConfig chain, int level)
        {
            if (chain == null)
                throw new ArgumentNullException(nameof(chain));
            if (!chain.HasLevel(level))
                throw new ArgumentOutOfRangeException(
                    nameof(level), $"Цепочка '{chain.name}' не имеет уровня {level}.");

            Id = id;
            Chain = chain;
            Level = level;
        }

        public void Upgrade()
        {
            if (IsMaxLevel)
                throw new InvalidOperationException($"Объект {Id} уже на максимальном уровне цепочки.");

            Level++;
        }

        public void SetPosition(Vector2Int position) => Position = position;
    }
}
