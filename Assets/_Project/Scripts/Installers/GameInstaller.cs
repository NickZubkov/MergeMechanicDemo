using MergeMechanic.Configs;
using MergeMechanic.Domain;
using MergeMechanic.Presentation;
using MergeMechanic.Services;
using MergeMechanic.Signals;
using UnityEngine;
using Zenject;

namespace MergeMechanic.Installers
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private GameConfig _config;
        [SerializeField] private BoardView _boardView;
        [SerializeField] private BoardObjectView _objectViewPrefab;
        [SerializeField] private Transform _itemsRoot;

        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);

            Container.BindInstance(_config);

            Container.BindInterfacesTo<FrameRatePolicy>().AsSingle();

            Container.BindInterfacesTo<BoardView>().FromInstance(_boardView).AsSingle();
            Container.BindInterfacesTo<UnityRandomProvider>().AsSingle();
            Container.BindInterfacesTo<GameBoardService>().AsSingle();

            Container.BindInterfacesTo<SpawnerTimerService>().AsSingle().NonLazy();

            Container.BindInterfacesTo<BoardPresenter>().AsSingle();
            Container.BindInterfacesTo<DragInputController>().AsSingle();

            Container.BindFactory<BoardObjectView, BoardObjectView.Factory>()
                     .FromComponentInNewPrefab(_objectViewPrefab)
                     .UnderTransform(_itemsRoot);

            Container.DeclareSignal<BoardObjectSpawnedSignal>();
            Container.DeclareSignal<BoardObjectMovedSignal>();
            Container.DeclareSignal<BoardObjectsMergedSignal>();
        }
    }
}
