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

namespace TransactionalNodeService.Proxy
{
    public class ConnectionSetFactory
    {
        private static ConnectionSetFactory instance = null;
        private static readonly object padlock = new object();

        private Dictionary<INode, Dictionary<IRelationship, Dictionary<ConnectionType, ConnectionSet>>> _connections;

        private ConnectionSetFactory()
        {
        }

        public static ConnectionSetFactory Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new ConnectionSetFactory();
                    }

                    return instance;
                }
            }
        }

        private Dictionary<INode, Dictionary<IRelationship, Dictionary<ConnectionType, ConnectionSet>>> Connections
        {
            get
            {
                if (_connections == null)
                {
                    _connections = new Dictionary<INode, Dictionary<IRelationship, Dictionary<ConnectionType, ConnectionSet>>>();
                }

                return _connections;
            }
        }

        public ConnectionSet GetConnection(INode node, IRelationship relationship, ConnectionType connectionType)
        {
            if (Connections.ContainsKey(node))
            {
                if (Connections[node].ContainsKey(relationship))
                {
                    if (Connections[node][relationship].ContainsKey(connectionType))
                    {
                        return Connections[node][relationship][connectionType];
                    }
                }
                else
                {
                    Connections[node][relationship] = new Dictionary<ConnectionType, ConnectionSet>();
                }
            }
            else
            {
                Connections[node] = new Dictionary<IRelationship, Dictionary<ConnectionType, ConnectionSet>>();
                Connections[node][relationship] = new Dictionary<ConnectionType, ConnectionSet>();
            }

            ConnectionSet newConnectionSet = new ConnectionSet() { Node = node, Relationship = relationship, ConnectionType = connectionType };

            Connections[node][relationship][connectionType] = newConnectionSet;

            return newConnectionSet;
        }
    }
}
