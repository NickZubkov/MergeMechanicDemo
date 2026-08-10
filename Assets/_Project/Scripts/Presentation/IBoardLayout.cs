using UnityEngine;

namespace MergeMechanic.Presentation
{
    public interface IBoardLayout
    {
        Vector3 CellToWorld(Vector2Int cell);
        Vector3 ScreenToWorld(Vector3 screenPosition);
        bool TryScreenToCell(Vector3 screenPosition, out Vector2Int cell);
        bool TryWorldToCell(Vector3 worldPosition, out Vector2Int cell);
    }
}
