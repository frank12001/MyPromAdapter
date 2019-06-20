namespace MyPromAdapter.Controllers.MyAdapter
{
    public abstract class Strategy
    {
        public abstract HttpAction[] GetActions(AlertmanagerMsg alertMsg);
    }
}