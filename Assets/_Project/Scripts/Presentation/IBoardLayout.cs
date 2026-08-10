using UnityEngine;

namespace MergeMechanic.Presentation
{
    /// <summary>Раскладка поля в мире. Прячет камеру от остальных слоёв.</summary>
    public interface IBoardLayout
    {
        void Build(int width, int height);
        Vector3 CellToWorld(Vector2Int cell);
        Vector3 ScreenToWorld(Vector3 screenPosition);
        bool TryScreenToCell(Vector3 screenPosition, out Vector2Int cell);
    }
}
