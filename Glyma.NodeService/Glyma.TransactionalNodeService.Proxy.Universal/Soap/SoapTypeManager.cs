using System.Collections.Generic;
using TransactionalNodeService.Proxy.Universal.TypeManagers;
using TransactionalNodeService.Proxy.Universal.Types;

namespace TransactionalNodeService.Proxy.Universal.Soap
{
    internal class SoapTypeManager
    {
        private ITypeManager<Service.NT, NodeType> _nodeTypes = null;
        private ITypeManager<Service.RT, RelationshipType> _relationshipTypes = null;
        private ITypeManager<Service.DT, ConnectionType> _connectionTypes = null;
        private ITypeManager<Service.MetadataType, MetadataType> _metadataTypes = null;

        public SoapTypeManager()
        {
        }

        public ITypeManager<Service.NT, NodeType> NodeTypes
        {
            get
            {
                if (_nodeTypes == null)
                {
                    _nodeTypes = new NodeTypeManager();
                }

                return _nodeTypes;
            }
        }

        public ITypeManager<Service.RT, RelationshipType> RelationshipTypes
        {
            get
            {
                if (_relationshipTypes == null)
                {
                    _relationshipTypes = new RelationshipTypeManager();
                }

                return _relationshipTypes;
            }
        }

        public ITypeManager<Service.DT, ConnectionType> ConnectionTypes
        {
            get
            {
                if (_connectionTypes == null)
                {
                    _connectionTypes = new ConnectionTypeManager();
                }

                return _connectionTypes;
            }
        }

        public ITypeManager<Service.MetadataType, MetadataType> MetadataTypes
        {
            get
            {
                if (_metadataTypes == null)
                {
                    _metadataTypes = new MetadataTypeManager();
                }

                return _metadataTypes;
            }
        }

        public void AddNodeTypes(IEnumerable<Service.NT> serviceNodeTypes)
        {
            foreach (Service.NT serviceNodeType in serviceNodeTypes)
            {
                NodeType proxyNodeType = new NodeType();
                proxyNodeType.LoadTypeInfo(serviceNodeType);

                NodeTypes.Add(serviceNodeType, proxyNodeType);
            }
        }

        public void AddRelationshipTypes(IEnumerable<Service.RT> serviceRelationshipTypes)
        {
            foreach (Service.RT serviceRelationshipType in serviceRelationshipTypes)
            {
                RelationshipType proxyRelationshipType = new RelationshipType();
                proxyRelationshipType.LoadTypeInfo(serviceRelationshipType);

                RelationshipTypes.Add(serviceRelationshipType, proxyRelationshipType);
            }
        }

        public void AddConnectionTypes(IEnumerable<Service.DT> serviceDescriptorTypes)
        {
            foreach (Service.DT serviceDescriptorType in serviceDescriptorTypes)
            {
                ConnectionType proxyConnectionType = new ConnectionType();
                proxyConnectionType.LoadTypeInfo(serviceDescriptorType);

                ConnectionTypes.Add(serviceDescriptorType, proxyConnectionType);
            }
        }

        public void AddMetadataTypes(IEnumerable<Service.MetadataType> serviceMetadataTypes)
        {
            foreach (Service.MetadataType serviceMetadataType in serviceMetadataTypes)
            {
                MetadataType proxyMetadataType = new MetadataType();
                proxyMetadataType.LoadTypeInfo(serviceMetadataType);

                MetadataTypes.Add(serviceMetadataType, proxyMetadataType);
            }
        }
    }
}
