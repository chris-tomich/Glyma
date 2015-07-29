namespace TransactionalNodeService.Proxy.Universal.EventArgs
{
    public class MapManagerActivityEventArgs : System.EventArgs
    {
        public MapManagerActivityEventArgs()
            : base()
        {
        }

        public int TransactionsLeft
        {
            get;
            set;
        }

        public ActivityStatusEnum Status
        {
            get;
            set;
        }
    }
}
