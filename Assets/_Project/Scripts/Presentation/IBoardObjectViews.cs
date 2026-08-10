namespace MergeMechanic.Presentation
{
    /// <summary>Реестр вьюх для input-слоя: нужен, чтобы поднять конкретную вьюху при захвате.</summary>
    public interface IBoardObjectViews
    {
        bool TryGetView(int id, out BoardObjectView view);
    }
}
