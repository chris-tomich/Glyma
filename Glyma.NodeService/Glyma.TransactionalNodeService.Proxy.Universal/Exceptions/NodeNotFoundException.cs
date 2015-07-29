using System;

namespace TransactionalNodeService.Proxy.Universal.Exceptions
{
    public class NodeNotFoundException : Exception
    {
        public NodeNotFoundException()
            : base()
        {
        }

        public NodeNotFoundException(string message)
            : base(message)
        {
        }
    }
}
