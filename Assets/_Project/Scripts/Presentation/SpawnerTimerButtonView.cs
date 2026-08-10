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

        [Inject]
        public void Construct(ISpawnerTimer timer) => _timer = timer;

        private void Start() => _button.onClick.AddListener(OnClick);

        private void OnDestroy() => _button.onClick.RemoveListener(OnClick);

        private void OnClick() => _timer.StartCountdown();

        private void Update()
        {
            _button.interactable = _timer.CanStart;

            switch (_timer.State)
            {
                case TimerState.Counting:
                    _label.text = TimeSpan
                        .FromSeconds(Mathf.CeilToInt(_timer.Remaining))
                        .ToString(@"mm\:ss");
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
