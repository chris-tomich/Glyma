using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;

namespace Glyma.Powershell.Base
{
    public class Repair_GLDatabaseIntegrityBase : IGLCmdletBase
    {
        Guid ToDescriptorType = new Guid("07C91D35-4DAC-431B-966B-64C924B8CDAB");
        Guid FromDescriptorType = new Guid("96DA1782-058C-4F9B-BB1A-31B048F8C75A");
        Guid MapContainerRelationshipType = new Guid("4AFF46D7-87BE-48DD-B703-A93E38EF8FFB");
        Guid TransclusionFromToRelationshipType = new Guid("9B64675F-D27B-42F1-BDAA-38D4697CC27A");
        Guid MapNodeType = new Guid("B8C354CB-C7D0-4982-9A0F-6C4368FAB749");

        public Model.Map RootMap
        {
            get;
            set;
        }

        private void RecusivelyFindValidMaps(ref HashSet<Guid> validMaps, Node mapNode)
        {
            var childRelationships = from dbDescriptor in mapNode.Descriptors
                                     where dbDescriptor.DescriptorTypeUid == ToDescriptorType && dbDescriptor.Relationship.RelationshipTypeUid == MapContainerRelationshipType
                                     select dbDescriptor.Relationship;

            foreach (var childRelationship in childRelationships)
            {
                var childMapNodes = from dbDescriptor in childRelationship.Descriptors
                                    where dbDescriptor.DescriptorTypeUid == FromDescriptorType && dbDescriptor.Node.NodeTypeUid == MapNodeType
                                    select dbDescriptor.Node;

                foreach (var childMapNode in childMapNodes)
                {
                    if (validMaps.Contains(childMapNode.NodeUid))
                    {
                        continue;
                    }
                    else
                    {
                        validMaps.Add(childMapNode.NodeUid);

                        RecusivelyFindValidMaps(ref validMaps, childMapNode);
                    }
                }
            }
        }

