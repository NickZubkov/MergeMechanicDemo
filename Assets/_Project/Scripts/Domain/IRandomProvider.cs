namespace MergeMechanic.Domain
{
    public interface IRandomProvider
    {
        int Range(int minInclusive, int maxExclusive);
        float Value01();
    }
}
