namespace MergeMechanic.Domain
{
    public class UnityRandomProvider : IRandomProvider
    {
        public int Range(int minInclusive, int maxExclusive)
            => UnityEngine.Random.Range(minInclusive, maxExclusive);

        public float Value01() => UnityEngine.Random.value;
    }
}
