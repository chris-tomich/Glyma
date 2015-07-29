using System.Collections.Generic;
using TransactionalNodeService.Proxy.Universal.Collections.MetadataCollection;

namespace TransactionalNodeService.Proxy.Universal.Soap.TransactionFramework
{
    public class InProcessTransactionResponse
    {
        private List<INode> _nodes = null;
        private List<IRelationship> _relationships;
        private List<IMetadataSet> _metadata;

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

        public List<INode> Nodes
        {
            get
            {
                if (_nodes == null)
                {
                    _nodes = new List<INode>();
                }

                return _nodes;
            }
        }

        public List<IRelationship> Relationships
        {
            get
            {
                if (_relationships == null)
                {
                    _relationships = new List<IRelationship>();
                }

                return _relationships;
            }
        }

        public List<IMetadataSet> Metadata
        {
            get
            {
                if (_metadata == null)
                {
                    _metadata = new List<IMetadataSet>();
                }

                return _metadata;
            }
        }
    }
}
