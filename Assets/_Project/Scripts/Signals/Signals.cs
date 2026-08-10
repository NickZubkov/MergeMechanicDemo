using MergeMechanic.Domain;
using UnityEngine;

namespace MergeMechanic.Signals
{
    public readonly struct BoardObjectSpawnedSignal
    {
        public readonly BoardObject Object;

        public BoardObjectSpawnedSignal(BoardObject boardObject) => Object = boardObject;
    }

    public readonly struct BoardObjectMovedSignal
    {
        public readonly BoardObject Object;
        public readonly Vector2Int From;
        public readonly Vector2Int To;

        public BoardObjectMovedSignal(BoardObject boardObject, Vector2Int from, Vector2Int to)
        {
            Object = boardObject;
            From = from;
            To = to;
        }
    }

    public readonly struct BoardObjectsMergedSignal
    {
        public readonly int ConsumedId;
        public readonly BoardObject Result;

        public BoardObjectsMergedSignal(int consumedId, BoardObject result)
        {
            ConsumedId = consumedId;
            Result = result;
        }
    }
}
