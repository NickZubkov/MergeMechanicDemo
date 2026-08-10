using System.Collections.Generic;
using MergeMechanic.Configs;
using MergeMechanic.Domain;
using UnityEngine;

namespace MergeMechanic.Services
{
    public interface IGameBoard
    {
        int Width { get; }
        int Height { get; }
        IEnumerable<BoardObject> Objects { get; }
        bool HasFreeCell { get; }

        BoardObject GetAt(Vector2Int cell);

        InteractionResult TryInteract(Vector2Int from, Vector2Int to);

        BoardObject PlaceNew(MergeChainConfig chain, int level);
    }
}
