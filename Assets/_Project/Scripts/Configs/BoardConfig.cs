using System;
using System.Collections.Generic;
using UnityEngine;

namespace MergeMechanic.Configs
{
    [CreateAssetMenu(menuName = "MergeMechanic/Board", fileName = "BoardConfig")]
    public class BoardConfig : ScriptableObject
    {
        [SerializeField, Min(1)] private int _width = 7;
        [SerializeField, Min(1)] private int _height = 9;
        [SerializeField] private List<StartingObject> _startingLayout = new List<StartingObject>();

        public int Width => _width;
        public int Height => _height;
        public IReadOnlyList<StartingObject> StartingLayout => _startingLayout;

        private void OnValidate()
        {
            var occupied = new HashSet<Vector2Int>();

            foreach (StartingObject entry in _startingLayout)
            {
                Vector2Int p = entry.Position;

                if (p.x < 0 || p.x >= _width || p.y < 0 || p.y >= _height)
                    Debug.LogError($"[{name}] стартовая позиция {p} вне поля {_width}x{_height}.", this);
                else if (!occupied.Add(p))
                    Debug.LogError($"[{name}] в клетке {p} задано больше одного стартового объекта.", this);

                if (entry.Chain == null)
                    Debug.LogError($"[{name}] стартовый объект в {p}: не задана цепочка.", this);
                else if (!entry.Chain.HasLevel(entry.Level))
                    Debug.LogError(
                        $"[{name}] стартовый объект в {p}: цепочка '{entry.Chain.name}' не имеет уровня {entry.Level}.",
                        this);
            }
        }

        [Serializable]
        public class StartingObject
        {
            [SerializeField] private MergeChainConfig _chain;
            [SerializeField, Min(1)] private int _level = 1;
            [SerializeField] private Vector2Int _position;

            public MergeChainConfig Chain => _chain;
            public int Level => _level;
            public Vector2Int Position => _position;
        }
    }
}
