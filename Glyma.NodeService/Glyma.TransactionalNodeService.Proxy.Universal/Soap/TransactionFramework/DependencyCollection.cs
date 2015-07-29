using System;
using System.Collections.Generic;

namespace TransactionalNodeService.Proxy.Universal.Soap.TransactionFramework
{
    public class DependencyCollection
    {
        private int? _dependencyCount = null;
        private Dictionary<IFacade, bool> _registeredFacades = null;

        public event EventHandler FacadesCompleted;

        public DependencyCollection()
        {
        }

        private Dictionary<IFacade, bool> RegisteredFacades
        {
            get
            {
                if (_registeredFacades == null)
                {
                    _registeredFacades = new Dictionary<IFacade, bool>();
                }

                return _registeredFacades;
            }
            set
            {
                _registeredFacades = value;
            }
        }

        public object State
        {
            get;
            set;
        }

        public void UnionWith(DependencyCollection dependencies)
        {
            if (dependencies == null)
            {
                return;
            }

            foreach (KeyValuePair<IFacade, bool> facades in dependencies.RegisteredFacades)
            {
                if (!facades.Value)
                {
                    AddFacade(facades.Key);
                }
            }
        }

        public void AddFacade(IFacade facade)
        {
            if (!facade.IsConcrete && !RegisteredFacades.ContainsKey(facade))
            {
                RegisteredFacades[facade] = false;

                if (_dependencyCount == null)
                {
                    _dependencyCount = 1;
                }
                else
                {
                    _dependencyCount++;
                }

                facade.BaseCured += OnFacadeCured;
            }
        }

        public bool IsCompleted
        {
            get
            {
                return (_dependencyCount == 0 || _dependencyCount == null);
            }
        }

        private void OnFacadeCured(object sender, System.EventArgs e)
        {
            IFacade facade = sender as IFacade;

            if (facade != null)
            {
                if (RegisteredFacades.ContainsKey(facade))
                {
                    RegisteredFacades[facade] = true;
                    _dependencyCount--;

                    if (_dependencyCount == 0)
                    {
                        if (FacadesCompleted != null)
                        {
                            FacadesCompleted(this, new System.EventArgs());
                        }
                    }
                }
            }
        }
    }
}
