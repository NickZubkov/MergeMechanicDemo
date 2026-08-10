namespace MergeMechanic.Domain
{
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
