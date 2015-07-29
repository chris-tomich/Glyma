using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SilverlightMappingToolBasic.MapDepth
{
    public sealed class Conductor
    {
        private static volatile Conductor instance;
        private static object syncRoot = new Object();
        IList<Guid> _activeNodes;
        IList<Guid> _searchedNodes;

        private Conductor() 
        {
            _activeNodes = new List<Guid>();
            _searchedNodes = new List<Guid>();
        }

        public static Conductor Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new Conductor();
                    }
                }

                return instance;
            }
        }

        public bool HasCompleted
        {
            get 
            {
                lock (_activeNodes)
                {
                    if (_activeNodes.Count == 0)
                    {
                        //_searchedNodes.Clear();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        public void MakeMeActive(Guid nodeProxyId)
        {
            lock (_activeNodes)
            {
                _activeNodes.Add(nodeProxyId);
                //if (_searchedNodes.Contains(nodeProxyId))
                //{
                //    System.Diagnostics.Debug.WriteLine("Duplicated search detected");
                //}
                //else
                //{
                //    System.Diagnostics.Debug.WriteLine("Unique: {0}", nodeProxyId);
                //}
                _searchedNodes.Add(nodeProxyId);
            }
        }

        public void MakeMeInActive(Guid nodeProxyId) 
        {
            lock (_activeNodes)
            {
                _activeNodes.Remove(nodeProxyId);
            }
        }

        /// <summary>
        /// Check to see if it's already active reducing the number of calls
        /// </summary>
        /// <param name="nodeProxyId"></param>
        /// <returns></returns>
        public bool IsActive(Guid nodeProxyId)
        {
            lock (_activeNodes)
            {
                return _activeNodes.Contains(nodeProxyId);
            }
        }

        public bool HasSearched(Guid nodeProxyId)
        {
            lock (_activeNodes)
            {
                bool result = _searchedNodes.Contains(nodeProxyId);
                //System.Diagnostics.Debug.WriteLine(result.ToString());
                return result;
            }
        }



        public override string ToString()
        {
            if (!HasCompleted)
            {
                return string.Format("ACTIVE: {0}", _activeNodes.Count);
            }
            else
            {
                return "COMPLETE";
            }
        }
    }
}
