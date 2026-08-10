using UnityEngine;

namespace MergeMechanic.Presentation
{
    public class BoardView : MonoBehaviour, IBoardLayout
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private SpriteRenderer _cellPrefab;
        [SerializeField] private Transform _cellsRoot;
        [SerializeField] private float _cellSize = 1f;
        [SerializeField] private float _padding = 0.5f;
        [SerializeField] private Color _cellColor = new Color(1f, 1f, 1f, 0.12f);

        private int _width;
        private int _height;
        private Vector3 _origin;

        public void Build(int width, int height)
        {
            _width = width;
            _height = height;
            _origin = new Vector3(
                -(width - 1) * _cellSize * 0.5f,
                -(height - 1) * _cellSize * 0.5f,
                0f);

            for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                SpriteRenderer cell = Instantiate(_cellPrefab, _cellsRoot);
                cell.sprite = DefaultSprite.Square;
                cell.transform.position = CellToWorld(new Vector2Int(x, y));
                cell.transform.localScale = Vector3.one * (_cellSize * 0.95f);
                cell.color = _cellColor;
            }

            FitCamera();
        }

        public Vector3 CellToWorld(Vector2Int cell)
            => _origin + new Vector3(cell.x * _cellSize, cell.y * _cellSize, 0f);

        public Vector3 ScreenToWorld(Vector3 screenPosition)
        {
            Vector3 world = _camera.ScreenToWorldPoint(screenPosition);
            world.z = 0f;
            return world;
        }

        public bool TryScreenToCell(Vector3 screenPosition, out Vector2Int cell)
        {
            Vector3 local = ScreenToWorld(screenPosition) - _origin;

            cell = new Vector2Int(
                Mathf.RoundToInt(local.x / _cellSize),
                Mathf.RoundToInt(local.y / _cellSize));

            return cell.x >= 0 && cell.x < _width && cell.y >= 0 && cell.y < _height;
        }

        private void FitCamera()
        {
            float halfHeight = _height * _cellSize * 0.5f + _padding;
            float halfWidth = _width * _cellSize * 0.5f + _padding;

            _camera.orthographicSize = Mathf.Max(halfHeight, halfWidth / _camera.aspect);
        }
    }
}
