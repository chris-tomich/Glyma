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
using ServerObjects = TransactionalNodeService.Proxy.ServerObjects;

namespace TransactionalNodeService.Proxy
{
    public class RelationshipMetadataCollection : MetadataCollection
    {
        public RelationshipMetadataCollection(Proxy.IRelationship parent)
        {
            Parent = parent;
        }

        private Proxy.IRelationship Parent
        {
            get;
            set;
        }

        public void Load(RelationshipMetadataCollection metadata)
        {
            if (this != metadata)
            {
                Metadata = Metadata.Union(metadata.Metadata, MetadataSetComparer.Instance).ToList();
            }
        }

        //public void Load(Guid metadataId, string name, string value)
        //{
        //    Load(metadataId, null, name, value);
        //}

        //public void Load(Guid metadataId, INode node, string name, string value)
        //{
        //    Load(metadataId, node, null, name, value);
        //}

        public IMetadataSet Load(ServerObjects.Metadata serviceMetadata, INode node)
        {
            IMetadataSet metadataSet = MetadataSetFactory.GetInstance(MapManager).GetMetadata(serviceMetadata, node, Parent);

            Metadata.Add(metadataSet);

            IMetadataSetManager metadataSetManager = metadataSet as IMetadataSetManager;

            if (metadataSetManager != null)
            {
                metadataSetManager.Container = this;
            }

            return metadataSet;
        }

        //public void Load(Guid metadataId, INode node, ConnectionType connectionType, string name, string value)
        //{
        //    IMetadataSet metadataTriple = MetadataSetFactory.GetInstance(MapManager).GetMetadata(metadataId, name, value, node, Parent, connectionType);
            //metadataTriple.Relationship = Parent;
            //metadataTriple.Node = node;
            //metadataTriple.ConnectionType = connectionType;
            //metadataTriple.Name = name;
            //metadataTriple.Value = value;

        //    Metadata.Add(metadataTriple);
        //}

        public IMetadataSet FindMetadata(string name)
        {
            IMetadataSet metadataValue = (from metadataTriple in Metadata
                                          orderby metadataTriple.Node ascending
                                          where metadataTriple.Name == name
                                          select metadataTriple).First();

            return metadataValue;
        }

        public IMetadataSet FindMetadata(INode node, string name)
        {
            IMetadataSet metadataValue = (from metadataTriple in Metadata
                                          orderby metadataTriple.Node ascending
                                          where metadataTriple.Name == name && metadataTriple.Node.Equals(node)
                                          select metadataTriple).First();

            return metadataValue;
        }

        public IMetadataSet FindMetadata(INode node, ConnectionType connectionType, string name)
        {
            IMetadataSet metadataValue = (from metadataTriple in Metadata
                                          orderby metadataTriple.Node ascending
                                          where metadataTriple.Name == name && metadataTriple.Node.Equals(node) && metadataTriple.ConnectionType.Equals(connectionType)
                                          select metadataTriple).First();

            return metadataValue;
        }

        public IDictionary<string, IMetadataSet> FindMetadata()
        {
            IDictionary<string, IMetadataSet> metadataValue = (from metadataTriple in Metadata
                                                               orderby metadataTriple.Node ascending
                                                               where metadataTriple.Node == null && metadataTriple.ConnectionType == null
                                                               select metadataTriple).ToDictionary(item => item.Name, item => item);

            return metadataValue;
        }

        public IDictionary<string, IMetadataSet> FindMetadata(INode node)
        {
            IDictionary<string, IMetadataSet> metadataValue = (from metadataTriple in Metadata
                                                               orderby metadataTriple.Node ascending
                                                               where metadataTriple.Node.Equals(node) && metadataTriple.ConnectionType == null
                                                               select metadataTriple).ToDictionary(item => item.Name, item => item);

            return metadataValue;
        }

        public IDictionary<string, IMetadataSet> FindMetadata(INode node, ConnectionType connectionType)
        {
            IDictionary<string, IMetadataSet> metadataValue = (from metadataTriple in Metadata
                                                               orderby metadataTriple.Node ascending
                                                               where metadataTriple.Node.Equals(node) && metadataTriple.ConnectionType.Equals(connectionType)
                                                               select metadataTriple).ToDictionary(item => item.Name, item => item);

            return metadataValue;
        }
    }
}
