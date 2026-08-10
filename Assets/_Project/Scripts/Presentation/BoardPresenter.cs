using System.Collections.Generic;
using MergeMechanic.Domain;
using MergeMechanic.Services;
using MergeMechanic.Signals;
using UnityEngine;
using Zenject;

namespace MergeMechanic.Presentation
{
    /// <summary>Держит сцену в соответствии с моделью. Сам состояние не меняет.</summary>
    public class BoardPresenter : IInitializable, System.IDisposable, IBoardObjectViews
    {
        private readonly IGameBoard _board;
        private readonly IBoardLayout _layout;
        private readonly BoardObjectView.Factory _factory;
        private readonly SignalBus _signalBus;
        private readonly Dictionary<int, BoardObjectView> _views = new Dictionary<int, BoardObjectView>();

        public BoardPresenter(
            IGameBoard board,
            IBoardLayout layout,
            BoardObjectView.Factory factory,
            SignalBus signalBus)
        {
            _board = board;
            _layout = layout;
            _factory = factory;
            _signalBus = signalBus;
        }

        public void Initialize()
        {
            _layout.Build(_board.Width, _board.Height);

            _signalBus.Subscribe<BoardObjectSpawnedSignal>(OnSpawned);
            _signalBus.Subscribe<BoardObjectMovedSignal>(OnMoved);
            _signalBus.Subscribe<BoardObjectsMergedSignal>(OnMerged);

            foreach (BoardObject boardObject in _board.Objects)
                CreateView(boardObject);
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<BoardObjectSpawnedSignal>(OnSpawned);
            _signalBus.Unsubscribe<BoardObjectMovedSignal>(OnMoved);
            _signalBus.Unsubscribe<BoardObjectsMergedSignal>(OnMerged);
        }

        public bool TryGetView(int id, out BoardObjectView view) => _views.TryGetValue(id, out view);

        private void CreateView(BoardObject boardObject)
        {
            BoardObjectView view = _factory.Create();
            view.Bind(boardObject);
            view.transform.position = _layout.CellToWorld(boardObject.Position);
            _views[boardObject.Id] = view;
        }

        private void OnSpawned(BoardObjectSpawnedSignal signal) => CreateView(signal.Object);

        private void OnMoved(BoardObjectMovedSignal signal)
        {
            if (_views.TryGetValue(signal.Object.Id, out BoardObjectView view))
                view.transform.position = _layout.CellToWorld(signal.To);
        }

        private void OnMerged(BoardObjectsMergedSignal signal)
        {
            if (_views.TryGetValue(signal.ConsumedId, out BoardObjectView consumed))
            {
                _views.Remove(signal.ConsumedId);
                UnityEngine.Object.Destroy(consumed.gameObject);
            }

            if (_views.TryGetValue(signal.Result.Id, out BoardObjectView result))
            {
                result.Bind(signal.Result);
                result.transform.position = _layout.CellToWorld(signal.Result.Position);
            }
        }
    }
}
