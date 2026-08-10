using System;
using MergeMechanic.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace MergeMechanic.Presentation
{
    public class SpawnerTimerButtonView : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _label;
        [SerializeField] private string _idleText = "Get Spawner";
        [SerializeField] private string _waitingText = "No space";

        private ISpawnerTimer _timer;
        private TimerState _shownState = (TimerState)(-1);
        private int _shownSeconds = -1;

        [Inject]
        public void Construct(ISpawnerTimer timer) => _timer = timer;

        private void Start() => _button.onClick.AddListener(OnClick);

        private void OnDestroy() => _button.onClick.RemoveListener(OnClick);

        private void OnClick() => _timer.StartCountdown();

        private void Update()
        {
            _button.interactable = _timer.CanStart;

            TimerState state = _timer.State;
            int seconds = state == TimerState.Counting ? Mathf.CeilToInt(_timer.Remaining) : -1;

            if (state == _shownState && seconds == _shownSeconds)
                return;

            _shownState = state;
            _shownSeconds = seconds;

            switch (state)
            {
                case TimerState.Counting:
                    _label.text = TimeSpan.FromSeconds(seconds).ToString(@"mm\:ss");
                    break;

                case TimerState.WaitingForSpace:
                    _label.text = _waitingText;
                    break;

                default:
                    _label.text = _idleText;
                    break;
            }
        }
    }
}
