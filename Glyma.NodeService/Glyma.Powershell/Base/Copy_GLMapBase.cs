using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using System.Security.Principal;

namespace Glyma.Powershell.Base
{
    public class Copy_GLMapBase : IGLCmdletBase
    {
        public Model.Map SourceMap
        {
            get;
            set;
        }

        public Model.Domain DestinationDomain
        {
            get;
            set;
        }

        public void ExecuteCmdletBase(PSCmdlet callingCmdlet)
        {
            Dictionary<Guid, QueryMapNode> originalNodes = new Dictionary<Guid, QueryMapNode>();
            List<QueryMapRelationship> originalRelationships = new List<QueryMapRelationship>();
            List<QueryMapDescriptor> originalDescriptors = new List<QueryMapDescriptor>();
            List<QueryMapMetadata> originalMetadata = new List<QueryMapMetadata>();

            if (DestinationDomain == null || !DestinationDomain.CheckIsValid())
            {
                callingCmdlet.WriteWarning("An invalid destination domain has been provided.");

                return;
            }

            if (SourceMap == null || !SourceMap.CheckIsValid())
            {
                callingCmdlet.WriteWarning("An invalid source map has been provided.");

                return;
            }

            Model.IDatabaseInfo sourceMapDbInfo = SourceMap.Domain;

            using (MappingToolDatabaseDataContext dataContext = new MappingToolDatabaseDataContext(sourceMapDbInfo.ConnectionString))
            {
                dataContext.CommandTimeout = 180;
                var queryMapResultSets = dataContext.QueryMapMultiDepth(SourceMap.Domain.DomainId, SourceMap.NodeId, -1, false);

                var queryMapResultSet = queryMapResultSets.GetResult<QueryMapMultiDepthResult>();

                while (queryMapResultSet != null)
                {
                    foreach (var queryMapResult in queryMapResultSet)
                    {
                        if (queryMapResult.Level != null)
                        {
                            if (queryMapResult.NodeUid.HasValue && queryMapResult.NodeUid != Guid.Empty)
                            {
                                /// Make sure that we aren't copying across a domain node.
                                if (queryMapResult.NodeTypeUid.HasValue && queryMapResult.NodeTypeUid != new Guid("263754C2-2F31-4D21-B9C4-6509E00A5E94"))
                                {
                                    /// The QueryMap procedure returns ALL nodes by following relationships to max depth meaning some nodes are repeated in some of the levels multiple nodes may connect to them.
                                    if (!originalNodes.ContainsKey(queryMapResult.NodeUid.Value))
                                    {
                                        /// TODO: Need to consider copying the NodeOriginalId.
                                        QueryMapNode node = new QueryMapNode();
                                        node.NodeUid = queryMapResult.NodeUid.Value;
                                        node.DomainUid = DestinationDomain.DomainId;
                                        node.NodeTypeUid = queryMapResult.NodeTypeUid;
                                        node.RootMapUid = queryMapResult.RootMapUid;
                                        node.Created = queryMapResult.Created;
                                        node.Modified = queryMapResult.Modified;
                                        node.CreatedBy = queryMapResult.CreatedBy;
                                        node.ModifiedBy = queryMapResult.ModifiedBy;

                                        originalNodes[queryMapResult.NodeUid.Value] = node;
                                    }
                                }
                            }
                        }
                        else if (queryMapResult.MetadataId != null)
                        {
                            if (queryMapResult.MetadataId.HasValue && queryMapResult.MetadataId != Guid.Empty)
                            {
                                QueryMapMetadata metadatum = new QueryMapMetadata();
                                metadatum.MetadataId = queryMapResult.MetadataId.Value;
                                metadatum.NodeUid = queryMapResult.NodeUid;
                                metadatum.RelationshipUid = queryMapResult.RelationshipUid;
                                metadatum.DescriptorTypeUid = queryMapResult.DescriptorTypeUid;
                                metadatum.MetadataTypeUid = queryMapResult.MetadataTypeUid;
                                metadatum.MetadataName = queryMapResult.MetadataName;
                                metadatum.MetadataValue = queryMapResult.MetadataValue;
                                metadatum.DomainUid = queryMapResult.DomainUid;
                                metadatum.RootMapUid = queryMapResult.RootMapUid;
                                metadatum.Created = queryMapResult.Created;
                                metadatum.Modified = queryMapResult.Modified;
                                metadatum.CreatedBy = queryMapResult.CreatedBy;
                                metadatum.ModifiedBy = queryMapResult.ModifiedBy;

                                originalMetadata.Add(metadatum);
                            }
                        }
                        else if (queryMapResult.DescriptorUid != null)
                        {
                            if (queryMapResult.DescriptorUid.HasValue && queryMapResult.DescriptorUid != Guid.Empty)
                            {
                                QueryMapDescriptor descriptor = new QueryMapDescriptor();
                                descriptor.DescriptorUid = queryMapResult.DescriptorUid.Value;
                                descriptor.NodeUid = queryMapResult.NodeUid;
                                descriptor.RelationshipUid = queryMapResult.RelationshipUid;
                                descriptor.DescriptorTypeUid = queryMapResult.DescriptorTypeUid;

                                originalDescriptors.Add(descriptor);
                            }
                        }
                        else
                        {
                            if (queryMapResult.RelationshipUid.HasValue && queryMapResult.RelationshipUid != Guid.Empty)
                            {
                                /// TODO: Need to consider copying the RelationshipOriginalId.
                                QueryMapRelationship relationship = new QueryMapRelationship();
                                relationship.RelationshipUid = queryMapResult.RelationshipUid.Value;
                                relationship.DomainUid = DestinationDomain.DomainId;
                                relationship.RelationshipTypeUid = queryMapResult.RelationshipTypeUid;
                                relationship.RootMapUid = queryMapResult.RootMapUid;
                                relationship.Created = queryMapResult.Created;
                                relationship.Modified = queryMapResult.Modified;
                                relationship.CreatedBy = queryMapResult.CreatedBy;
                                relationship.ModifiedBy = queryMapResult.ModifiedBy;

                                originalRelationships.Add(relationship);
                            }
                        }
                    }

                    queryMapResultSet = queryMapResultSets.GetResult<QueryMapMultiDepthResult>();
                }
            }

            int totalNodes = originalNodes.Count;
            int totalRelationships = originalRelationships.Count;
            int totalMetadata = originalMetadata.Count;
            int totalDescriptors = originalDescriptors.Count;

            Dictionary<Guid, Guid> newNodeIds = new Dictionary<Guid, Guid>();
            Dictionary<Guid, Guid> newRelationshipIds = new Dictionary<Guid, Guid>();

            Model.IDatabaseInfo destinationDomainDbInfo = DestinationDomain;

            /// The following performs the creation of nodes in the new database
            using (MappingToolDatabaseDataContext dataContext = new MappingToolDatabaseDataContext(destinationDomainDbInfo.ConnectionString))
            {
                dataContext.CommandTimeout = 180;
                int count = 0;

                callingCmdlet.WriteVerbose("Processing of nodes starting.");

                DateTime currentTime = DateTime.Now;

                WindowsIdentity currentUserIdentity = WindowsIdentity.GetCurrent();
                string currentUserName = "anonymous";

                if (currentUserIdentity != null)
                {
                    currentUserName = currentUserIdentity.Name;
                }

                Guid newRootMapId = Guid.NewGuid();

                /// If it is the root map node, we want to assign it the pre-determined ID as we've been using this as the RootMapUid for everything;
                QueryMapNode sourceMapNode = originalNodes[SourceMap.NodeId];

                Node newRootMapNode = new Node();
                newRootMapNode.NodeUid = newRootMapId;
                newRootMapNode.DomainUid = DestinationDomain.DomainId;
                newRootMapNode.NodeTypeUid = sourceMapNode.NodeTypeUid;
                newRootMapNode.RootMapUid = newRootMapId;
                newRootMapNode.Created = sourceMapNode.Created;
                newRootMapNode.Modified = currentTime;
                newRootMapNode.CreatedBy = sourceMapNode.CreatedBy;
                newRootMapNode.ModifiedBy = currentUserName;

                newNodeIds[sourceMapNode.NodeUid] = newRootMapNode.NodeUid;

                dataContext.Nodes.InsertOnSubmit(newRootMapNode);

                /// Iterate through all nodes in memory and change their node IDs and root map IDs so that they won't clash with the map from where they were copied.
                foreach (QueryMapNode memoryNode in originalNodes.Values)
                {
                    if (memoryNode.NodeUid == SourceMap.NodeId)
                    {
                        continue;
                    }

                    count++;

                    if (count % 10 == 0)
                    {
                        callingCmdlet.WriteVerbose(count + " / " + totalNodes + " Nodes Completed");
                    }

                    Node node = new Node();
                    node.NodeUid = Guid.NewGuid();
                    node.DomainUid = DestinationDomain.DomainId;
                    node.NodeTypeUid = memoryNode.NodeTypeUid;
                    node.RootMapUid = newRootMapId;
                    node.Created = memoryNode.Created;
                    node.Modified = currentTime;
                    node.CreatedBy = memoryNode.CreatedBy;
                    node.ModifiedBy = currentUserName;

                    newNodeIds[memoryNode.NodeUid] = node.NodeUid;

                    dataContext.Nodes.InsertOnSubmit(node);
                }

                if (!newNodeIds.ContainsKey(SourceMap.NodeId))
                {
                    callingCmdlet.WriteWarning("Unexpected scenario. The source map NodeId can't be found in the list of copied nodes.");

                    return;
                }

                callingCmdlet.WriteVerbose("Committing nodes starting.");

                /// Commit node additions.
                dataContext.SubmitChanges();

                callingCmdlet.WriteVerbose("Committing nodes completed.");

                count = 0;

                callingCmdlet.WriteVerbose("Processing of relationships starting.");

                foreach (QueryMapRelationship memoryRelationship in originalRelationships)
                {
                    count++;

                    if (count % 10 == 0)
                    {
                        callingCmdlet.WriteVerbose(count + " / " + totalRelationships + " Relationships Completed");
                    }

                    Relationship relationship = new Relationship();
                    relationship.RelationshipUid = Guid.NewGuid();
                    relationship.DomainUid = DestinationDomain.DomainId;
                    relationship.RelationshipTypeUid = memoryRelationship.RelationshipTypeUid;
                    relationship.RootMapUid = newRootMapId;
                    relationship.Created = memoryRelationship.Created;
                    relationship.Modified = currentTime;
                    relationship.CreatedBy = memoryRelationship.CreatedBy;
                    relationship.ModifiedBy = currentUserName;

                    newRelationshipIds[memoryRelationship.RelationshipUid] = relationship.RelationshipUid;

                    dataContext.Relationships.InsertOnSubmit(relationship);
                }

                callingCmdlet.WriteVerbose("Committing relationships starting.");

                /// Commit relationship additions.
                dataContext.SubmitChanges();

                callingCmdlet.WriteVerbose("Committing relationships completed.");

                count = 0;

                callingCmdlet.WriteVerbose("Processing of descriptors starting.");

                foreach (QueryMapDescriptor memoryDescriptor in originalDescriptors)
                {
                    count++;

                    if (count % 10 == 0)
                    {
                        callingCmdlet.WriteVerbose(count + " / " + totalDescriptors + " Descriptors Completed");
                    }

                    if (newNodeIds.ContainsKey(memoryDescriptor.NodeUid.Value) && newRelationshipIds.ContainsKey(memoryDescriptor.RelationshipUid.Value))
                    {
                        Descriptor descriptor = new Descriptor();
                        descriptor.DescriptorUid = Guid.NewGuid();
                        descriptor.NodeUid = newNodeIds[memoryDescriptor.NodeUid.Value];
                        descriptor.RelationshipUid = newRelationshipIds[memoryDescriptor.RelationshipUid.Value];
                        descriptor.DescriptorTypeUid = memoryDescriptor.DescriptorTypeUid;

                        dataContext.Descriptors.InsertOnSubmit(descriptor);
                    }
                }

                callingCmdlet.WriteVerbose("Committing descriptors starting.");

                /// Commit descriptor additions.
                dataContext.SubmitChanges();

                callingCmdlet.WriteVerbose("Committing descriptors completed.");

                count = 0;

                callingCmdlet.WriteVerbose("Processing of metadata starting.");

                foreach (QueryMapMetadata memoryMetadatum in originalMetadata)
                {
                    count++;

                    if (count % 10 == 0)
                    {
                        callingCmdlet.WriteVerbose(count + " / " + totalMetadata + " Metadata Completed");
                    }

                    if (memoryMetadatum.NodeUid == null || newNodeIds.ContainsKey(memoryMetadatum.NodeUid.Value))
                    {
                        if (memoryMetadatum.RelationshipUid == null || newRelationshipIds.ContainsKey(memoryMetadatum.RelationshipUid.Value))
                        {
                            Metadata metadatum = new Metadata();
                            metadatum.MetadataId = Guid.NewGuid();

                            if (memoryMetadatum.NodeUid != null)
                            {
                                metadatum.NodeUid = newNodeIds[memoryMetadatum.NodeUid.Value];
                            }
                            else
                            {
                                metadatum.NodeUid = null;
                            }

                            if (memoryMetadatum.RelationshipUid != null)
                            {
                                metadatum.RelationshipUid = newRelationshipIds[memoryMetadatum.RelationshipUid.Value];
                            }
                            else
                            {
                                metadatum.RelationshipUid = null;
                            }

                            metadatum.DescriptorTypeUid = memoryMetadatum.DescriptorTypeUid;
                            metadatum.MetadataTypeUid = memoryMetadatum.MetadataTypeUid;
                            metadatum.MetadataName = memoryMetadatum.MetadataName;
                            metadatum.MetadataValue = memoryMetadatum.MetadataValue;
                            metadatum.DomainUid = DestinationDomain.DomainId;
                            metadatum.RootMapUid = newRootMapId;
                            metadatum.Created = memoryMetadatum.Created;
                            metadatum.Modified = currentTime;
                            metadatum.CreatedBy = memoryMetadatum.CreatedBy;
                            metadatum.ModifiedBy = currentUserName;

                            dataContext.Metadatas.InsertOnSubmit(metadatum);
                        }
                    }
                }

                callingCmdlet.WriteVerbose("Committing metadata starting.");

                /// Commit metadata additions.
                dataContext.SubmitChanges();

                callingCmdlet.WriteVerbose("Committing metadata completed.");

                callingCmdlet.WriteVerbose("Connecting map to domain.");

                Relationship domainNodeMapRelationship = new Relationship();
                domainNodeMapRelationship.RelationshipUid = Guid.NewGuid();
                domainNodeMapRelationship.DomainUid = DestinationDomain.DomainId;
                domainNodeMapRelationship.RelationshipTypeUid = new Guid("4AFF46D7-87BE-48DD-B703-A93E38EF8FFB");
                domainNodeMapRelationship.RootMapUid = newRootMapId;
                domainNodeMapRelationship.Created = currentTime;
                domainNodeMapRelationship.Modified = currentTime;
                domainNodeMapRelationship.CreatedBy = currentUserName;
                domainNodeMapRelationship.ModifiedBy = currentUserName;

                Descriptor domainNodeMapFromDescriptor = new Descriptor();
                domainNodeMapFromDescriptor.DescriptorUid = Guid.NewGuid();
                domainNodeMapFromDescriptor.DescriptorTypeUid = new Guid("96DA1782-058C-4F9B-BB1A-31B048F8C75A");
                domainNodeMapFromDescriptor.NodeUid = newNodeIds[SourceMap.NodeId];
                domainNodeMapFromDescriptor.RelationshipUid = domainNodeMapRelationship.RelationshipUid;

                Descriptor domainNodeMapToDescriptor = new Descriptor();
                domainNodeMapToDescriptor.DescriptorUid = Guid.NewGuid();
                domainNodeMapToDescriptor.DescriptorTypeUid = new Guid("07C91D35-4DAC-431B-966B-64C924B8CDAB");
                domainNodeMapToDescriptor.NodeUid = DestinationDomain.NodeId;
                domainNodeMapToDescriptor.RelationshipUid = domainNodeMapRelationship.RelationshipUid;

                dataContext.Relationships.InsertOnSubmit(domainNodeMapRelationship);
                dataContext.Descriptors.InsertOnSubmit(domainNodeMapFromDescriptor);
                dataContext.Descriptors.InsertOnSubmit(domainNodeMapToDescriptor);

                dataContext.SubmitChanges();

                callingCmdlet.WriteVerbose("Completed connecting map to domain.");

                callingCmdlet.WriteVerbose("Copy completed.");
            }
        }
    }
}
