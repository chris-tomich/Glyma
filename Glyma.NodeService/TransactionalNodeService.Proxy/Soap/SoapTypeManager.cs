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
using Service = TransactionalNodeService.Service;
using System.Collections.Generic;
using Proxy = TransactionalNodeService.Proxy;

namespace TransactionalNodeService.Soap
{
    internal class SoapTypeManager
    {
        private Proxy.ITypeManager<Service.NT, Proxy.NodeType> _nodeTypes = null;
        private Proxy.ITypeManager<Service.RT, Proxy.RelationshipType> _relationshipTypes = null;
        private Proxy.ITypeManager<Service.DT, Proxy.ConnectionType> _connectionTypes = null;
        private Proxy.ITypeManager<Service.MetadataType, Proxy.MetadataType> _metadataTypes = null;

        public SoapTypeManager()
        {
        }

        public Proxy.ITypeManager<Service.NT, Proxy.NodeType> NodeTypes
        {
            get
            {
                if (_nodeTypes == null)
                {
                    _nodeTypes = new Proxy.NodeTypeManager();
                }

                return _nodeTypes;
            }
        }

        public Proxy.ITypeManager<Service.RT, Proxy.RelationshipType> RelationshipTypes
        {
            get
            {
                if (_relationshipTypes == null)
                {
                    _relationshipTypes = new Proxy.RelationshipTypeManager();
                }

                return _relationshipTypes;
            }
        }

        public Proxy.ITypeManager<Service.DT, Proxy.ConnectionType> ConnectionTypes
        {
            get
            {
                if (_connectionTypes == null)
                {
                    _connectionTypes = new Proxy.ConnectionTypeManager();
                }

                return _connectionTypes;
            }
        }

        public Proxy.ITypeManager<Service.MetadataType, Proxy.MetadataType> MetadataTypes
        {
            get
            {
                if (_metadataTypes == null)
                {
                    _metadataTypes = new Proxy.MetadataTypeManager();
                }

                return _metadataTypes;
            }
        }

        public void AddNodeTypes(IEnumerable<Service.NT> serviceNodeTypes)
        {
            foreach (Service.NT serviceNodeType in serviceNodeTypes)
            {
                Proxy.NodeType proxyNodeType = new Proxy.NodeType();
                proxyNodeType.LoadTypeInfo(serviceNodeType);

                NodeTypes.Add(serviceNodeType, proxyNodeType);
            }
        }

        public void AddRelationshipTypes(IEnumerable<Service.RT> serviceRelationshipTypes)
        {
            foreach (Service.RT serviceRelationshipType in serviceRelationshipTypes)
            {
                Proxy.RelationshipType proxyRelationshipType = new Proxy.RelationshipType();
                proxyRelationshipType.LoadTypeInfo(serviceRelationshipType);

                RelationshipTypes.Add(serviceRelationshipType, proxyRelationshipType);
            }
        }

        public void AddConnectionTypes(IEnumerable<Service.DT> serviceDescriptorTypes)
        {
            foreach (Service.DT serviceDescriptorType in serviceDescriptorTypes)
            {
                Proxy.ConnectionType proxyConnectionType = new Proxy.ConnectionType();
                proxyConnectionType.LoadTypeInfo(serviceDescriptorType);

                ConnectionTypes.Add(serviceDescriptorType, proxyConnectionType);
            }
        }

        public void AddMetadataTypes(IEnumerable<Service.MetadataType> serviceMetadataTypes)
        {
            foreach (Service.MetadataType serviceMetadataType in serviceMetadataTypes)
            {
                Proxy.MetadataType proxyMetadataType = new Proxy.MetadataType();
                proxyMetadataType.LoadTypeInfo(serviceMetadataType);

                MetadataTypes.Add(serviceMetadataType, proxyMetadataType);
            }
        }
    }
}
