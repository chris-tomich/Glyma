using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Linq;

namespace TransactionalNodeService.Soap.TransactionFramework
{
    public class DependencyCollection
    {
        private int? _dependencyCount = null;
        private Dictionary<Proxy.IFacade, bool> _registeredFacades = null;

        public event EventHandler FacadesCompleted;

        public DependencyCollection()
        {
        }

        private Dictionary<Proxy.IFacade, bool> RegisteredFacades
        {
            get
            {
                if (_registeredFacades == null)
                {
                    _registeredFacades = new Dictionary<Proxy.IFacade, bool>();
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

            foreach (KeyValuePair<Proxy.IFacade, bool> facades in dependencies.RegisteredFacades)
            {
                if (!facades.Value)
                {
                    AddFacade(facades.Key);
                }
            }
        }

        public void AddFacade(Proxy.IFacade facade)
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

        private void OnFacadeCured(object sender, EventArgs e)
        {
            Proxy.IFacade facade = sender as Proxy.IFacade;

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
                            FacadesCompleted(this, new EventArgs());
                        }
                    }
                }
            }
        }
    }
}