        public void ExecuteCmdletBase(PSCmdlet callingCmdlet)
        {
            HashSet<Guid> validMaps = new HashSet<Guid>();
            HashSet<Guid> orphanedNodes = new HashSet<Guid>();
            HashSet<Guid> orphanedRelationships = new HashSet<Guid>();

            if (RootMap == null || !RootMap.CheckIsValid())
            {
                callingCmdlet.WriteWarning("An invalid source map has been provided.");

                return;
            }

            Model.IDatabaseInfo mapDbInfo = RootMap.Domain;

            using (MappingToolDatabaseDataContext dataContext = new MappingToolDatabaseDataContext(mapDbInfo.ConnectionString))
            {
                dataContext.CommandTimeout = 180;

                var dbRootMapNodes = from dbNode in dataContext.Nodes
                                     where dbNode.NodeUid == RootMap.NodeId
                                     select dbNode;

                var dbRootMapNode = dbRootMapNodes.FirstOrDefault();

                if (dbRootMapNode == null)
                {
                    throw new KeyNotFoundException("The specified root map doesn't exist.");
                }

                if (dbRootMapNode.NodeUid != dbRootMapNode.RootMapUid)
                {
                    throw new NotSupportedException("The provided root map isn't a root map.");
                }

                /// We need to find all the valid children maps, starting at the root map. We'll do this recursively.
                #region Find valid maps
                validMaps.Add(dbRootMapNode.NodeUid);

                RecusivelyFindValidMaps(ref validMaps, dbRootMapNode);
                #endregion

                #region Find all nodes that aren't part of a valid map
                var allNodesInMap = from dbNode in dataContext.Nodes
                                    where dbNode.RootMapUid == RootMap.NodeId
                                    select dbNode;

                foreach (var node in allNodesInMap)
                {
                    if (validMaps.Contains(node.NodeUid))
                    {
                        continue;
                    }

                    bool isNodeValid = CheckIsValid(ref validMaps, node);

                    if (!isNodeValid)
                    {
                        /// If the orphaned nodes list doesn't contain the node, then add the node to the list.
                        if (!orphanedNodes.Contains(node.NodeUid))
                        {
                            orphanedNodes.Add(node.NodeUid);
                        }

                        /// Find all the orphaned relationships.
                        FindOrphanedRelationships(ref orphanedRelationships, node);
                    }
                }
                #endregion

                int orphanedNodesCount = 0;
                int orphanedMetadataCount = 0;
                int orphanedDescriptorsCount = 0;
                int orphanedRelationshipsCount = 0;

                foreach (Guid nodeId in orphanedNodes)
                {
                    var orphanedDescriptors = from dbDescriptor in dataContext.Descriptors
                                              where dbDescriptor.NodeUid == nodeId
                                              select dbDescriptor;

                    foreach (var orphanedDescriptor in orphanedDescriptors)
                    {
                        dataContext.Descriptors.DeleteOnSubmit(orphanedDescriptor);
                        orphanedDescriptorsCount++;
                    }

                    var orphanedMetadata = from dbMetadata in dataContext.Metadatas
                                           where dbMetadata.NodeUid == nodeId
                                           select dbMetadata;

                    foreach (var orphanedMetadatum in orphanedMetadata)
                    {
                        dataContext.Metadatas.DeleteOnSubmit(orphanedMetadatum);
                        orphanedMetadataCount++;
                    }
                }

                dataContext.SubmitChanges();

                foreach (Guid relationshipId in orphanedRelationships)
                {
                    var orphanedDescriptors = from dbDescriptor in dataContext.Descriptors
                                              where dbDescriptor.RelationshipUid == relationshipId
                                              select dbDescriptor;

                    foreach (var orphanedDescriptor in orphanedDescriptors)
                    {
                        dataContext.Descriptors.DeleteOnSubmit(orphanedDescriptor);
                        orphanedDescriptorsCount++;
                    }

                    var orphanedMetadata = from dbMetadata in dataContext.Metadatas
                                           where dbMetadata.RelationshipUid == relationshipId
                                           select dbMetadata;

                    foreach (var orphanedMetadatum in orphanedMetadata)
                    {
                        dataContext.Metadatas.DeleteOnSubmit(orphanedMetadatum);
                        orphanedMetadataCount++;
                    }
                }

                dataContext.SubmitChanges();

                foreach (Guid nodeId in orphanedNodes)
                {
                    var orphanedDbNodes = from dbNode in dataContext.Nodes
                                          where dbNode.NodeUid == nodeId
                                          select dbNode;

                    foreach (var orphanedNode in orphanedDbNodes)
                    {
                        dataContext.Nodes.DeleteOnSubmit(orphanedNode);
                        orphanedNodesCount++;
                    }
                }

                dataContext.SubmitChanges();

                foreach (Guid relationshipId in orphanedRelationships)
                {
                    var orphanedDbRelationships = from dbRelationship in dataContext.Relationships
                                                  where dbRelationship.RelationshipUid == relationshipId
                                                  select dbRelationship;

                    foreach (var orphanedRelationship in orphanedDbRelationships)
                    {
                        dataContext.Relationships.DeleteOnSubmit(orphanedRelationship);
                        orphanedRelationshipsCount++;
                    }
                }

                dataContext.SubmitChanges();

                callingCmdlet.WriteVerbose(string.Format("Deleted {0} orphaned node/s.", orphanedNodesCount));
                callingCmdlet.WriteVerbose(string.Format("Deleted {0} orphaned relationship/s.", orphanedRelationshipsCount));
                callingCmdlet.WriteVerbose(string.Format("Deleted {0} orphaned metadata/s.", orphanedMetadataCount));
                callingCmdlet.WriteVerbose(string.Format("Deleted {0} orphaned descriptor/s.", orphanedDescriptorsCount));
            }
        }

        private void FindOrphanedRelationships(ref HashSet<Guid> orphanedRelationships, Node orphanedNode)
        {
            var relationships = from dbDescriptor in orphanedNode.Descriptors
                                select dbDescriptor.Relationship;

            foreach (var relationship in relationships)
            {
                if (orphanedRelationships.Contains(relationship.RelationshipUid))
                {
                    continue;
                }
                else
                {
                    orphanedRelationships.Add(relationship.RelationshipUid);
                }
            }
        }

        private bool CheckIsValid(ref HashSet<Guid> validMaps, Node node)
        {
            var parentRelationships = from dbDescriptor in node.Descriptors
                                      where dbDescriptor.DescriptorTypeUid == FromDescriptorType && dbDescriptor.Relationship.RelationshipTypeUid != TransclusionFromToRelationshipType
                                      select dbDescriptor.Relationship;

            foreach (var parentRelationship in parentRelationships)
            {
                var parentNodes = from dbDescriptor in parentRelationship.Descriptors
                                  where dbDescriptor.DescriptorTypeUid == ToDescriptorType && dbDescriptor.Node.NodeTypeUid == MapNodeType
                                  select dbDescriptor.Node;

                foreach (var parentNode in parentNodes)
                {
                    if (validMaps.Contains(parentNode.NodeUid))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
