using MergeMechanic.Domain;
using MergeMechanic.Services;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace MergeMechanic.Presentation
{
    public class DragInputController : ITickable
    {
        private readonly IGameBoard _board;
        private readonly IBoardLayout _layout;
        private readonly IBoardObjectViews _views;

        private BoardObjectView _dragged;
        private Vector2Int _fromCell;
        private Vector3 _grabOffset;

        public DragInputController(IGameBoard board, IBoardLayout layout, IBoardObjectViews views)
        {
            _board = board;
            _layout = layout;
            _views = views;
        }

        public void Tick()
        {
            if (Input.GetMouseButtonDown(0))
                TryBeginDrag();
            else if (_dragged != null && Input.GetMouseButtonUp(0))
                EndDrag();
            else if (_dragged != null && Input.GetMouseButton(0))
                _dragged.transform.position = _layout.ScreenToWorld(Input.mousePosition) + _grabOffset;
        }

        private void TryBeginDrag()
        {
            if (IsPointerOverUI())
                return;

            if (!_layout.TryScreenToCell(Input.mousePosition, out Vector2Int cell))
                return;

            BoardObject boardObject = _board.GetAt(cell);
            if (boardObject == null)
                return;

            if (!_views.TryGetView(boardObject.Id, out BoardObjectView view))
                return;

            _dragged = view;
            _fromCell = cell;
            _grabOffset = view.transform.position - _layout.ScreenToWorld(Input.mousePosition);
            view.SetDragging(true);
        }

        private void EndDrag()
        {
            BoardObjectView view = _dragged;
            Vector2Int from = _fromCell;
            _dragged = null;

            view.SetDragging(false);

            InteractionResult result = InteractionResult.Rejected;
            Vector2Int settled = from;

            if (_layout.TryScreenToCell(Input.mousePosition, out Vector2Int toCell))
            {
                result = _board.TryInteract(from, toCell);
                if (result == InteractionResult.Moved)
                    settled = toCell;
            }

            if (result != InteractionResult.Merged)
                view.transform.position = _layout.CellToWorld(settled);
        }

        private static bool IsPointerOverUI()
            => EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
}
