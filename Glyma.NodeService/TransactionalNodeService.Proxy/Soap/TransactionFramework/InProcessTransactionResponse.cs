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
using Proxy = TransactionalNodeService.Proxy;

namespace TransactionalNodeService.Soap.TransactionFramework
{
    public class InProcessTransactionResponse
    {
        private List<Proxy.INode> _nodes = null;
        private List<Proxy.IRelationship> _relationships;
        private List<Proxy.IMetadataSet> _metadata;

        public InProcessTransactionResponse()
        {
        }

        public void Add(InProcessTransactionResponse response)
        {
            if (response.Nodes.Count > 0)
            {
                Nodes.AddRange(response.Nodes);
            }

            if (response.Relationships.Count > 0)
            {
                Relationships.AddRange(response.Relationships);
            }

            if (response.Metadata.Count > 0)
            {
                Metadata.AddRange(response.Metadata);
            }
        }

        public List<Proxy.INode> Nodes
        {
            get
            {
                if (_nodes == null)
                {
                    _nodes = new List<Proxy.INode>();
                }

                return _nodes;
            }
        }

        public List<Proxy.IRelationship> Relationships
        {
            get
            {
                if (_relationships == null)
                {
                    _relationships = new List<Proxy.IRelationship>();
                }

                return _relationships;
            }
        }

        public List<Proxy.IMetadataSet> Metadata
        {
            get
            {
                if (_metadata == null)
                {
                    _metadata = new List<Proxy.IMetadataSet>();
                }

                return _metadata;
            }
        }
    }
}
