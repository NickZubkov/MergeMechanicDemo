using UnityEngine;

namespace MergeMechanic.Presentation
{
    public class BoardView : MonoBehaviour, IBoardLayout, IBoardBuilder
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private SpriteRenderer _cellPrefab;
        [SerializeField] private Transform _cellsRoot;
        [SerializeField] private RectTransform _bottomUiArea;
        [SerializeField] private float _cellSize = 1f;
        [SerializeField] private float _padding = 0.5f;
        [SerializeField] private Color _cellColor = new Color(1f, 1f, 1f, 0.12f);

        private readonly Vector3[] _uiCorners = new Vector3[4];

        private int _width;
        private int _height;
        private Vector3 _origin;
        private int _fittedScreenWidth;
        private int _fittedScreenHeight;

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
                if (cell.sprite == null)
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
            => TryWorldToCell(ScreenToWorld(screenPosition), out cell);

        public bool TryWorldToCell(Vector3 worldPosition, out Vector2Int cell)
        {
            Vector3 local = worldPosition - _origin;

            cell = new Vector2Int(
                Mathf.RoundToInt(local.x / _cellSize),
                Mathf.RoundToInt(local.y / _cellSize));

            return cell.x >= 0 && cell.x < _width && cell.y >= 0 && cell.y < _height;
        }

        private void Update()
        {
            if (_width == 0 || _height == 0)
                return;

            if (Screen.width == _fittedScreenWidth && Screen.height == _fittedScreenHeight)
                return;

            FitCamera();
        }

        private void FitCamera()
        {
            _fittedScreenWidth = Screen.width;
            _fittedScreenHeight = Screen.height;

            float reserved = ReservedBottomFraction();
            float halfHeight = (_height * _cellSize * 0.5f + _padding) / (1f - reserved);
            float halfWidth = (_width * _cellSize * 0.5f + _padding) / _camera.aspect;

            _camera.orthographicSize = Mathf.Max(halfHeight, halfWidth);

            Vector3 position = _camera.transform.position;
            position.y = -_camera.orthographicSize * reserved;
            _camera.transform.position = position;
        }

        private float ReservedBottomFraction()
        {
            if (_bottomUiArea == null || Screen.height <= 0)
                return 0f;

            Canvas.ForceUpdateCanvases();
            _bottomUiArea.GetWorldCorners(_uiCorners);

            return Mathf.Clamp(_uiCorners[1].y / Screen.height, 0f, 0.5f);
        }
    }
}
