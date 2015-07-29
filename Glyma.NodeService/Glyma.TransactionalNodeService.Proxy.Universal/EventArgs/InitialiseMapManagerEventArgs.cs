namespace TransactionalNodeService.Proxy.Universal.EventArgs
{
    public class InitialiseMapManagerEventArgs : EventRegisterEventArgs
    {
        public InitialiseMapManagerEventArgs()
            : base()
        {
            IsInitialised = false;
        }

        public InitialiseMapManagerEventArgs(bool isInitialised)
            : this()
        {
            IsInitialised = isInitialised;
        }

        public bool IsInitialised
        {
            get;
            set;
        }

        public string ErrorMessage
        {
            get;
            set;
        }
    }
}
