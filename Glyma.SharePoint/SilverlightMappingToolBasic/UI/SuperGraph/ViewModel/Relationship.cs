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
using SilverlightMappingToolBasic.UI.ViewModel;
using TNSProxy = TransactionalNodeService.Proxy;
using System.Collections.Generic;

namespace SilverlightMappingToolBasic.UI.SuperGraph.ViewModel
{
    public class Relationship : IRelationship
    {
        public Relationship()
        {
        }

        public TNSProxy.IRelationship Proxy
        {
            get;
            private set;
        }

        public Guid Id
        {
            get
            {
                if (Proxy != null)
                {
                    return Proxy.ClientId;
                }
                else
                {
                    return Guid.Empty;
                }
            }
        }

        public Guid DomainId
        {
            get
            {
                if (Proxy != null)
                {
                    return Proxy.DomainId;
                }
                else
                {
                    return Guid.Empty;
                }
            }
        }

        public string OriginalId
        {
            get
            {
                if (Proxy != null)
                {
                    return Proxy.OriginalId;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public Guid MapObjectType
        {
            get
            {
                if (Proxy != null)
                {
                    return Proxy.RelationshipType.Id;
                }
                else
                {
                    return Guid.Empty;
                }
            }
        }

        public Guid From
        {
            get;
            set;
        }

        public Guid To
        {
            get;
            set;
        }

        public void LoadRelationship(TNSProxy.IRelationship relationship)
        {
            Proxy = relationship;

            if (relationship.Status == TNSProxy.LoadState.Full)
            {
                IEnumerable<TNSProxy.NodeTuple> nodes = relationship.Nodes.FindNodes();

                foreach (TNSProxy.NodeTuple nodeTuple in relationship.Nodes.FindNodes())
                {
                    if (nodeTuple.ConnectionType.Name == "To")
                    {
                        To = nodeTuple.Node.ClientId;
                    }
                    else if (nodeTuple.ConnectionType.Name == "From")
                    {
                        From = nodeTuple.Node.ClientId;
                    }
                }
            }
        }
    }
}
