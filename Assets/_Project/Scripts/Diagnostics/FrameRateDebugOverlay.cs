using UnityEngine;

namespace MergeMechanic.Diagnostics
{
    public class FrameRateDebugOverlay : MonoBehaviour
    {
        private static readonly int[] Targets = { -1, 30, 60, 120 };

        private float _smoothedDelta;
        private float _worstDelta;
        private int _targetIndex;
        private GUIStyle _labelStyle;
        private GUIStyle _buttonStyle;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            var host = new GameObject("FrameRateDebugOverlay");
            host.AddComponent<FrameRateDebugOverlay>();
            DontDestroyOnLoad(host);
        }

        private void Awake()
        {
            _smoothedDelta = Time.unscaledDeltaTime;
            _worstDelta = 0f;
        }

        private void Update()
        {
            _smoothedDelta = Mathf.Lerp(_smoothedDelta, Time.unscaledDeltaTime, 0.1f);

            if (Time.unscaledDeltaTime > _worstDelta)
                _worstDelta = Time.unscaledDeltaTime;
        }

        private void OnGUI()
        {
            float unit = Screen.height * 0.035f;

            if (_labelStyle == null)
            {
                _labelStyle = new GUIStyle(GUI.skin.label);
                _labelStyle.fontSize = Mathf.RoundToInt(unit * 0.8f);
                _labelStyle.normal.textColor = Color.white;

                _buttonStyle = new GUIStyle(GUI.skin.button);
                _buttonStyle.fontSize = Mathf.RoundToInt(unit * 0.8f);
            }

            float fps = _smoothedDelta > 0f ? 1f / _smoothedDelta : 0f;
            float width = Screen.width * 0.62f;

            GUI.Box(new Rect(0f, 0f, width, unit * 5.6f), GUIContent.none);

            GUI.Label(
                new Rect(unit * 0.3f, unit * 0.2f, width, unit),
                $"FPS {fps:F1}   dt {_smoothedDelta * 1000f:F1} ms",
                _labelStyle);

            GUI.Label(
                new Rect(unit * 0.3f, unit * 1.2f, width, unit),
                $"target {Application.targetFrameRate}   display {Screen.currentResolution.refreshRateRatio.value:F0} Hz",
                _labelStyle);

            GUI.Label(
                new Rect(unit * 0.3f, unit * 2.2f, width, unit),
                $"worst dt {_worstDelta * 1000f:F1} ms   touches {Input.touchCount}",
                _labelStyle);

            int next = Targets[(_targetIndex + 1) % Targets.Length];

            if (GUI.Button(new Rect(unit * 0.3f, unit * 3.3f, width * 0.9f, unit * 2f), $"target -> {next}", _buttonStyle))
            {
                _targetIndex = (_targetIndex + 1) % Targets.Length;
                Application.targetFrameRate = Targets[_targetIndex];
                _worstDelta = 0f;
            }
        }
    }
}
