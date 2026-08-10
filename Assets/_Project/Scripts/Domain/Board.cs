using System.Collections.Generic;
using UnityEngine;

namespace MergeMechanic.Domain
{
    /// <summary>Сетка объектов. Только хранение — правил здесь нет.</summary>
    public class Board
    {
        private readonly BoardObject[,] _cells;
        private readonly List<Vector2Int> _freeCellsBuffer = new List<Vector2Int>();

        public int Width { get; }
        public int Height { get; }

        public Board(int width, int height)
        {
            Width = width;
            Height = height;
            _cells = new BoardObject[width, height];
        }

        public bool IsInside(Vector2Int cell)
            => cell.x >= 0 && cell.x < Width && cell.y >= 0 && cell.y < Height;

        public BoardObject Get(Vector2Int cell) => IsInside(cell) ? _cells[cell.x, cell.y] : null;

        public bool IsFree(Vector2Int cell) => IsInside(cell) && _cells[cell.x, cell.y] == null;

        public void Place(BoardObject boardObject, Vector2Int cell)
        {
            _cells[cell.x, cell.y] = boardObject;
            boardObject.SetPosition(cell);
        }

        public void Remove(Vector2Int cell) => _cells[cell.x, cell.y] = null;

        public void Move(Vector2Int from, Vector2Int to)
        {
            BoardObject moved = _cells[from.x, from.y];
            _cells[from.x, from.y] = null;
            Place(moved, to);
        }

        public IEnumerable<BoardObject> Objects
        {
            get
            {
                for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                {
                    BoardObject current = _cells[x, y];
                    if (current != null)
                        yield return current;
                }
            }
        }

        public bool HasFreeCell
        {
            get
            {
                for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    if (_cells[x, y] == null)
                        return true;

                return false;
            }
        }

        public bool TryGetRandomFreeCell(IRandomProvider random, out Vector2Int cell)
        {
            _freeCellsBuffer.Clear();

            for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                if (_cells[x, y] == null)
                    _freeCellsBuffer.Add(new Vector2Int(x, y));

            if (_freeCellsBuffer.Count == 0)
            {
                cell = default;
                return false;
            }

            cell = _freeCellsBuffer[random.Range(0, _freeCellsBuffer.Count)];
            return true;
        }
    }
}
