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

        /// <summary>Единая точка входа для любого ввода: тап это from == to.</summary>
        InteractionResult TryInteract(Vector2Int from, Vector2Int to);

        /// <summary>Ставит новый объект в случайную свободную клетку. Возвращает null, если места нет.</summary>
        BoardObject PlaceNew(MergeChainConfig chain, int level);
    }
}
