namespace MergeMechanic.Domain
{
    /// <summary>Источник случайности за интерфейсом, чтобы весь вероятностный выбор был в одном месте.</summary>
    public interface IRandomProvider
    {
        int Range(int minInclusive, int maxExclusive);
        float Value01();
    }
}
