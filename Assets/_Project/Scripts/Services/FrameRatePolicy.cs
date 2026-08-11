using UnityEngine;
using Zenject;

namespace MergeMechanic.Services
{
    public class FrameRatePolicy : IInitializable
    {
        private const int FallbackFrameRate = 60;
        private const double MinimumRefreshRate = 24d;

        public void Initialize()
        {
            double refreshRate = Screen.currentResolution.refreshRateRatio.value;

            Application.targetFrameRate = refreshRate >= MinimumRefreshRate
                ? Mathf.RoundToInt((float)refreshRate)
                : FallbackFrameRate;
        }
    }
}
