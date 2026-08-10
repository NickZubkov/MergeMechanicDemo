namespace MergeMechanic.Domain
{
    /// <summary>Единственное правило мерджа. Вынесено отдельно, потому что Задание 3 будет менять его.</summary>
    public static class MergeRule
    {
        public static bool CanMerge(BoardObject source, BoardObject target)
        {
            if (source == null || target == null || ReferenceEquals(source, target))
                return false;

            return source.Chain == target.Chain
                   && source.Level == target.Level
                   && !target.IsMaxLevel;
        }
    }
}
