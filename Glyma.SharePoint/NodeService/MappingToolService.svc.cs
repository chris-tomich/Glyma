using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Xml.Serialization;
using System.IO;
using System.Drawing;
using System.Data.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Xml;

using System.Data;
using System.Data.SqlClient;

using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Client.Services;

using ICSharpCode.SharpZipLib.Zip;

using SevenSigma.Logging;

namespace NodeService
{
    [BasicHttpBindingServiceMetadataExchangeEndpointAttribute]
    [AspNetCompatibilityRequirements(RequirementsMode=AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single, Namespace = "http://sevensigma.com.au/NodeService")]
    public class MappingToolService : IMappingToolService
    {
        private List<string> evaluatedNodes = new List<string>();
        private List<string> excludedNodes = new List<string>();
        private object deleteLock = new object();
        
        public MappingToolService()
        {
        }

        public SoapNode CreateNewDomain(string domainName)
        {
            SoapNode soapDomainNode = null;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (MappingToolDatabaseDataContext mappingDb = new MappingToolDatabaseDataContext())
                {
                    var nodeTypes = from dbNodeTypes in mappingDb.NodeTypes where dbNodeTypes.NodeTypeName == "DomainNode" select dbNodeTypes;
                    var metadataTypes = from dbMetadataTypes in mappingDb.MetadataTypes where dbMetadataTypes.MetadataTypeName == "string" select dbMetadataTypes;

                    NodeType nodeType = nodeTypes.First();
                    MetadataType metadataType = metadataTypes.First();

                    Domain domain = new Domain();
                    domain.DomainUid = Guid.NewGuid();

                    mappingDb.Domains.InsertOnSubmit(domain);
                    mappingDb.SubmitChanges();

                    Node domainNode = new Node();
                    domainNode.DomainUid = domain.DomainUid;
                    domainNode.NodeUid = Guid.NewGuid();
                    domainNode.NodeType = nodeType;
                    domainNode.NodeTypeUid = nodeType.NodeTypeUid;

                    mappingDb.Nodes.InsertOnSubmit(domainNode);
                    mappingDb.SubmitChanges();

                    SoapMetadataType soapMetadataType = new SoapMetadataType();
                    soapMetadataType.Id = metadataType.MetadataTypeUid;
                    soapMetadataType.Name = metadataType.MetadataTypeName;

                    soapDomainNode = UpdateNodeMetadata(domain.DomainUid, domainNode.NodeUid, Guid.Empty, null, "Name", domainName, soapMetadataType);
                }
            });
            return soapDomainNode;
        }

        public SoapNode AddNode(Guid domainId, SoapNodeType nodeType, string originalId)
        {
            SoapNode soapNode = null;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (MappingToolDatabaseDataContext mappingDb = new MappingToolDatabaseDataContext())
                {
                    var domains = from dbDomain in mappingDb.Domains where dbDomain.DomainUid == domainId select dbDomain;

                    Domain domain;

                    if (domains.Count() > 0)
                    {
                        domain = domains.First();
                    }
                    else
                    {
                        domain = new Domain();
                        domain.DomainUid = domainId;
                        domain.DomainOriginalId = domainId.ToString();

                        mappingDb.Domains.InsertOnSubmit(domain);
                        mappingDb.SubmitChanges();
                    }

                    Node node = CreateNode(nodeType.Id, originalId);

                    domain.Nodes.Add(node);

                    mappingDb.SubmitChanges();

                    soapNode = new SoapNode();
                    soapNode.Domain = domainId;
                    soapNode.Id = node.NodeUid;
                    soapNode.NodeType = nodeType;
                }
            });
            return soapNode;
        }

        private static Node CreateNode(Guid nodeTypeId, string originalId)
        {
            Node node = new Node();
            node.NodeUid = Guid.NewGuid();
            node.NodeOriginalId = originalId;
            node.NodeTypeUid = nodeTypeId;
            return node;
        }

        public DeleteResult DeleteMetadata(MetadataContext context)
        {
            DeleteResult result = new DeleteResult() { DeleteSuccessful = false, DeletedId = Guid.Empty };
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (MappingToolDatabaseDataContext mappingDb = new MappingToolDatabaseDataContext())
                {
                    if ((context.RelationshipUid != null && context.RelationshipUid.HasValue) && 
                        (context.DescriptorTypeUid != null && context.DescriptorTypeUid.HasValue))
                    {
                        var deleteMetadatas = from metadata in mappingDb.Metadatas
                                          where metadata.NodeUid.Value == context.NodeUid.Value
                                          && metadata.DescriptorTypeUid.Value == context.DescriptorTypeUid.Value
                                          && metadata.RelationshipUid.Value == context.RelationshipUid.Value
                                          && metadata.MetadataName == context.MetadataName
                                          select metadata;
                        DeleteMetadata(context, result, mappingDb, deleteMetadatas);
                    }
                    else if ((context.RelationshipUid == null || !context.RelationshipUid.HasValue) && 
                        (context.DescriptorTypeUid != null || context.DescriptorTypeUid.HasValue))
                    {
                        var deleteMetadatas = from metadata in mappingDb.Metadatas
                                          where metadata.NodeUid.Value == context.NodeUid.Value
                                          && metadata.DescriptorTypeUid.Value == context.DescriptorTypeUid.Value
                                          && metadata.MetadataName == context.MetadataName
                                          select metadata;
                        DeleteMetadata(context, result, mappingDb, deleteMetadatas);
                    }
                    else if ((context.RelationshipUid != null || context.RelationshipUid.HasValue) &&
                        (context.DescriptorTypeUid == null || !context.DescriptorTypeUid.HasValue))
                    {
                        var deleteMetadatas = from metadata in mappingDb.Metadatas
                                          where metadata.NodeUid.Value == context.NodeUid.Value
                                          && metadata.RelationshipUid.Value == context.RelationshipUid.Value
                                          && metadata.MetadataName == context.MetadataName
                                          select metadata;
                        DeleteMetadata(context, result, mappingDb, deleteMetadatas);
                    }
                    else
                    {
                        var deleteMetadatas = from metadata in mappingDb.Metadatas
                                              where metadata.NodeUid.Value == context.NodeUid.Value
                                              && metadata.MetadataName == context.MetadataName
                                              select metadata;

                        DeleteMetadata(context, result, mappingDb, deleteMetadatas);
                    }

                    
                }
            });
            return result;
        }

        private static void DeleteMetadata(MetadataContext context, DeleteResult result, MappingToolDatabaseDataContext mappingDb, IQueryable<Metadata> deleteMetadatas)
        {
            if (deleteMetadatas.Count() == 1)
            {
                Metadata metadata = deleteMetadatas.First();
                if (metadata != null)
                {
                    mappingDb.Metadatas.DeleteOnSubmit(metadata);
                    try
                    {
                        mappingDb.SubmitChanges();
                        result.DeleteSuccessful = true;
                        result.DeletedId = metadata.MetadataId;
                        LoggingService.WriteTrace(LoggingService.Categories.WcfServices, TraceSeverity.Verbose,
                            "Deleted metadata with ID: {0} the value was: '{1}'", metadata.MetadataId.ToString(),
                            metadata.MetadataValue);
                    }
                    catch (Exception e)
                    {
                        LoggingService.WriteTrace(LoggingService.Categories.WcfServices, TraceSeverity.Unexpected,
                            "There was an error deleting the metadata with ID: {0} due to {1}: {2}",
                            metadata.MetadataId.ToString(), e.GetType().ToString(), e.Message);
                    }
                }
            }
            else
            {
                LoggingService.WriteTrace(LoggingService.Categories.WcfServices, TraceSeverity.Unexpected,
                    "There was an error deleting the metadata with the name {0} from the NodeUid: {1}",
                    context.MetadataName, context.NodeUid);
            }
        }

        public DeleteResult DeleteRelationship(Guid domainId, Guid relationshipId)
        {
            lock (deleteLock)
            {
                DeleteResult result = new DeleteResult() { DeleteSuccessful = false, DeletedId = relationshipId };
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (MappingToolDatabaseDataContext mappingDb = new MappingToolDatabaseDataContext())
                    {
                        var deleteRelationships = from relationships in mappingDb.Relationships
                                                  where relationships.RelationshipUid == relationshipId && relationships.DomainUid == domainId
                                                  select relationships;

                        //if the relationship existed it'll be the only one and can delete all the descriptors and metadatum
                        if (deleteRelationships.Count() > 0)
                        {
                            var relationship = deleteRelationships.First();

                            var deleteDescriptors = from descriptors in mappingDb.Descriptors
                                                    where descriptors.RelationshipUid == relationshipId
                                                    select descriptors;

                            foreach (var descriptor in deleteDescriptors)
                            {
                                mappingDb.Descriptors.DeleteOnSubmit(descriptor);
                            }

                            try
                            {
                                mappingDb.SubmitChanges();
                            }
                            catch (Exception)
                            {
                                //Debug.WriteLine("An exception occurred of type: {0}\r\n{1}", e.Message, e);
                            }

                            // Delete metadata associated with the relationship just deleted
                            var deleteRelationshipMetadatum = from metadatum in mappingDb.Metadatas
                                                              where metadatum.RelationshipUid == relationshipId
                                                              select metadatum;
                            foreach (var relationshipMetadatum in deleteRelationshipMetadatum)
                            {
                                mappingDb.Metadatas.DeleteOnSubmit(relationshipMetadatum);
                            }
                            try
                            {
                                mappingDb.SubmitChanges();
                            }
                            catch (Exception)
                            {
                                //Debug.WriteLine("An exception occurred of type: {0}\r\n{1}", e.Message, e);
                            }

                            mappingDb.Relationships.DeleteOnSubmit(relationship);
                            try
                            {
                                mappingDb.SubmitChanges();
                                result.DeleteSuccessful = true;
                            }
                            catch (Exception)
                            {
                                //Debug.WriteLine("An exception occurred of type: {0}\r\n{1}", e.Message, e);
                            }
                        }
                    }
                });
                return result;
            }
        }

        public DeleteResult DeleteNode(Guid domainId, Guid nodeId)
        {
            lock (deleteLock)
            {
                List<Guid> relationshipIds = new List<Guid>();
                List<Guid> deletedDescriptors = new List<Guid>();
                DeleteResult result = new DeleteResult() { DeleteSuccessful = false, DeletedId = nodeId };
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (MappingToolDatabaseDataContext mappingDb = new MappingToolDatabaseDataContext())
                    {
                        var deleteNodes = from nodes in mappingDb.Nodes
                                          where nodes.NodeUid == nodeId && nodes.DomainUid == domainId
                                          select nodes;
                        //if the node existed it'll be the only one and delete all associated relationships/descriptors and metadatum
                        if (deleteNodes.Count() > 0)
                        {
                            // Delete the descriptors that have the node to be deleted associated with them directly
                            var deleteDescriptors = from descriptors in mappingDb.Descriptors
                                                    where descriptors.NodeUid == nodeId
                                                    select descriptors;
                            foreach (var descriptor in deleteDescriptors)
                            {
                                relationshipIds.Add(descriptor.RelationshipUid.Value);
                                deletedDescriptors.Add(descriptor.DescriptorUid);
                                mappingDb.Descriptors.DeleteOnSubmit(descriptor);
                            }
                            try
                            {
                                mappingDb.SubmitChanges();
                            }
                            catch (Exception)
                            {
                                //Debug.WriteLine("An exception occurred of type: {0}\r\n{1}", e.Message, e);
                            }

                            foreach (Guid relationshipId in relationshipIds)
                            {
                                var deleteRelationships = from relationships in mappingDb.Relationships
                                                          where relationships.RelationshipUid == relationshipId
                                                          select relationships;
                                if (deleteRelationships.Count() > 0)
                                {
                                    var relationship = deleteRelationships.First();

                                    // Delete the descriptors that dangle off the other side of the relationship just deleted
                                    var deleteAltDescriptors = from descriptors2 in mappingDb.Descriptors
                                                               where descriptors2.RelationshipUid == relationshipId
                                                               select descriptors2;
                                    foreach (var altDescriptor in deleteAltDescriptors)
                                    {
                                        if (!deletedDescriptors.Contains(altDescriptor.DescriptorUid))
                                        {
                                            mappingDb.Descriptors.DeleteOnSubmit(altDescriptor);
                                        }
                                    }

                                    try
                                    {
                                        mappingDb.SubmitChanges();
                                    }
                                    catch (Exception)
                                    {
                                        //Debug.WriteLine("An exception occurred of type: {0}\r\n{1}", e.Message, e);
                                    }

                                    // Delete metadata associated with the relationship just deleted
                                    var deleteRelationshipMetadatum = from metadatum in mappingDb.Metadatas
                                                                      where metadatum.RelationshipUid == relationshipId
                                                                      select metadatum;
                                    foreach (var relationshipMetadatum in deleteRelationshipMetadatum)
                                    {
                                        mappingDb.Metadatas.DeleteOnSubmit(relationshipMetadatum);
                                    }
                                    try
                                    {
                                        mappingDb.SubmitChanges();
                                    }
                                    catch (Exception)
                                    {
                                        //Debug.WriteLine("An exception occurred of type: {0}\r\n{1}", e.Message, e);
                                    }

                                    mappingDb.Relationships.DeleteOnSubmit(relationship);
                                    try
                                    {
                                        mappingDb.SubmitChanges();
                                    }
                                    catch (Exception)
                                    {
                                        //Debug.WriteLine("An exception occurred of type: {0}\r\n{1}", e.Message, e);
                                    }
                                }
                            }

                            var nodeToDelete = deleteNodes.First();
                            var deleteNodeMetadatum = from metadatum in mappingDb.Metadatas
                                                      where metadatum.NodeUid == nodeToDelete.NodeUid
                                                      select metadatum;
                            foreach (var metadatum in deleteNodeMetadatum)
                            {
                                mappingDb.Metadatas.DeleteOnSubmit(metadatum);
                            }
                            try
                            {
                                mappingDb.SubmitChanges();
                            }
                            catch (Exception)
                            {
                                //Debug.WriteLine("An exception occurred of type: {0}\r\n{1}", e.Message, e);
                            }

                            // Delete the actual node, it will only be one
                            mappingDb.Nodes.DeleteOnSubmit(nodeToDelete);
                            try
                            {
                                mappingDb.SubmitChanges();
                                result.DeleteSuccessful = true;
                            }
                            catch (Exception)
                            {
                                //Debug.WriteLine("An exception occurred of type: {0}\r\n{1}", e.Message, e);
                            }
                        }
                    }
                });
                return result;
            }
        }

        public DeleteResult DeleteNodeTransclusion(Guid domainId, Guid mapIdToDeleteFrom, Guid nodeIdToDelete)
        {
            lock (deleteLock)
            {
                DeleteResult result = new DeleteResult() { DeleteSuccessful = true };
                Guid transclusionRelationshipDeleted = Guid.Empty;
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (MappingToolDatabaseDataContext mappingDb = new MappingToolDatabaseDataContext())
                    {
                        var deleteNodes = from nodes in mappingDb.Nodes
                                          where nodes.NodeUid == nodeIdToDelete && nodes.DomainUid == domainId
                                          select nodes;

                        //if the node existed it'll be the only one and delete just the transclusion relationship
                        if (deleteNodes.Count() > 0)
                        {
                            // Delete the descriptors that have the node to be deleted associated with them directly
                            var deleteDescriptors = from descriptors in mappingDb.Descriptors
                                                    where descriptors.NodeUid == nodeIdToDelete
                                                    select descriptors;
                            foreach (var descriptor in deleteDescriptors)
                            {
                                //Delete the Transclusion Relationships
                                if (descriptor.Relationship.RelationshipType.RelationshipTypeName == "TransclusionRelationship")
                                {
                                    foreach (Descriptor relDescriptor in descriptor.Relationship.Descriptors)
                                    {
                                        if (relDescriptor.DescriptorType.DescriptorTypeName == "TransclusionMap")
                                        {
                                            if (mapIdToDeleteFrom == relDescriptor.NodeUid)
                                            {
                                                //Delete the transclusion relationship and all associated metadata
                                                DeleteResult relationshipDeleteResult = DeleteRelationship(domainId, descriptor.RelationshipUid.Value);
                                                if (!relationshipDeleteResult.DeleteSuccessful)
                                                {
                                                    result.DeleteSuccessful = false;
                                                }
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                });
                result.DeletedId = nodeIdToDelete;
                return result;
            }
        }

        private bool IsNodeInMap(Guid domainId, Guid mapId, Guid nodeId)
        {
            bool result = false;
            SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (MappingToolDatabaseDataContext mappingDb = new MappingToolDatabaseDataContext())
                    {
                        var mapContainerRelationships = Queryable.Where(Queryable.Where(mappingDb.Relationships, (r1) => r1.DomainUid == domainId), (r) => r.RelationshipType.RelationshipTypeName == "MapContainerRelationship");
                        var transclusionRelationships = Queryable.Where(Queryable.Where(mappingDb.Relationships, (r1) => r1.DomainUid == domainId), (r) => r.RelationshipType.RelationshipTypeName == "TransclusionRelationship");
                        var relationships = Queryable.Union(mapContainerRelationships, transclusionRelationships);

                        //test if the node existed
                        if (relationships.Count() > 0)
                        {
                            foreach (Relationship relationship in relationships)
                            {
                                if (result)
                                {
                                    break;
                                }
                                if (relationship.RelationshipType.RelationshipTypeName == "MapContainerRelationship")
                                {
                                    foreach (Descriptor descriptor in relationship.Descriptors.Where(
                                        d => d.DescriptorType.DescriptorTypeName == "To").Where(n => n.NodeUid == mapId))
                                    {
                                        if (result)
                                        {
                                            break;
                                        }
                                        foreach (Descriptor altDesc in descriptor.Relationship.Descriptors.Where(d => d.DescriptorUid != descriptor.DescriptorUid))
                                        {
                                            if (altDesc.NodeUid == nodeId)
                                            {
                                                result = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                                else if (relationship.RelationshipType.RelationshipTypeName == "TransclusionRelationship")
                                {
                                    foreach (Descriptor descriptor in relationship.Descriptors.Where(
                                        d => d.DescriptorType.DescriptorTypeName == "TransclusionMap").Where(n => n.NodeUid == mapId))
                                    {
                                        if (result)
                                        {
                                            break;
                                        }
                                        foreach (Descriptor altDesc in descriptor.Relationship.Descriptors.Where(d => d.DescriptorUid != descriptor.DescriptorUid))
                                        {
                                            if (altDesc.NodeUid == nodeId)
                                            {
                                                result = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                });

            return result;
        } 

        public DeleteResult DeleteNodePromoteTransclusion(Guid domainId, Guid mapIdToDeleteFrom, Guid nodeIdToDelete)
        {
            lock (deleteLock)
            {
                DeleteResult result = new DeleteResult() { DeleteSuccessful = true };
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (MappingToolDatabaseDataContext mappingDb = new MappingToolDatabaseDataContext())
                    {
                        Dictionary<string, SoapNodeType> soapNodeTypes;
                        Dictionary<string, SoapRelationshipType> soapRelTypes;
                        Dictionary<string, SoapMetadataType> soapMetaTypes;
                        Dictionary<string, SoapDescriptorType> soapDescTypes;
                        GetSoapTypes(out soapNodeTypes, out soapRelTypes, out soapMetaTypes, out soapDescTypes);

                        var deleteNodes = from nodes in mappingDb.Nodes
                                          where nodes.NodeUid == nodeIdToDelete && nodes.DomainUid == domainId
                                          select nodes;

                        //if the node existed it'll be the only one and delete just the transclusion relationship
                        if (deleteNodes.Count() > 0)
                        {
                            DescriptorType fromDescriptorType = null;
                            var fromDecriptorTypes = from dbDescriptorTypes in mappingDb.DescriptorTypes
                                                     where dbDescriptorTypes.DescriptorTypeName.ToLower() == "from"
                                                     select dbDescriptorTypes;
                            if (fromDecriptorTypes.Count() != 1)
                            {
                                result.DeleteSuccessful = false;

                            }
                            else
                            {
                                fromDescriptorType = fromDecriptorTypes.First();

                                bool promotionCompleted = false;
                                bool deletedOriginalMapContainer = false;

                                // Delete the appropriate relationships from the node and promote a transclusion relationship
                                var deletedNodesDescriptors = from descriptors in mappingDb.Descriptors
                                                        where descriptors.NodeUid == nodeIdToDelete
                                                        select descriptors;

                                foreach (var descriptor in deletedNodesDescriptors)
                                {
                                    if (descriptor.Relationship != null)
                                    {
                                        //Delete the Map Container Relationship
                                        if (descriptor.Relationship.RelationshipType.RelationshipTypeName == "MapContainerRelationship" && !deletedOriginalMapContainer)
                                        {
                                            foreach (Descriptor relDescriptor in descriptor.Relationship.Descriptors)
                                            {
                                                if (relDescriptor.DescriptorType.DescriptorTypeName == "To")
                                                {
                                                    if (mapIdToDeleteFrom == relDescriptor.NodeUid)
                                                    {
                                                        //Delete the map container relationship and all associated metadata
                                                        DeleteResult relationshipDeleteResult = DeleteRelationship(domainId, descriptor.RelationshipUid.Value);
                                                        if (!relationshipDeleteResult.DeleteSuccessful)
                                                        {
                                                            result.DeleteSuccessful = false;
                                                        }
                                                    }
                                                    break;
                                                }
                                            }
                                            deletedOriginalMapContainer = true;
                                        }

                                        else if (descriptor.Relationship.RelationshipType.RelationshipTypeName == "TransclusionRelationship" && !promotionCompleted)
                                        {
                                            string xPos = null;
                                            string yPos = null;
                                            foreach (Descriptor d1 in descriptor.Relationship.Descriptors)
                                            {
                                                if (d1.NodeUid == nodeIdToDelete)
                                                {
                                                    foreach (Metadata metadata in d1.Node.Metadatas)
                                                    {
                                                        if (metadata.MetadataName == "XPosition" && metadata.RelationshipUid == descriptor.RelationshipUid)
                                                        {
                                                            xPos = metadata.MetadataValue;
                                                        }
                                                        else if (metadata.MetadataName == "YPosition" && metadata.RelationshipUid == descriptor.RelationshipUid)
                                                        {
                                                            yPos = metadata.MetadataValue;
                                                        }
                                                        if (xPos != null && yPos != null)
                                                        {
                                                            //found both values
                                                            break;
                                                        }
                                                    }
                                                    break;
                                                }
                                            }

                                            Guid newRootMapId = Guid.Empty;
                                            foreach (Descriptor altDescriptor in descriptor.Relationship.Descriptors)
                                            {
                                                if (altDescriptor.DescriptorType.DescriptorTypeName == "TransclusionMap")
                                                {
                                                    newRootMapId = altDescriptor.NodeUid.Value;
                                                    break;
                                                }
                                            }

                                            //Delete the transclusion relationship and all associated metadata
                                            DeleteResult relationshipDeleteResult = DeleteRelationship(domainId, descriptor.RelationshipUid.Value);
                                            if (!relationshipDeleteResult.DeleteSuccessful)
                                            {
                                                result.DeleteSuccessful = false;
                                            }

                                            if (xPos != null && yPos != null)
                                            {
                                                //Connect the Map Container Relationship between what was the transclusion map and the transcluded node (promotion to original map and original node)
                                                if (newRootMapId != Guid.Empty)
                                                {
                                                    ConnectedNodesResult connectResult = ConnectToMap(domainId, newRootMapId, nodeIdToDelete, soapDescTypes, soapRelTypes);
                                                    if (connectResult != null)
                                                    {
                                                        UpdateNodeMetadata(domainId, nodeIdToDelete, connectResult.Relationship.Id,
                                                                soapDescTypes["From"], "XPosition", xPos, soapMetaTypes["double"]);
                                                        UpdateNodeMetadata(domainId, nodeIdToDelete, connectResult.Relationship.Id,
                                                                soapDescTypes["From"], "YPosition", yPos, soapMetaTypes["double"]);

                                                        SoapNode promotedNode = connectResult.Nodes[nodeIdToDelete];
                                                        List<Guid> relationshipsToConvert = new List<Guid>();
                                                        foreach (SoapRelationship relationship in promotedNode.Relationships.Values)
                                                        {
                                                            //find all Transclusion relationships for the promotoed node on the transclusion map
                                                            if (relationship.RelationshipType.Name == "TransclusionRelationship")
                                                            {
                                                                Guid mapNodeId = relationship.Nodes.Where(d => d.Key.Name == "TransclusionMap").First().Value;
                                                                if (mapNodeId == newRootMapId)
                                                                {
                                                                    relationshipsToConvert.Add(relationship.Id);
                                                                }
                                                            }
                                                        }

                                                        foreach (Guid relationshipId in relationshipsToConvert)
                                                        {
                                                            if (promotedNode.Relationships[relationshipId].Nodes.Where(pair => pair.Key.Name == "From").Count() > 0)
                                                            {
                                                                Guid fromNodeId = promotedNode.Relationships[relationshipId].Nodes.Where(pair => pair.Key.Name == "From").First().Value;
                                                                Guid toNodeId = promotedNode.Relationships[relationshipId].Nodes.Where(pair => pair.Key.Name == "To").First().Value;
                                                                Dictionary<SoapDescriptorType, Guid> nodes = new Dictionary<SoapDescriptorType, Guid>();
                                                                nodes.Add(soapDescTypes["From"], fromNodeId);
                                                                nodes.Add(soapDescTypes["To"], toNodeId);
                                                                ConnectNodes(domainId, nodes, soapRelTypes["FromToRelationship"], relationshipId.ToString());
                                                                DeleteRelationship(domainId, relationshipId);
                                                            }
                                                        }
                                                    }
                                                    promotionCompleted = true;
                                                }
                                            }
                                        }

                                        else if (descriptor.Relationship.RelationshipType.RelationshipTypeName == "FromToRelationship")
                                        {
                                            DeleteRelationship(domainId, descriptor.RelationshipUid.Value);
                                        }
                                    }
                                }
                            }
                        }
                    }
                });
                result.DeletedId = nodeIdToDelete;
                return result;
            }
        }

        public ConnectedNodesResult ConnectNodes(Guid domainId, Dictionary<SoapDescriptorType, Guid> nodes, SoapRelationshipType relationshipType, string originalId)
        {
            ConnectedNodesResult connectedNodesResult = null;
            if (nodes.Values.Distinct().Count() > 1)
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (MappingToolDatabaseDataContext mappingDb = new MappingToolDatabaseDataContext())
                    {
                        List<SoapNode> updatedNodes = new List<SoapNode>();

                        var domains = from dbDomain in mappingDb.Domains where dbDomain.DomainUid == domainId select dbDomain;

                        var domain = domains.First();

                        Relationship relationship = new Relationship();
                        relationship.RelationshipUid = Guid.NewGuid();
                        relationship.RelationshipOriginalId = originalId;
                        relationship.RelationshipTypeUid = relationshipType.Id;

                        SoapRelationship soapRelationship = new SoapRelationship();
                        soapRelationship.Id = relationship.RelationshipUid;
                        soapRelationship.RelationshipType = relationshipType;

                        foreach (KeyValuePair<SoapDescriptorType, Guid> keyValuePair in nodes)
                        {
                            Descriptor descriptor = new Descriptor();
                            descriptor.DescriptorUid = Guid.NewGuid();
                            descriptor.RelationshipUid = relationship.RelationshipUid;
                            descriptor.DescriptorTypeUid = keyValuePair.Key.Id;

                            Node node = domain.Nodes.Single(x => x.NodeUid == keyValuePair.Value);
                            node.Domain = domain;
                            node.Descriptors.Add(descriptor);

                            soapRelationship.Nodes.Add(keyValuePair.Key, node.NodeUid);
                            SoapNode soapNode = node.ToSoapObject();
                            soapNode.Relationships.Add(soapRelationship.Id, soapRelationship);
                            updatedNodes.Add(soapNode);

                            domain.Relationships.Add(relationship);
                        }

                        mappingDb.SubmitChanges();

                        connectedNodesResult = new ConnectedNodesResult();
                        connectedNodesResult.Relationship = soapRelationship;
                        connectedNodesResult.Nodes = new Dictionary<Guid, SoapNode>();
                        foreach (SoapNode soapNode in updatedNodes)
                        {
                            if (!connectedNodesResult.Nodes.ContainsKey(soapNode.Id))
                            {
                                connectedNodesResult.Nodes.Add(soapNode.Id, soapNode);
                            }
                        }
                    }
                });
            }
            else
            {
                LoggingService.WriteTrace(LoggingService.Categories.WcfServices, TraceSeverity.Unexpected, "ConnectNodes() failed: There must be at least 2 distinct nodes connected.");
                //This will cause an error to be returned on the WFC event, over SOAP it won't contain the fault contract.
                throw new FaultException<ConnectNodesFailureFault>(new ConnectNodesFailureFault(), "There must be at least 2 distinct nodes connected.");
            }

            return connectedNodesResult;
        }

        public bool RenameNodeMetadata(Guid domainId, Guid soapNodeId, Guid soapRelationshipId, SoapDescriptorType soapDescriptorType, string originalMetadataName, string newMetadataName)
        {
            bool success = false;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (MappingToolDatabaseDataContext mappingDb = new MappingToolDatabaseDataContext())
                {
                    try
                    {
                        var nodes = from dbNode in mappingDb.Nodes where dbNode.DomainUid == domainId && dbNode.NodeUid == soapNodeId select dbNode;
                        Node node = nodes.Single();

                        foreach (Metadata metadatum in node.Metadatas.Where(x => x.MetadataName == originalMetadataName))
                        {
                            if (soapNodeId != Guid.Empty)
                            {
                                if (metadatum.NodeUid != soapNodeId)
                                {
                                    continue; //this should never happen since it's the node's metadata
                                }
                            }
                            if (soapRelationshipId != Guid.Empty)
                            {
                                if (metadatum.RelationshipUid != soapRelationshipId)
                                {
                                    continue;
                                }
                            }
                            if (soapDescriptorType != null)
                            {
                                if (metadatum.DescriptorTypeUid != soapDescriptorType.Id)
                                {
                                    continue;
                                }
                            }

                            //same context so we can rename it
                            metadatum.MetadataName = newMetadataName;
                            mappingDb.SubmitChanges();
                        }
                    }
                    catch (Exception ex)
                    {
                        LoggingService.WriteTrace(LoggingService.Categories.WcfServices, TraceSeverity.Unexpected,
                            "An error occurred renaming the metadata entry {0}, exception={1}", originalMetadataName, ex.Message);
                    }
                    finally
                    {
                        success = true;
                        LoggingService.WriteTrace(LoggingService.Categories.WcfServices, TraceSeverity.Verbose, 
                            "Metadata property on Node with Id: {0} with name: {1} was renamed to: {2}", 
                            soapNodeId.ToString(), originalMetadataName, newMetadataName);
                    }
                }
            });
            return success;
        }

        public SoapNode UpdateNodeMetadata(Guid domainId, Guid soapNodeId, Guid soapRelationshipId, SoapDescriptorType soapDescriptorType, string metadataName, string metadataValue, SoapMetadataType soapMetadataType)
        {
            SoapNode soapNode = null;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (MappingToolDatabaseDataContext mappingDb = new MappingToolDatabaseDataContext())
                {
                    var nodes = from dbNode in mappingDb.Nodes where dbNode.DomainUid == domainId && dbNode.NodeUid == soapNodeId select dbNode;

                    var node = nodes.First();

                    SoapMetadata soapMetadata = UpdateMetadata(mappingDb, soapNodeId, soapRelationshipId, soapDescriptorType, metadataName, metadataValue, soapMetadataType, node);
                    soapNode = node.ToSoapObject();
                    mappingDb.SubmitChanges();
                }
            });
            return soapNode;
        }

        private static SoapMetadata UpdateMetadata(MappingToolDatabaseDataContext mappingDb, Guid soapNodeId, Guid soapRelationshipId, SoapDescriptorType soapDescriptorType, string metadataName, string metadataValue, SoapMetadataType soapMetadataType, Node node)
        {
            SoapMetadata soapMetadata = new SoapMetadata();
            soapMetadata.MetadataName = metadataName;
            soapMetadata.MetadataValue = metadataValue;
            soapMetadata.MetadataType = soapMetadataType;

            bool existingMetaData = false;
            foreach (Metadata metadatum in node.Metadatas.Where(x => x.MetadataName == metadataName))
            {
                if (soapNodeId != Guid.Empty)
                {
                    if (metadatum.NodeUid != soapNodeId)
                    {
                        continue;
                    }
                }
                if (soapRelationshipId != Guid.Empty)
                {
                    if (metadatum.RelationshipUid != soapRelationshipId)
                    {
                        continue;
                    }
                }
                if (soapDescriptorType != null)
                {
                    if (metadatum.DescriptorTypeUid != soapDescriptorType.Id)
                    {
                        continue;
                    }
                }
                existingMetaData = true;
                if (metadatum.MetadataTypeUid != soapMetadataType.Id)
                {
                    //change in MetadataType
                     metadatum.MetadataType = mappingDb.MetadataTypes.Single(mdt => mdt.MetadataTypeUid == soapMetadataType.Id);
                }
                metadatum.MetadataValue = metadataValue;
                break;
            }
            if (!existingMetaData)
            {
                Metadata metadata = new Metadata();
                metadata.MetadataId = Guid.NewGuid();
                metadata.MetadataTypeUid = soapMetadataType.Id;

                if (soapNodeId != Guid.Empty)
                {
                    metadata.NodeUid = soapNodeId;
                }

                if (soapRelationshipId != Guid.Empty)
                {
                    metadata.RelationshipUid = soapRelationshipId;
                }

                if (soapDescriptorType != null)
                {
                    metadata.DescriptorTypeUid = soapDescriptorType.Id;
                }

                metadata.MetadataName = metadataName;
                metadata.MetadataValue = metadataValue;

                node.Metadatas.Add(metadata);
            }

            return soapMetadata;
        }

        public List<SoapTypeElement> GetAllSoapTypes()
        {
            List<SoapTypeElement> soapTypes = new List<SoapTypeElement>();
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (MappingToolDatabaseDataContext mappingDb = new MappingToolDatabaseDataContext())
                {
                    var metadataTypes = from dbMetadataTypes in mappingDb.MetadataTypes select dbMetadataTypes;
                    var nodeTypes = from dbNodeTypes in mappingDb.NodeTypes select dbNodeTypes;
                    var descriptorTypes = from dbDescriptorTypes in mappingDb.DescriptorTypes select dbDescriptorTypes;
                    var relationshipTypes = from dbRelationshipTypes in mappingDb.RelationshipTypes select dbRelationshipTypes;

                    foreach (var metadataType in metadataTypes)
                    {
                        SoapMetadataType soapMetadataType = new SoapMetadataType();
                        soapMetadataType.Id = metadataType.MetadataTypeUid;
                        soapMetadataType.Name = metadataType.MetadataTypeName;

                        soapTypes.Add(soapMetadataType);
                    }

                    foreach (var nodeType in nodeTypes)
                    {
                        SoapNodeType soapNodeType = new SoapNodeType();
                        soapNodeType.Id = nodeType.NodeTypeUid;
                        soapNodeType.Name = nodeType.NodeTypeName;

                        soapTypes.Add(soapNodeType);
                    }

                    foreach (var descriptorType in descriptorTypes)
                    {
                        SoapDescriptorType soapDescriptorType = new SoapDescriptorType();
                        soapDescriptorType.Id = descriptorType.DescriptorTypeUid;
                        soapDescriptorType.Name = descriptorType.DescriptorTypeName;

                        soapTypes.Add(soapDescriptorType);
                    }

                    foreach (var relationshipType in relationshipTypes)
                    {
                        SoapRelationshipType soapRelationshipType = new SoapRelationshipType();
                        soapRelationshipType.Id = relationshipType.RelationshipTypeUid;
                        soapRelationshipType.Name = relationshipType.RelationshipTypeName;

                        soapTypes.Add(soapRelationshipType);
                    }
                }
            });
            return soapTypes;
        }

        public Dictionary<Guid, SoapNode> GetAllNodes(Guid domainId)
        {
            Dictionary<Guid, SoapNode> nodes = new Dictionary<Guid, SoapNode>();
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (MappingToolDatabaseDataContext mappingDb = new MappingToolDatabaseDataContext())
                {
                    var matchingNodes = from dbNode
                                        in mappingDb.Nodes
                                        where dbNode.DomainUid == domainId
                                        select dbNode;

                    foreach (var node in matchingNodes)
                    {
                        SoapNode soapNode = new SoapNode();
                        soapNode.Domain = domainId;
                        soapNode.Id = node.NodeUid;
                        soapNode.NodeType = (SoapNodeType)SoapTypeFactory.GetSoapType<SoapNodeType>(node.NodeTypeUid.Value);

                        /// Get the relationships for the node
                        /// 

                        foreach (var originatingDescriptor in node.Descriptors)
                        {
                            Relationship relationship = originatingDescriptor.Relationship;

                            var relationshipMetadata = from dbDatum
                                              in relationship.Metadatas
                                                       where !dbDatum.NodeUid.HasValue && !dbDatum.DescriptorTypeUid.HasValue
                                                       select dbDatum;

                            SoapRelationship soapRelationship = new SoapRelationship();
                            soapRelationship.Id = relationship.RelationshipUid;
                            soapRelationship.RelationshipType = (SoapRelationshipType)SoapTypeFactory.GetSoapType<SoapRelationshipType>(relationship.RelationshipTypeUid.Value);

                            foreach (var datum in relationshipMetadata)
                            {
                                SoapMetadata soapDatum = new SoapMetadata();
                                soapDatum.MetadataName = datum.MetadataName;
                                soapDatum.MetadataValue = datum.MetadataValue;

                                if (datum.MetadataTypeUid.HasValue)
                                {
                                    soapDatum.MetadataType = (SoapMetadataType)SoapTypeFactory.GetSoapType<SoapMetadataType>(datum.MetadataTypeUid.Value);
                                }

                                soapRelationship.Metadata.Add(datum.MetadataName, soapDatum);
                            }

                            foreach (var destinationDescriptor in relationship.Descriptors)
                            {
                                if (destinationDescriptor.DescriptorUid == originatingDescriptor.DescriptorUid)
                                {
                                    continue;
                                }

                                if (nodes.ContainsKey(destinationDescriptor.Node.NodeUid))
                                {
                                    continue;
                                }

                                Node childNode = destinationDescriptor.Node;

                                /// Connect the node to the relationships.
                                /// 

                                if (!destinationDescriptor.DescriptorTypeUid.HasValue)
                                {
                                    throw new NotSupportedException("The descriptor doesn't have a type ID.");
                                }

                                SoapDescriptorType soapDescriptorType = (SoapDescriptorType)SoapTypeFactory.GetSoapType<SoapDescriptorType>(destinationDescriptor.DescriptorTypeUid.Value);
                                soapRelationship.Nodes.Add(soapDescriptorType, childNode.NodeUid);
                            }

                            soapNode.Relationships.Add(soapRelationship.Id, soapRelationship);
                        }

                        /// Get the metadata for the node according to the current context.
                        /// 
                        var metadata = from dbDatum in node.Metadatas select dbDatum;

                        foreach (var datum in metadata.OrderBy(x => (x.RelationshipUid != null) ? x.RelationshipUid : Guid.Empty).OrderBy(x => (x.DescriptorTypeUid != null) ? x.DescriptorTypeUid : Guid.Empty))
                        {
                            MetadataContext key = new MetadataContext()
                            {
                                MetadataName = datum.MetadataName,
                                NodeUid = datum.NodeUid,
                                RelationshipUid = datum.RelationshipUid,
                                DescriptorTypeUid = datum.DescriptorTypeUid
                            };

                            SoapMetadata soapDatum = new SoapMetadata();
                            soapDatum.MetadataName = datum.MetadataName;
                            soapDatum.MetadataValue = datum.MetadataValue;

                            if (datum.MetadataTypeUid.HasValue)
                            {
                                soapDatum.MetadataType = (SoapMetadataType)SoapTypeFactory.GetSoapType<SoapMetadataType>(datum.MetadataTypeUid.Value);
                            }

                            soapNode.Metadata.Add(key, soapDatum);
                        }
                        /// End getting metadata.
                        /// 

                        nodes.Add(soapNode.Id, soapNode);
                    }
                }
            });
            return nodes;
        }

        public Dictionary<Guid, SoapNode> GetNodes(Dictionary<Guid, Guid> nodeIds)
        {
            throw new NotImplementedException();
        }

        public Guid GetContainerMapNodeId(Guid domainId, Guid nodeId)
        {
            Guid result = Guid.Empty;
            SPSecurity.RunWithElevatedPrivileges(delegate() 
            {
                using (MappingToolDatabaseDataContext mappingDb = new MappingToolDatabaseDataContext())
                {
                    var mapNodes = from dbNode in mappingDb.Nodes
                                   where dbNode.NodeType.NodeTypeName == "CompendiumMapNode"
                                   && dbNode.DomainUid == domainId
                                   select dbNode;

                    Node containerMap = null;
                    var nodes = mapNodes.Where(d => d.NodeUid == nodeId);
                    if (nodes.Count() == 1)
                    {
                        containerMap = nodes.First();
                        result = containerMap.NodeUid;
                    }
                    else
                    {
                        foreach (Node mapNode in mapNodes)
                        {
                            foreach (Descriptor descriptor in mapNode.Descriptors)
                            {
                                if (descriptor.Relationship.RelationshipType.RelationshipTypeName == "MapContainerRelationship" &&
                                    descriptor.DescriptorType.DescriptorTypeName == "To")
                                {
                                    foreach (Descriptor altDescriptor in descriptor.Relationship.Descriptors.Where(de => de.DescriptorUid != descriptor.DescriptorUid))
                                    {
                                        if (altDescriptor.NodeUid == nodeId)
                                        {
                                            containerMap = mapNode;
                                            break;
                                        }
                                    }
                                    if (containerMap != null)
                                    {
                                        break;
                                    }
                                }
                            }
                            if (containerMap != null)
                            {
                                break;
                            }
                        }
                    }
                    if (containerMap != null)
                    {
                        result = containerMap.NodeUid;
                    }
                }
            });
            return result;
        }

        public Dictionary<string, Guid> GetDomainNodeIds()
        {
            Dictionary<string, Guid> result = null;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (MappingToolDatabaseDataContext mappingDb = new MappingToolDatabaseDataContext())
                {
                    var domainNodes = from dbNode in mappingDb.Nodes
                                      where dbNode.NodeType.NodeTypeName == "DomainNode"
                                      select dbNode;

                    Dictionary<string, Guid> domainNodeIds = new Dictionary<string, Guid>();

                    foreach (var domainNode in domainNodes)
                    {
                        IEnumerable<string> key = from dbMetadata in domainNode.Metadatas
                                                  where dbMetadata.MetadataName == "Name"
                                                  select dbMetadata.MetadataValue;

                        if (!domainNodeIds.ContainsKey(key.First()) && domainNode.DomainUid.HasValue)
                        {
                            domainNodeIds[key.First()] = domainNode.DomainUid.Value;
                        }
                    }

                    result = domainNodeIds;
                }
            });
            return result;
        }

        public Guid GetDomainNodeId(Guid domainId)
        {
            Guid domainNodeId = Guid.Empty;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (MappingToolDatabaseDataContext mappingDb = new MappingToolDatabaseDataContext())
                {
                    var matchingDomainNodes = from dbNode
                                             in mappingDb.Nodes
                                              where dbNode.NodeType.NodeTypeName == "DomainNode" && dbNode.DomainUid == domainId
                                              select dbNode;
                    if (matchingDomainNodes.Count() > 0)
                    {
                        Node domainNode = matchingDomainNodes.First();
                        if (domainNode != null)
                        {
                            domainNodeId = domainNode.NodeUid;
                        }
                    }
                }
            });
            return domainNodeId;
        }

        public RelatedNodesSearchResult GetDomainMapNodes(Guid domainId)
        {
            RelatedNodesSearchResult mapNodesResult = null;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (MappingToolDatabaseDataContext mappingDb = new MappingToolDatabaseDataContext())
                {
                    Guid domainNodesId = GetDomainNodeId(domainId);
                    var mapNodes = (from dbNode in mappingDb.Nodes
                                    where dbNode.NodeType.NodeTypeName == "CompendiumMapNode" && dbNode.DomainUid == domainId
                                    select dbNode);

                    List<Node> mapNodeResults = new List<Node>();
                    foreach (Node mapNode in mapNodes)
                    {
                        foreach (Descriptor descriptor in mapNode.Descriptors)
                        {
                            if (descriptor.Relationship.RelationshipType.RelationshipTypeName == "MapContainerRelationship" &&
                                descriptor.DescriptorType.DescriptorTypeName == "From")
                            {
                                foreach (Descriptor altDescriptor in descriptor.Relationship.Descriptors.Where(de => de.DescriptorUid != descriptor.DescriptorUid))
                                {
                                    if (altDescriptor.NodeUid == domainNodesId && !mapNodeResults.Contains(descriptor.Node))
                                    {
                                        mapNodeResults.Add(descriptor.Node);
                                    }
                                }
                            }
                        }
                    }

                    mapNodesResult = new RelatedNodesSearchResult(domainId, mapNodeResults);
                }
            });
            return mapNodesResult;
        }

        public RelatedNodesSearchResult GetDomainMapNodes(Guid domainId, int page, int pageSize)
        {
            RelatedNodesSearchResult mapNodesResult = null;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (MappingToolDatabaseDataContext mappingDb = new MappingToolDatabaseDataContext())
                {
                    Guid domainNodesId = GetDomainNodeId(domainId);
                    var mapNodes = from dbNode in mappingDb.Nodes
                                    where dbNode.NodeType.NodeTypeName == "CompendiumMapNode" && dbNode.DomainUid == domainId
                                    select dbNode;

                    List<Node> mapNodeResults = new List<Node>();
                    foreach (Node mapNode in mapNodes) 
                    {
                        foreach (Descriptor descriptor in mapNode.Descriptors)
                        {
                            if (descriptor.Relationship.RelationshipType.RelationshipTypeName == "MapContainerRelationship" &&
                                descriptor.DescriptorType.DescriptorTypeName == "From")
                            {
                                foreach (Descriptor altDescriptor in descriptor.Relationship.Descriptors.Where(de => de.DescriptorUid != descriptor.DescriptorUid))
                                {
                                    if (altDescriptor.NodeUid == domainNodesId && !mapNodeResults.Contains(descriptor.Node))
                                    {
                                        mapNodeResults.Add(descriptor.Node);
                                    }
                                }
                            }
                        }
                    }
                    var mapNodes1 = mapNodeResults.Skip((page - 1) * pageSize).Take(pageSize);

                    mapNodesResult = new RelatedNodesSearchResult(domainId, mapNodes1);
                }
            });
            return mapNodesResult;
        }

        public RelatedNodesSearchResult GetMapsNodes(Guid domainId, Guid mapNodeId)
        {
            RelatedNodesSearchResult result = null;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (MappingToolDatabaseDataContext mappingDb = new MappingToolDatabaseDataContext())
                {
                    var matchingMapId = from dbNode
                                        in mappingDb.Nodes
                                        where dbNode.DomainUid == domainId && dbNode.NodeUid == mapNodeId
                                        select dbNode;
                    Dictionary<Guid, SoapNode> nodes = new Dictionary<Guid, SoapNode>();

                    if (matchingMapId.Count() > 0)
                    {
                        var mapNode = matchingMapId.First();

                        IEnumerable<Node> relatedNodes = mapNode.GetConnectedNodes(
                            (Descriptor localDescriptor) => (localDescriptor.DescriptorType.DescriptorTypeName == "To") || (localDescriptor.DescriptorType.DescriptorTypeName == "TransclusionMap"),
                            (Relationship relationship) => relationship.RelationshipType.RelationshipTypeName != "FromToRelationship");

                        foreach (Node relatedNode in relatedNodes)
                        {
                            SoapNode soapNode = relatedNode.ToSoapObject();

                            if (ContainsMetadataKeyName(soapNode, "XPosition") && ContainsMetadataKeyName(soapNode, "YPosition"))
                            {
                                if (soapNode.NodeType.Name != "DomainNode")
                                {
                                    nodes.Add(soapNode.Id, soapNode);
                                }
                            }
                        }
                    }
                    result = new RelatedNodesSearchResult() { Nodes = nodes, SearchedNode = mapNodeId };
                }
            });
            return result;
        }

        public RelatedNodesSearchResult GetRelatedNodes(Guid domainId, Guid nodeId, int depth)
        {
            RelatedNodesSearchResult result = null;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (MappingToolDatabaseDataContext mappingDb = new MappingToolDatabaseDataContext())
                {
                    var matchingFocusedNodes = from dbNode
                                               in mappingDb.Nodes
                                               where dbNode.DomainUid == domainId && dbNode.NodeUid == nodeId
                                               select dbNode;

                    Dictionary<Guid, SoapNode> nodes = new Dictionary<Guid, SoapNode>();

                    if (matchingFocusedNodes.Count() > 0)
                    {
                        var focusedNode = matchingFocusedNodes.First();

                        RecurseThroughLevels(nodes, domainId, focusedNode, focusedNode, depth, null);
                    }

                    result = new RelatedNodesSearchResult() { Nodes = nodes, SearchedNode = nodeId };                   
                }
            });
            return result;
        }

        public RelatedNodesSearchResult GetNodesWithConditions(Guid domainId, Guid nodeId, int maxDepth, List<Condition> conditions)
        {
            RelatedNodesSearchResult result = null;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (MappingToolDatabaseDataContext mappingDb = new MappingToolDatabaseDataContext())
                {
                    var matchingFocusedNodes = from dbNode
                                               in mappingDb.Nodes
                                               where dbNode.DomainUid == domainId && dbNode.NodeUid == nodeId
                                               select dbNode;

                    Dictionary<Guid, SoapNode> nodes = new Dictionary<Guid, SoapNode>();

                    if (matchingFocusedNodes.Count() > 0)
                    {
                        var focusedNode = matchingFocusedNodes.First();

                        RecurseThroughLevels(nodes, domainId, focusedNode, focusedNode, maxDepth, conditions);

                        evaluatedNodes.Clear();
                        excludedNodes.Clear();
                    }

                    result = new RelatedNodesSearchResult() { Nodes = nodes, SearchedNode = nodeId };    
                }
            });
            return result;
        }

        public RelatedNodesSearchResult GetNodesXmlQuery(string graphQueryXml)
        {
            GraphQuery graphQuery = null;

            try
            {
                XmlSerializer graphQueryXmlSerializer = new XmlSerializer(typeof(GraphQuery));
                StringReader graphQueryXmlReader = new StringReader(graphQueryXml);

                graphQuery = graphQueryXmlSerializer.Deserialize(graphQueryXmlReader) as GraphQuery;
            }
            finally
            {
                if (graphQuery == null)
                {
                    LoggingService.WriteTrace(LoggingService.Categories.WcfServices, TraceSeverity.Unexpected, "Graph Query XML is not in the correct format.");
                    throw new NotSupportedException("XML is not in the correct format.");
                }
            }

            RelatedNodesSearchResult nodesSearchResult = null;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (MappingToolDatabaseDataContext mappingDb = new MappingToolDatabaseDataContext())
                {
                    Guid startingNodeId;
                    List<Node> capturedNodes;

                    if (graphQuery.Context.NodeId == Guid.Empty)
                    {
                        var allMapNodes = from dbNode in mappingDb.Nodes where dbNode.DomainUid == graphQuery.Context.DomainId select dbNode;
                        capturedNodes = allMapNodes.ToList();
                        startingNodeId = Guid.Empty;
                    }
                    else
                    {
                        var startingNodes = from dbNode in mappingDb.Nodes where dbNode.DomainUid == graphQuery.Context.DomainId && dbNode.NodeUid == graphQuery.Context.NodeId select dbNode;

                        if (startingNodes.Count() <= 0)
                        {
                            throw new ArgumentOutOfRangeException("The given node context doesn't exist.");
                        }

                        var startingNode = startingNodes.First();
                        startingNodeId = startingNode.NodeUid;

                        capturedNodes = RecurseThroughGraph(graphQuery, startingNode, 1);
                        capturedNodes.Add(startingNode);
                    }

                    IEnumerable<Node> filteredNodes = graphQuery.Filters.ApplyFilters(mappingDb, capturedNodes);

                    Dictionary<Guid, SoapNode> soapNodes = new Dictionary<Guid, SoapNode>();

                    int count = 0;

                    foreach (Node node in filteredNodes)
                    {
                        count++;

                        if (count > graphQuery.Items && graphQuery.Items > 0)
                        {
                            break;
                        }

                        if (!soapNodes.ContainsKey(node.NodeUid))
                        {
                            SoapNode soapNode = node.ToSoapObject();
                            soapNodes.Add(soapNode.Id, soapNode);
                        }
                    }

                    nodesSearchResult = new RelatedNodesSearchResult();
                    nodesSearchResult.Nodes = soapNodes;
                    nodesSearchResult.SearchedNode = startingNodeId;
                }
            });
            return nodesSearchResult;
        }

        private List<Node> RecurseThroughGraph(GraphQuery graphQuery, Node currentNode, int currentDepth)
        {
            if (currentDepth < graphQuery.Depth)
            {
                List<Node> capturedNodes = null;

                foreach (GraphQueryCondition condition in graphQuery.Conditions)
                {
                    List<Node> allowedNodes = new List<Node>(condition.ApplySearchConditions(currentNode));

                    foreach (Node allowedNode in allowedNodes)
                    {
                        capturedNodes = RecurseThroughGraph(graphQuery, allowedNode, ++currentDepth);
                    }

                    if (capturedNodes != null)
                    {
                        capturedNodes = new List<Node>(capturedNodes.Union(allowedNodes));
                    }
                    else
                    {
                        capturedNodes = allowedNodes;
                    }
                }

                return capturedNodes;
            }

            return new List<Node>();
        }

        private void RecurseThroughLevels(Dictionary<Guid, SoapNode> nodes, Guid domainId, Node currentNode, Node initialSearchNode, int depth, List<Condition> conditions)
        {
            SoapNode currentSoapNode;

            if (!nodes.ContainsKey(currentNode.NodeUid))
            {
                currentSoapNode = new SoapNode();
                currentSoapNode.Id = currentNode.NodeUid;
                currentSoapNode.Domain = domainId;

                if (!currentNode.NodeTypeUid.HasValue)
                {
                    throw new NotSupportedException("The node doesn't have a type ID.");
                }

                currentSoapNode.NodeType = (SoapNodeType)SoapTypeFactory.GetSoapType<SoapNodeType>(currentNode.NodeTypeUid.Value);

                if (currentSoapNode.NodeType.Name != "DomainNode")
                {
                    /// Get the metadata for the child node according to the current context.
                    var currentNodeMetadata = from dbDatum in currentNode.Metadatas select dbDatum;

                    foreach (var datum in currentNodeMetadata.OrderBy(x => (x.RelationshipUid != null) ? x.RelationshipUid : Guid.Empty).OrderBy(x => (x.DescriptorTypeUid != null) ? x.DescriptorTypeUid : Guid.Empty))
                    {
                        MetadataContext key = new MetadataContext()
                        {
                            MetadataName = datum.MetadataName,
                            NodeUid = datum.NodeUid,
                            RelationshipUid = datum.RelationshipUid,
                            DescriptorTypeUid = datum.DescriptorTypeUid
                        };

                        SoapMetadata soapDatum = new SoapMetadata();
                        soapDatum.MetadataName = datum.MetadataName;
                        soapDatum.MetadataValue = datum.MetadataValue;

                        if (datum.MetadataTypeUid.HasValue)
                        {
                            soapDatum.MetadataType = (SoapMetadataType)SoapTypeFactory.GetSoapType<SoapMetadataType>(datum.MetadataTypeUid.Value);
                        }

                        currentSoapNode.Metadata[key] = soapDatum;
                    }
                    /// End getting metadata.

                    //let the conditions determine if the node is added or not.
                    if (conditions == null)
                    {
                        nodes.Add(currentSoapNode.Id, currentSoapNode);
                    }
                }
            }
            else
            {
                currentSoapNode = nodes[currentNode.NodeUid];
            }

            foreach (var originatingDescriptor in currentNode.Descriptors)
            {
                Relationship relationship = originatingDescriptor.Relationship;

                var relationshipMetadata = from dbDatum
                                           in relationship.Metadatas
                                           where !dbDatum.NodeUid.HasValue && !dbDatum.DescriptorTypeUid.HasValue
                                           select dbDatum;

                SoapRelationship soapRelationship = new SoapRelationship();
                soapRelationship.Id = relationship.RelationshipUid;
                soapRelationship.RelationshipType = (SoapRelationshipType)SoapTypeFactory.GetSoapType<SoapRelationshipType>(relationship.RelationshipTypeUid.Value);

                foreach (var datum in relationshipMetadata)
                {
                    SoapMetadata soapDatum = new SoapMetadata();
                    soapDatum.MetadataName = datum.MetadataName;
                    soapDatum.MetadataValue = datum.MetadataValue;

                    if (datum.MetadataTypeUid.HasValue)
                    {
                        soapDatum.MetadataType = (SoapMetadataType)SoapTypeFactory.GetSoapType<SoapMetadataType>(datum.MetadataTypeUid.Value);
                    }

                    soapRelationship.Metadata.Add(datum.MetadataName, soapDatum);
                }

                SoapDescriptorType soapOriginatingDescriptorType = (SoapDescriptorType)SoapTypeFactory.GetSoapType<SoapDescriptorType>(originatingDescriptor.DescriptorTypeUid.Value);
                soapRelationship.Nodes.Add(soapOriginatingDescriptorType, currentNode.NodeUid);

                if (!currentSoapNode.Relationships.ContainsKey(soapRelationship.Id))
                {
                    currentSoapNode.Relationships.Add(soapRelationship.Id, soapRelationship);
                }

                foreach (var destinationDescriptor in relationship.Descriptors)
                {
                    if (destinationDescriptor.DescriptorUid == originatingDescriptor.DescriptorUid)
                    {
                        continue; //original end of the relationship
                    }

                    if (nodes.ContainsKey(destinationDescriptor.Node.NodeUid))
                    {
                        continue; //already know about this node
                    }

                    Node childNode = destinationDescriptor.Node;

                    /// Connect the parent node to the relationships.
                    if (!destinationDescriptor.DescriptorTypeUid.HasValue)
                    {
                        throw new NotSupportedException("The descriptor doesn't have a type ID.");
                    }

                    SoapDescriptorType soapDestinationDescriptorType = (SoapDescriptorType)SoapTypeFactory.GetSoapType<SoapDescriptorType>(destinationDescriptor.DescriptorTypeUid.Value);
                    soapRelationship.Nodes.Add(soapDestinationDescriptorType, childNode.NodeUid);

                    SoapNode soapNode = new SoapNode();
                    soapNode.Id = childNode.NodeUid;
                    soapNode.Domain = domainId;

                    if (!soapNode.Relationships.ContainsKey(soapRelationship.Id))
                    {
                        soapNode.Relationships.Add(soapRelationship.Id, soapRelationship);
                    }

                    if (!childNode.NodeTypeUid.HasValue)
                    {
                        throw new NotSupportedException("The node doesn't have a type ID.");
                    }

                    soapNode.NodeType = (SoapNodeType)SoapTypeFactory.GetSoapType<SoapNodeType>(childNode.NodeTypeUid.Value);

                    /// Get the metadata for the child or parent node according to the current context.
                    /// 

                    var metadata = from dbDatum in childNode.Metadatas select dbDatum;

                    foreach (var datum in metadata.OrderBy(x => (x.RelationshipUid != null) ? x.RelationshipUid : Guid.Empty).OrderBy(x => (x.DescriptorTypeUid != null) ? x.DescriptorTypeUid : Guid.Empty))
                    {
                        MetadataContext key = new MetadataContext()
                        {
                            MetadataName = datum.MetadataName,
                            NodeUid = datum.NodeUid,
                            RelationshipUid = datum.RelationshipUid,
                            DescriptorTypeUid = datum.DescriptorTypeUid
                        };

                        SoapMetadata soapDatum = new SoapMetadata();
                        soapDatum.MetadataName = datum.MetadataName;
                        soapDatum.MetadataValue = datum.MetadataValue;

                        if (datum.MetadataTypeUid.HasValue)
                        {
                            soapDatum.MetadataType = (SoapMetadataType)SoapTypeFactory.GetSoapType<SoapMetadataType>(datum.MetadataTypeUid.Value);
                        }

                        soapNode.Metadata[key] = soapDatum;
                    }

                    /// End getting metadata.
                    /// 

                    if (depth >= 0 && conditions == null)
                    {
                        // Add the child or parent node to the collection
                        nodes.Add(destinationDescriptor.Node.NodeUid, soapNode);

                        if (depth > 0) //don't recurse any further
                        {
                            int nextDepthLevel = depth - 1;

                            RecurseThroughLevels(nodes, domainId, destinationDescriptor.Node, initialSearchNode, nextDepthLevel, conditions);
                        }
                    }
                    else if (conditions != null)
                    {
                        //Map equality test code for debugging - doesn't work correctly
                        //MapEqualityMatch mapMatch = new MapEqualityMatch();
                        //mapMatch.Evaluate(initialSearchNode, destinationDescriptor.Node, destinationDescriptor.Relationship, destinationDescriptor);
                        bool nodeAlreadyAdded = false;
                        bool nodeExcludedAlready = false;
                        if (!evaluatedNodes.Contains(destinationDescriptor.Node.NodeUid.ToString()))
                        {
                            foreach (Condition conditionRule in conditions)
                            {
                                //no need to evaluate already excluded nodes against any conditions
                                if (!excludedNodes.Contains(destinationDescriptor.Node.NodeUid.ToString()))
                                {
                                    ConditionResult conditionResult = conditionRule.Evaluate(initialSearchNode, destinationDescriptor.Node, destinationDescriptor.Relationship, destinationDescriptor);
                                    if (!evaluatedNodes.Contains(destinationDescriptor.Node.NodeUid.ToString()))
                                    {
                                        evaluatedNodes.Add(destinationDescriptor.Node.NodeUid.ToString());
                                    }
                                    if (conditionResult.Value)
                                    {
                                        if (!conditionResult.IncludeNode)
                                        {
                                            //if the node isn't to be included and the rule matches remove it
                                            excludedNodes.Add(destinationDescriptor.Node.NodeUid.ToString());
                                            nodeExcludedAlready = true;
                                            if (nodeAlreadyAdded)
                                            {
                                                nodes.Remove(destinationDescriptor.Node.NodeUid);
                                            }
                                        }
                                        if (conditionResult.IncludeNode && !nodeAlreadyAdded && !nodeExcludedAlready && soapNode.NodeType.Name != "DomainNode")
                                        {
                                            //if the node is to be included, hasn't matched an exclusion rule and isn't the domain node add it
                                            nodes.Add(destinationDescriptor.Node.NodeUid, soapNode);
                                            nodeAlreadyAdded = true;
                                        }
                                        if (conditionResult.Action == Action.Continue)
                                        {
                                            //keep looking through the related nodes
                                            RecurseThroughLevels(nodes, domainId, destinationDescriptor.Node, initialSearchNode, -1, conditions);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public SoapNode PasteNodeClone(Guid domainId, Guid copiedNodeId)
        {
            SoapNode result = null;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (MappingToolDatabaseDataContext mappingDb = new MappingToolDatabaseDataContext())
                {
                    var domains = from dbDomain in mappingDb.Domains where dbDomain.DomainUid == domainId select dbDomain;

                    Domain domain;

                    if (domains.Count() > 0)
                    {
                        domain = domains.First();

                        var originalNode = from dbNode
                                           in mappingDb.Nodes
                                           where dbNode.NodeUid == copiedNodeId
                                           && dbNode.DomainUid == domainId
                                           select dbNode;

                        if (originalNode.Count() > 0)
                        {
                            var origNode = originalNode.First();

                            Node node = CreateNode(origNode.NodeTypeUid.Value, origNode.NodeUid.ToString());

                            node.Metadatas = new EntitySet<Metadata>();
                            foreach (Metadata metadata in origNode.Metadatas)
                            {
                                if (!(metadata.MetadataName == "XPosition" || metadata.MetadataName == "YPosition"))
                                {
                                    Metadata metaCopy = new Metadata();
                                    metaCopy.MetadataId = Guid.NewGuid();
                                    metaCopy.MetadataName = metadata.MetadataName;
                                    metaCopy.MetadataType = metadata.MetadataType;
                                    metaCopy.MetadataTypeUid = metadata.MetadataTypeUid;
                                    metaCopy.MetadataValue = metadata.MetadataValue;
                                    metaCopy.NodeUid = node.NodeUid;
                                    metaCopy.Node = node;
                                    metaCopy.DescriptorTypeUid = metadata.DescriptorTypeUid;
                                    metaCopy.RelationshipUid = metadata.RelationshipUid;
                                    metaCopy.Relationship = metadata.Relationship;
                                    node.Metadatas.Add(metaCopy);
                                }
                            }

                            domain.Nodes.Add(node);

                            mappingDb.SubmitChanges();

                            SoapNode soapNode = node.ToSoapObject();
                            result = soapNode;
                        }
                    }
                }
            });
            return result;
        }

        public void InsertNodes(Guid domainId, Guid mapId, string siteUrl, byte[] userToken, Guid webId, Guid listId, int itemId)
        {
            Stopwatch sw = Stopwatch.StartNew();
            string fileName = string.Empty;
            try
            {
                using (SPSite site = new SPSite(siteUrl, new SPUserToken(userToken)))
                {
                    SPWeb web = site.RootWeb;
                    SPList list = web.Lists.GetList(listId, false);
                    SPListItem item = list.GetItemById(itemId);
                    fileName = item.File.Name; //used for logging the completion

                    bool isZipFile = item.File.Name.EndsWith("zip");

                    // The Compendium XML files always have DTD declarations at the top of the page so we need to ignore these otherwise the .NET XML parsers cry cause they can't find what they refer to.
                    XmlReaderSettings settings = new XmlReaderSettings();
                    settings.ProhibitDtd = false;
                    settings.XmlResolver = null;
                    XmlReader xml = null;
                    CompendiumStorage storage = null;
                    Stream fileStream = null;
                    bool isDomainNodeRoot = false;
                    string linkedFilesUrl = string.Empty;

                    try 
                    {
                        Stream sharepointFile = item.File.OpenBinaryStream(); // either the zip file or the XML map export

                        if (isZipFile)
                        {
                            SPFolder linkedFilesFolder = list.RootFolder.SubFolders.Add("Linked Files");
                            list.RootFolder.Update();
                            linkedFilesFolder.SubFolders.Add(mapId.ToString());
                            linkedFilesFolder.Update();

                            try
                            {
                                ZipFile zipFile = Zip.ExtractZipFileFromZipStream(sharepointFile);
                                foreach (ZipEntry zipEntry in zipFile)
                                {
                                    if (zipEntry.Name.StartsWith("Exports/"))
                                    {
                                        fileStream = Zip.GetPartStream(zipFile, zipEntry);
                                        xml = XmlReader.Create(fileStream, settings);
                                    }
                                    else if (zipEntry.Name.StartsWith("Linked Files/"))
                                    {
                                        MemoryStream entryStream = null;
                                        try
                                        {
                                            string zipEntryFileName = zipEntry.Name.Substring(zipEntry.Name.LastIndexOf("/") + 1);
                                            entryStream = (MemoryStream)Zip.GetPartStream(zipFile, zipEntry);
                                            linkedFilesUrl = siteUrl + "/" + linkedFilesFolder.Url + "/" + mapId.ToString() + "/";
                                            linkedFilesFolder.Files.Add(linkedFilesUrl + zipEntryFileName, entryStream.ToArray(), true);
                                            linkedFilesFolder.Update();
                                        }
                                        catch (Exception ex)
                                        {
                                            LoggingService.WriteTrace(LoggingService.Categories.WcfServices, TraceSeverity.Unexpected, "Failed uploading the zip file linked file: {0}", ex.ToString());
                                        }
                                        finally
                                        {
                                            if (entryStream != null)
                                            {
                                                entryStream.Close();
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                LoggingService.WriteTrace(LoggingService.Categories.WcfServices, TraceSeverity.Unexpected, ex.ToString());
                            }
                            finally
                            {
                                sharepointFile.Close();
                            }
                        }
                        else
                        {
                            fileStream = sharepointFile;
                            xml = XmlReader.Create(fileStream, settings);
                        }

                        // Traverse the XML tree till we find the model element. This marks the start of the Compendium data and this is where the CompendiumStorage class expects to load from.
                        while (!(xml.Name == "model" && xml.NodeType == XmlNodeType.Element) && xml.Read())
                        {
                        }

                        // Load the XML tree into the CompendiumStorage class. This is basically a helper class that represents the XML tree as C# objects.
                        storage = new CompendiumStorage();
                        isDomainNodeRoot = storage.Load(xml, linkedFilesUrl);
                    }
                    finally 
                    {
                        if (fileStream != null)
                        {
                            fileStream.Close();
                        }
                    }

                    // Get all the nodes from the CompendiumStorage class.
                    INode[] nodes = storage.GetAllNodes();
                    LoggingService.WriteTrace(LoggingService.Categories.WcfServices, TraceSeverity.High, "Inserting {0} nodes from Compendium Map export {1}", nodes.Length, fileName);

                    Dictionary<string, int> nodeIds = new Dictionary<string, int>();

                    Dictionary<string, SoapNodeType> soapNodeTypes;
                    Dictionary<string, SoapRelationshipType> soapRelTypes;
                    Dictionary<string, SoapMetadataType> soapMetaTypes;
                    Dictionary<string, SoapDescriptorType> soapDescTypes;
                    GetSoapTypes(out soapNodeTypes, out soapRelTypes, out soapMetaTypes, out soapDescTypes);

                    Dictionary<string, SoapNode> addedNodes = new Dictionary<string, SoapNode>();
                    Dictionary<string, SoapRelationship> addedRelationships = new Dictionary<string, SoapRelationship>();

                    AddAllNodes(nodes, soapNodeTypes, soapMetaTypes, domainId, addedNodes);

                    ConnectNodes(nodes, soapRelTypes, soapDescTypes, domainId, addedNodes, addedRelationships, isDomainNodeRoot);

                    SetMetadata(nodes, soapRelTypes, soapMetaTypes, soapDescTypes, domainId, addedNodes, addedRelationships, isDomainNodeRoot);

                    foreach (CompendiumViewRelationship rootViewRelationship in storage.RootViewRelationships)
                    {
                        ConnectedNodesResult result = ConnectToMap(domainId, mapId, addedNodes[rootViewRelationship.FromNode.Id].Id, soapDescTypes, soapRelTypes);
                        UpdateNodeMetadata(domainId, addedNodes[rootViewRelationship.FromNode.Id].Id, result.Relationship.Id,
                                soapDescTypes["From"], "XPosition",
                                rootViewRelationship.XPosition.ToString(), soapMetaTypes["double"]);
                        UpdateNodeMetadata(domainId, addedNodes[rootViewRelationship.FromNode.Id].Id, result.Relationship.Id,
                            soapDescTypes["From"], "YPosition",
                            rootViewRelationship.YPosition.ToString(), soapMetaTypes["double"]);
                    }

                    
                }
            }
            finally
            {
                LoggingService.WriteTrace(LoggingService.Categories.WcfServices, TraceSeverity.High, "Completed Compendium map import {0} in {1:0.000}s", fileName, sw.Elapsed.TotalSeconds);
            }
        }

        /// <summary>
        /// Tests for the existence of the metadata key name in all the metadata for a node. This is useful to work out if a node has any coordinates stored in the metadata for the context it is in.
        /// </summary>
        /// <param name="soapNode">The SoapNode to check</param>
        /// <param name="keyName">The metadata key name</param>
        /// <returns>True if in the current context of the node it has the metadata</returns>
        private bool ContainsMetadataKeyName(SoapNode soapNode, string keyName)
        {
            bool result = false;
            if (soapNode != null && soapNode.Metadata != null)
            {
                foreach (MetadataContext mdc in soapNode.Metadata.Keys)
                {
                    if (mdc.MetadataName == keyName)
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        private void GetSoapTypes(out Dictionary<string, SoapNodeType> soapNodeTypes, out Dictionary<string, SoapRelationshipType> soapRelTypes, out Dictionary<string, SoapMetadataType> soapMetaTypes, out Dictionary<string, SoapDescriptorType> soapDescTypes)
        {
            List<SoapTypeElement> soapTypes = GetAllSoapTypes();
            soapNodeTypes = new Dictionary<string, SoapNodeType>();
            soapRelTypes = new Dictionary<string, SoapRelationshipType>();
            soapMetaTypes = new Dictionary<string, SoapMetadataType>();
            soapDescTypes = new Dictionary<string, SoapDescriptorType>();
            foreach (SoapTypeElement soapType in soapTypes)
            {
                if (soapType is SoapNodeType)
                {
                    soapNodeTypes.Add(soapType.Name, soapType as SoapNodeType);
                }
                else if (soapType is SoapRelationshipType)
                {
                    soapRelTypes.Add(soapType.Name, soapType as SoapRelationshipType);
                }
                else if (soapType is SoapMetadataType)
                {
                    soapMetaTypes.Add(soapType.Name, soapType as SoapMetadataType);
                }
                else if (soapType is SoapDescriptorType)
                {
                    soapDescTypes.Add(soapType.Name, soapType as SoapDescriptorType);
                }
            }
        }

        private ConnectedNodesResult ConnectToMap(Guid domainId, Guid mapId, Guid nodeId, Dictionary<string, SoapDescriptorType> soapDescTypes, Dictionary<string, SoapRelationshipType> soapRelTypes)
        {
            Dictionary<SoapDescriptorType, Guid> nodes = new Dictionary<SoapDescriptorType,Guid>();
            SoapDescriptorType toDesc = soapDescTypes["To"];
            SoapDescriptorType fromDesc = soapDescTypes["From"];
            nodes.Add(toDesc, mapId);
            nodes.Add(fromDesc, nodeId);
            ConnectedNodesResult result = ConnectNodes(domainId, nodes, soapRelTypes["MapContainerRelationship"], null);
            return result;
        }

        public Guid CreateNewMap(Guid domainId, string mapName)
        {
            Guid newMapUid = Guid.Empty;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                Node mapNode = null;
                using (MappingToolDatabaseDataContext mappingDb = new MappingToolDatabaseDataContext())
                {
                    var mapNodeTypes = from dbNodeTypes in mappingDb.NodeTypes where dbNodeTypes.NodeTypeName == "CompendiumMapNode" select dbNodeTypes;
                    var metadataTypes = from dbMetaDataTypes in mappingDb.MetadataTypes where dbMetaDataTypes.MetadataTypeName == "string" select dbMetaDataTypes;
                    var domains = from dbDomain in mappingDb.Domains where dbDomain.DomainUid == domainId select dbDomain;

                    if (mapNodeTypes.Count() == 1 && metadataTypes.Count() == 1 && domains.Count() == 1) //there should only be one
                    {
                        Domain domain = domains.First();
                        NodeType nodeType = mapNodeTypes.First();
                        MetadataType metadataType = metadataTypes.First();

                        SoapMetadataType soapMetadataType = new SoapMetadataType();
                        soapMetadataType.Id = metadataType.MetadataTypeUid;
                        soapMetadataType.Name = metadataType.MetadataTypeName;

                        mapNode = CreateNode(nodeType.NodeTypeUid, mapName);
                        UpdateMetadata(mappingDb, mapNode.NodeUid, Guid.Empty, null, "Name", mapName, soapMetadataType, mapNode);
                        domain.Nodes.Add(mapNode);
                        mappingDb.SubmitChanges();
                    }
                    if (mapNode != null)
                    {
                        newMapUid = mapNode.NodeUid;

                        Dictionary<string, SoapNodeType> soapNodeTypes;
                        Dictionary<string, SoapRelationshipType> soapRelTypes;
                        Dictionary<string, SoapMetadataType> soapMetaTypes;
                        Dictionary<string, SoapDescriptorType> soapDescTypes;
                        GetSoapTypes(out soapNodeTypes, out soapRelTypes, out soapMetaTypes, out soapDescTypes);

                        Guid domainNodeId = GetDomainNodeId(domainId);
                        ConnectToMap(domainId, domainNodeId, newMapUid, soapDescTypes, soapRelTypes);
                    }
                }
            });
            return newMapUid;
        }

        private void SetMetadata(INode[] nodes, Dictionary<string, SoapRelationshipType> soapNodeTypes, 
            Dictionary<string, SoapMetadataType> soapMetaTypes, Dictionary<string, SoapDescriptorType> soapDescTypes, Guid domainUid,
            Dictionary<string, SoapNode> addedNodes, Dictionary<string, SoapRelationship> addedRelationships, bool isDomainNodeRoot) 
        {
            List<Guid> addedTransclusionRelationships = new List<Guid>();

            foreach (INode node in nodes)
            {
                if (node.NodeType != null)
                {
                    foreach (IDescriptor descriptor in node.Descriptors.Where(d => d.DescriptorType.Name == "From"))
                    {
                        if (descriptor.Relationship is CompendiumViewRelationship)
                        {
                            CompendiumViewRelationship view = descriptor.Relationship as CompendiumViewRelationship;
                            if (!view.IsRootView)
                            {
                                UpdateNodeMetadata(domainUid, addedNodes[node.Id].Id, addedRelationships[descriptor.Relationship.Id].Id, 
                                    soapDescTypes[descriptor.DescriptorType.Name], "XPosition",
                                    view.XPosition.ToString(), soapMetaTypes["double"]);
                                UpdateNodeMetadata(domainUid, addedNodes[node.Id].Id, addedRelationships[descriptor.Relationship.Id].Id, 
                                    soapDescTypes[descriptor.DescriptorType.Name], "YPosition",
                                    view.YPosition.ToString(), soapMetaTypes["double"]);
                            }
                        }
                    }    

                    foreach (IDescriptor descriptor in node.Descriptors.Where(d => d.DescriptorType.Name == "TransclusionNode"))
                    {
                        if (descriptor.Relationship is CompendiumTransclusionRelationship)
                        {
                            CompendiumTransclusionRelationship transclusion = descriptor.Relationship as CompendiumTransclusionRelationship;
                            if (transclusion != null)
                            {
                                foreach (IDescriptor tranDesc in transclusion.Descriptors.Where(d => d.DescriptorType.Name == "TransclusionMap"))
                                {
                                    if (!addedTransclusionRelationships.Contains(addedRelationships[tranDesc.Relationship.Id].Id) &&
                                        addedRelationships[tranDesc.Relationship.Id].RelationshipType.Name == "TransclusionRelationship")
                                    {
                                        addedTransclusionRelationships.Add(addedRelationships[tranDesc.Relationship.Id].Id);
                                        UpdateNodeMetadata(domainUid, addedNodes[node.Id].Id, addedRelationships[tranDesc.Relationship.Id].Id,
                                            soapDescTypes[tranDesc.DescriptorType.Name], "XPosition",
                                            transclusion.XPosition.ToString(), soapMetaTypes["double"]);
                                        UpdateNodeMetadata(domainUid, addedNodes[node.Id].Id, addedRelationships[tranDesc.Relationship.Id].Id,
                                            soapDescTypes[tranDesc.DescriptorType.Name], "YPosition",
                                            transclusion.YPosition.ToString(), soapMetaTypes["double"]);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ConnectNodes(INode[] nodes, Dictionary<string, SoapRelationshipType> soapRelTypes,
            Dictionary<string, SoapDescriptorType> soapDescTypes, Guid domainUid, Dictionary<string, SoapNode> addedNodes,
            Dictionary<string, SoapRelationship> addedRelationships, bool isDomainNodeMapNode)
        {
            List<string> relationships = new List<string>();
            Guid domainNodeId = GetDomainNodeId(domainUid);

            List<CompendiumViewRelationship> rootRelationships = new List<CompendiumViewRelationship>();
            foreach (INode node in nodes)
            {
                if (node.NodeType != null)
                {
                    foreach (IDescriptor descriptor in node.Descriptors)
                    {
                        if (relationships.Contains(descriptor.Relationship.Id))
                        {
                            continue;
                        }

                        if (descriptor.Relationship is CompendiumLinkRelationship)
                        {
                            //if it's a link node we'll add it as long as it's only the To descriptor
                            if (descriptor.DescriptorType.Name == "To")
                            {
                                continue;
                            }
                        }
                        else if (descriptor.Relationship is CompendiumViewRelationship)
                        {
                            //it's a map view we'll add it
                            CompendiumViewRelationship relationship = descriptor.Relationship as CompendiumViewRelationship;
                            if (relationship.IsRootView)
                            {
                                continue; //don't add the root view if it's between the domain node and the map
                            }
                        }
                        else if (descriptor.Relationship is CompendiumTransclusionRelationship)
                        {
                            //if it's a transclusion we'll add it as long as it's only the To descriptor
                            if (descriptor.DescriptorType.Name == "To")
                            {
                                continue;
                            }
                        }
                        else
                        {
                            // In this situation the relationship is of an unknown type so lets just skip it.
                            continue;
                        }

                        // Add the relationship ID to the list of relationships.
                        if (!string.IsNullOrEmpty(descriptor.Relationship.Id))
                        {
                            relationships.Add(descriptor.Relationship.Id);
                        }

                        Dictionary<SoapDescriptorType, Guid> relatedNodes = new Dictionary<SoapDescriptorType, Guid>();
                        SoapRelationshipType relationshipType = null;

                        // Go through each of the descriptors on the relationship and assign to the relationship SharePoint list item the "From" and "To" node IDs.
                        foreach (IDescriptor foreignDescriptor in descriptor.Relationship.Descriptors)
                        {
                            if (foreignDescriptor.Node.NodeType != null)
                            {
                                if (descriptor.Relationship is CompendiumViewRelationship)
                                {
                                    relationshipType = soapRelTypes["MapContainerRelationship"];
                                    if (foreignDescriptor.DescriptorType is CompendiumFromDescriptor)
                                    {
                                        relatedNodes.Add(soapDescTypes["From"], addedNodes[foreignDescriptor.Node.Id].Id);
                                    }
                                    else if (foreignDescriptor.DescriptorType is CompendiumToDescriptor)
                                    {
                                        relatedNodes.Add(soapDescTypes["To"], addedNodes[foreignDescriptor.Node.Id].Id);
                                    }
                                }
                                else if (descriptor.Relationship is CompendiumLinkRelationship)
                                {
                                    if (descriptor.Node.NodeType.Name == "CompendiumMapNode") //it will be a From Descriptor Type already
                                    {
                                        relationshipType = soapRelTypes["MapRelationship"];
                                    }
                                    else
                                    {
                                        relationshipType = soapRelTypes["FromToRelationship"];
                                    }
                                    if (foreignDescriptor.DescriptorType is CompendiumFromDescriptor)
                                    {
                                        // If the descriptor is a "From" descriptor then assign the connected node to the "From" column of the relationship SharePoint list item.
                                        relatedNodes.Add(soapDescTypes["From"], addedNodes[foreignDescriptor.Node.Id].Id);
                                    }
                                    else if (foreignDescriptor.DescriptorType is CompendiumToDescriptor)
                                    {
                                        // If the descriptor is a "To" descriptor then assign the connected node to the "From" column of the relationship SharePoint list item.
                                        relatedNodes.Add(soapDescTypes["To"], addedNodes[foreignDescriptor.Node.Id].Id);
                                    }
                                }
                                else if (descriptor.Relationship is CompendiumTransclusionRelationship)
                                {
                                    relationshipType = soapRelTypes["TransclusionRelationship"];

                                    if (foreignDescriptor.DescriptorType is CompendiumFromDescriptor)
                                    {
                                        // If the descriptor is a "From" descriptor then assign the connected node to the "From" column of the relationship SharePoint list item.
                                        relatedNodes.Add(soapDescTypes["From"], addedNodes[foreignDescriptor.Node.Id].Id);
                                    }
                                    else if (foreignDescriptor.DescriptorType is CompendiumToDescriptor)
                                    {
                                        // If the descriptor is a "To" descriptor then assign the connected node to the "From" column of the relationship SharePoint list item.
                                        relatedNodes.Add(soapDescTypes["To"], addedNodes[foreignDescriptor.Node.Id].Id);
                                    }
                                    else if (foreignDescriptor.DescriptorType is CompendiumTransclusionMapDescriptor)
                                    {
                                        relatedNodes.Add(soapDescTypes["TransclusionMap"], addedNodes[foreignDescriptor.Node.Id].Id);
                                    }
                                }
                                else
                                {
                                    throw new NotSupportedException("The provided relationship is not supported by this tool - " + descriptor.Relationship.GetType().Name);
                                }
                            }
                            else
                            {
                                continue; //if it's null node type it's a file linked in the map, ignore it.
                            }
                        }

                        ConnectedNodesResult connectedNodesResult = ConnectNodes(domainUid, relatedNodes, relationshipType, descriptor.Relationship.Id);
                        if (!addedRelationships.ContainsKey(descriptor.Relationship.Id))
                        {
                            addedRelationships.Add(descriptor.Relationship.Id, connectedNodesResult.Relationship);
                        }
                    }
                }
            }
        }

        private void AddAllNodes(INode[] nodes, Dictionary<string, SoapNodeType> soapNodeTypes, 
            Dictionary<string, SoapMetadataType> soapMetaTypes, Guid domainUid, Dictionary<string, SoapNode> addedNodes)
        {
            foreach (INode node in nodes)
            {
                if (node.NodeType != null)
                {
                    if (soapNodeTypes.ContainsKey(node.NodeType.Name))
                    {
                        SoapNodeType nodeType = soapNodeTypes[node.NodeType.Name];

                        //When doing the import you don't want the domain node to be added, you are adding into an existing or newly created domain already.
                        if (nodeType.Name != "DomainNode")
                        {
                            SoapNode soapNode = AddNode(domainUid, nodeType, node.Id);
                            addedNodes.Add(node.Id, soapNode);
                            UpdateNodeMetadata(domainUid, soapNode.Id, Guid.Empty, null, "Name", node.Name, soapMetaTypes["string"]);

                            if (nodeType.Name == "CompendiumReferenceNode")
                            {
                                UpdateNodeMetadata(domainUid, soapNode.Id, Guid.Empty, null, "LinkedFile.Source", node.Attachment, soapMetaTypes["string"]);
                            }
                        }
                    }
                }
            }
        }
    }
}
