using System;
using UnityEngine;

namespace MergeMechanic.Configs
{
    [Serializable]
    public class SpawnEntry
    {
        [SerializeField] private MergeChainConfig _chain;
        [SerializeField, Min(1)] private int _level = 1;
        [SerializeField, Min(0f)] private float _weight = 1f;

        public MergeChainConfig Chain => _chain;
        public int Level => _level;
        public float Weight => _weight;
    }
}
