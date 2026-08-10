namespace MergeMechanic.Services
{
    public interface ISpawnerTimer
    {
        TimerState State { get; }
        float Remaining { get; }
        bool CanStart { get; }
        void StartCountdown();
    }
}
