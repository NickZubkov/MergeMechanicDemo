namespace MergeMechanic.Presentation
{
    public interface IBoardObjectViews
    {
        bool TryGetView(int id, out BoardObjectView view);
    }
}
