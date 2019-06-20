namespace MyPromAdapter.Controllers.MyAdapter
{
    public class HttpActionFactory
    {
        private Strategy Strategy;
        private AlertmanagerMsg alertMsg;
        public HttpActionFactory(AlertmanagerMsg msg)
        {
            alertMsg = msg;
            Strategy = new StrategyScaleGaming();
        }

        public HttpAction[] GetActions()
        {
            return Strategy.GetActions(alertMsg);
        }
    }
}