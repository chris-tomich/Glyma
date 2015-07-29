using System;

namespace TransactionalNodeService.Proxy.Universal
{
    public interface IFacade
    {
        bool IsConcrete { get; }

        event EventHandler BaseCured;

        void ResetToFacade();
    }
}
