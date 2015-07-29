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
using ServerObjects = TransactionalNodeService.Proxy.ServerObjects;
using System.Collections.Generic;

namespace TransactionalNodeService.Soap
{
    public class SoapToServerObjectConverter
    {
        public SoapToServerObjectConverter()
        {
        }

        public ServerObjects.Node ToNode(Service.NO soapServiceNode)
        {
            ServerObjects.Node soNode = new ServerObjects.Node();
            soNode.Depth = soapServiceNode.DH;
            soNode.DomainUid = soapServiceNode.D;
            soNode.RootMapUid = soapServiceNode.RM;
            soNode.NodeOriginalId = soapServiceNode.O;
            soNode.NodeTypeUid = soapServiceNode.T;
            soNode.NodeUid = soapServiceNode.N;

            foreach (KeyValuePair<Service.MetadataContext, Service.DA> metadataContextPair in soapServiceNode.M)
            {
                Service.MetadataContext soapServiceMetadataContext = metadataContextPair.Key;
                Service.DA soapServiceMetadata = metadataContextPair.Value;

                ServerObjects.Metadata soMetadata = ToMetadata(soapServiceMetadata);

                ServerObjects.MetadataContext soMetadataContext = new ServerObjects.MetadataContext();
                soMetadataContext.DescriptorTypeUid = soapServiceMetadataContext.DescriptorTypeUid;
                soMetadataContext.MetadataId = soapServiceMetadataContext.MetadataId;
                soMetadataContext.MetadataName = soapServiceMetadataContext.MetadataName;
                soMetadataContext.NodeUid = soapServiceMetadataContext.NodeUid;
                soMetadataContext.RelationshipUid = soapServiceMetadataContext.RelationshipUid;

                soNode.Metadata.Add(soMetadataContext, soMetadata);
            }

            return soNode;
        }

        public ServerObjects.Relationship ToRelationship(Service.RE soapServiceRelationship)
        {
            ServerObjects.Relationship soRelationship = new ServerObjects.Relationship();
            soRelationship.DomainUid = soapServiceRelationship.D;
            soRelationship.RootMapUid = soapServiceRelationship.RM;
            soRelationship.RelationshipOriginalId = soapServiceRelationship.O;
            soRelationship.RelationshipTypeUid = soapServiceRelationship.T;
            soRelationship.RelationshipUid = soapServiceRelationship.R;

            foreach (KeyValuePair<Guid, Guid> nodePair in soapServiceRelationship.N)
            {
                soRelationship.Nodes.Add(nodePair.Key, nodePair.Value);
            }

            foreach (KeyValuePair<Service.MetadataContext, Service.DA> metadataContextPair in soapServiceRelationship.M)
            {
                Service.MetadataContext soapServiceMetadataContext = metadataContextPair.Key;
                Service.DA soapServiceMetadata = metadataContextPair.Value;

                ServerObjects.Metadata soMetadata = ToMetadata(soapServiceMetadata);

                ServerObjects.MetadataContext soMetadataContext = new ServerObjects.MetadataContext();
                soMetadataContext.DescriptorTypeUid = soapServiceMetadataContext.DescriptorTypeUid;
                soMetadataContext.MetadataId = soapServiceMetadataContext.MetadataId;
                soMetadataContext.MetadataName = soapServiceMetadataContext.MetadataName;
                soMetadataContext.NodeUid = soapServiceMetadataContext.NodeUid;
                soMetadataContext.RelationshipUid = soapServiceMetadataContext.RelationshipUid;

                soRelationship.Metadata.Add(soMetadataContext, soMetadata);
            }

            return soRelationship;
        }

        public ServerObjects.Metadata ToMetadata(Service.DA soapServiceMetadata)
        {
            ServerObjects.Metadata soMetadata = new ServerObjects.Metadata();
            soMetadata.DescriptorTypeUid = soapServiceMetadata.D;
            soMetadata.MetadataId = soapServiceMetadata.M;
            soMetadata.DomainUid = soapServiceMetadata.DI;
            soMetadata.RootMapUid = soapServiceMetadata.RM;
            soMetadata.MetadataName = soapServiceMetadata.MN;
            soMetadata.MetadataTypeUid = soapServiceMetadata.T;
            soMetadata.MetadataValue = soapServiceMetadata.MV;
            soMetadata.NodeUid = soapServiceMetadata.N;
            soMetadata.RelationshipUid = soapServiceMetadata.R;

            return soMetadata;
        }

        public ServerObjects.QueryResponse ToQueryResponse(Service.QueryResponse soapServiceQueryResponse)
        {
            ServerObjects.QueryResponse soQueryResponse = new ServerObjects.QueryResponse();

            if (soapServiceQueryResponse == null || soapServiceQueryResponse.Domain == null)
            {
                return soQueryResponse;
            }

            soQueryResponse.Domain = soapServiceQueryResponse.Domain.DomainUid;
            soQueryResponse.ErrorId = soapServiceQueryResponse.ErrorId;
            soQueryResponse.ErrorMessage = soapServiceQueryResponse.ErrorMessage;
            soQueryResponse.FinalObjectIndex = soapServiceQueryResponse.FinalObjectIndex;
            soQueryResponse.LastObjectIndex = soapServiceQueryResponse.LastObjectIndex;
            soQueryResponse.StartingObjectIndex = soapServiceQueryResponse.StartingObjectIndex;

            if (soapServiceQueryResponse.NodeContext != null)
            {
                soQueryResponse.NodeContext = ToNode(soapServiceQueryResponse.NodeContext);
            }

            foreach (Service.NO soapServiceNode in soapServiceQueryResponse.Nodes.Values)
            {
                ServerObjects.Node soNode = ToNode(soapServiceNode);

                soQueryResponse.Nodes.Add(soNode.NodeUid, soNode);
            }

            foreach (Service.RE soapServiceRelationship in soapServiceQueryResponse.Relationships.Values)
            {
                ServerObjects.Relationship soRelationship = ToRelationship(soapServiceRelationship);

                soQueryResponse.Relationships.Add(soRelationship.RelationshipUid, soRelationship);
            }

            return soQueryResponse;
        }
    }
}
