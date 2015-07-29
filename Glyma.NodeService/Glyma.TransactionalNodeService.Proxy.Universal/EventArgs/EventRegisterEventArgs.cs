namespace TransactionalNodeService.Proxy.Universal.EventArgs
{
    public class EventRegisterEventArgs : System.EventArgs
    {
        public EventRegisterEventArgs()
            : base()
        {
        }

        public object Context
        {
            get;
            set;
        }

        public object State
        {
            get;
            set;
        }
    }
}
