using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glyma.Powershell.Update.v1_5_0_r1
{
    public class DbIntegrityUtilities
    {
        private Guid MapContainerRelationship = new Guid("4AFF46D7-87BE-48DD-B703-A93E38EF8FFB");

        private class CollisionInfo
        {
            public QueryMapNode CollisionPoint;
            public Guid CurrentRootMap;
            public QueryMapNode CollidingRootMap;
        }

        public DbIntegrityUtilities(QueryMapResultConsumer consumer)
        {
            Consumer = consumer;
        }

        private QueryMapResultConsumer Consumer
        {
            get;
            set;
        }

        public void DetectAndFixMapCollisions(System.Management.Automation.PSCmdlet cmdlet, MappingToolDatabaseDataContext mapDatabaseContext, IEnumerable<Guid> rootMapIds)
        {
            DetectAndFixMapCollisions(cmdlet, mapDatabaseContext, rootMapIds, true, true);
        }

        public void DetectAndFixMapCollisions(System.Management.Automation.PSCmdlet cmdlet, MappingToolDatabaseDataContext mapDatabaseContext, IEnumerable<Guid> rootMapIds, bool fillRootMapId, bool fixCollisions)
        {
            foreach (Guid rootMapId in rootMapIds)
            {
                if (Consumer.Nodes.ContainsKey(rootMapId))
                {
                    QueryMapNode rootMap = Consumer.Nodes[rootMapId];

                    var dbNodes = from qNode in mapDatabaseContext.Nodes
                                  where qNode.NodeUid == rootMap.NodeUid
                                  select qNode;

                    HashSet<Guid> convertedNodes = new HashSet<Guid>();
                    List<CollisionInfo> collisions = new List<CollisionInfo>();

                    if (dbNodes != null && dbNodes.Count() > 0)
                    {
                        var dbNode = dbNodes.First();

                        if (fillRootMapId)
                        {
                            dbNode.RootMapUid = rootMap.NodeUid;
                            mapDatabaseContext.SubmitChanges();
                        }

                        convertedNodes.Add(dbNode.NodeUid);
                    }

                    IEnumerable<QueryMapRelationship> relationships = Consumer.GetRelationships(rootMap, MapContainerRelationship);

                    foreach (QueryMapRelationship relationship in relationships)
                    {
                        IEnumerable<QueryMapNode> nodes = Consumer.GetNodes(relationship, rootMap);

                        foreach (QueryMapNode node in nodes)
                        {
                            if (!convertedNodes.Contains(node.NodeUid))
                            {
                                RecurseNodes(cmdlet, mapDatabaseContext, rootMap, node, convertedNodes, collisions, fillRootMapId);
                            }
                        }
                    }

                    if (fixCollisions)
                    {
                        FixCollisions(cmdlet, mapDatabaseContext, collisions);
                    }
                }
            }
        }

        private void RecurseNodes(System.Management.Automation.PSCmdlet cmdlet, MappingToolDatabaseDataContext mapDatabaseContext, QueryMapNode rootMap, QueryMapNode node, HashSet<Guid> convertedNodes, List<CollisionInfo> collisions, bool fillRootMapId)
        {
            var dbNodes = from qNode in mapDatabaseContext.Nodes
                          where qNode.NodeUid == node.NodeUid
                          select qNode;

            if (dbNodes != null && dbNodes.Count() > 0)
            {
                var dbNode = dbNodes.First();

                convertedNodes.Add(dbNode.NodeUid);

                if (dbNode.RootMapUid != null && dbNode.RootMapUid != rootMap.NodeUid)
                {
                    CollisionInfo collisionInfo = new CollisionInfo();
                    collisionInfo.CollisionPoint = node;
                    collisionInfo.CurrentRootMap = dbNode.RootMapUid.Value;
                    collisionInfo.CollidingRootMap = rootMap;

                    collisions.Add(collisionInfo);
                    cmdlet.WriteWarning("Collision detected.");
                }
                else if (dbNode.RootMapUid == null && fillRootMapId)
                {
                    dbNode.RootMapUid = rootMap.NodeUid;
                    mapDatabaseContext.SubmitChanges();
                }
            }

            IEnumerable<QueryMapRelationship> relationships = Consumer.GetRelationships(node, MapContainerRelationship);

            foreach (QueryMapRelationship relationship in relationships)
            {
                IEnumerable<QueryMapNode> children = Consumer.GetNodes(relationship, node);

                foreach (QueryMapNode childNode in children)
                {
                    if (!convertedNodes.Contains(childNode.NodeUid))
                    {
                        RecurseNodes(cmdlet, mapDatabaseContext, rootMap, childNode, convertedNodes, collisions, fillRootMapId);
                    }
                }
            }
        }

        private void FixCollisions(System.Management.Automation.PSCmdlet cmdlet, MappingToolDatabaseDataContext mapDatabaseContext, List<CollisionInfo> collisions)
        {
            List<QueryMapRelationship> relationshipsToReconnect = new List<QueryMapRelationship>();

            foreach (CollisionInfo collision in collisions)
            {
                cmdlet.WriteWarning("Fixing collision.");
                IEnumerable<QueryMapRelationship> relationships = Consumer.GetRelationships(collision.CollisionPoint);

                foreach (QueryMapRelationship relationship in relationships)
                {
                    IEnumerable<QueryMapNode> nodes = Consumer.GetNodes(relationship, collision.CollisionPoint);

                    foreach (QueryMapNode node in nodes)
                    {
                        var rootMapIds = from qNode in mapDatabaseContext.Nodes
                                         where qNode.NodeUid == node.NodeUid
                                         select qNode.RootMapUid;

                        Guid? rootMapId = rootMapIds.FirstOrDefault();

                        if (rootMapId != null && rootMapId == collision.CollidingRootMap.NodeUid)
                        {
                            relationshipsToReconnect.Add(relationship);
                        }
                    }
                }

                DateTime currentTimestamp = DateTime.Now;

                Node copiedNode = new Node();
                copiedNode.NodeUid = Guid.NewGuid();
                copiedNode.DomainUid = collision.CollisionPoint.DomainUid;
                copiedNode.RootMapUid = collision.CollidingRootMap.NodeUid;
                copiedNode.NodeTypeUid = collision.CollisionPoint.NodeTypeUid;
                copiedNode.NodeOriginalId = collision.CollisionPoint.NodeOriginalId;
                copiedNode.Created = currentTimestamp;
                copiedNode.Modified = currentTimestamp;

                mapDatabaseContext.Nodes.InsertOnSubmit(copiedNode);
                mapDatabaseContext.SubmitChanges();

                var originalMetadata = from qMetadata in Consumer.Metadata
                                       where qMetadata.NodeUid == collision.CollisionPoint.NodeUid
                                       select qMetadata;

                foreach (var originalMetadatum in originalMetadata)
                {
                    Metadata copiedMetadata = new Metadata();
                    copiedMetadata.MetadataId = Guid.NewGuid();
                    copiedMetadata.RootMapUid = collision.CollidingRootMap.NodeUid;
                    copiedMetadata.DomainUid = copiedNode.DomainUid;
                    copiedMetadata.MetadataTypeUid = originalMetadatum.MetadataTypeUid;
                    copiedMetadata.NodeUid = copiedNode.NodeUid;
                    copiedMetadata.RelationshipUid = originalMetadatum.RelationshipUid;
                    copiedMetadata.DescriptorTypeUid = originalMetadatum.DescriptorTypeUid;
                    copiedMetadata.MetadataName = originalMetadatum.MetadataName;
                    copiedMetadata.MetadataValue = originalMetadatum.MetadataValue;
                    copiedMetadata.Created = currentTimestamp;
                    copiedMetadata.Modified = currentTimestamp;

                    mapDatabaseContext.Metadatas.InsertOnSubmit(copiedMetadata);
                }

                mapDatabaseContext.SubmitChanges();

                foreach (QueryMapRelationship relationshipToReconnect in relationshipsToReconnect)
                {
                    var descriptors = from qDescriptor in mapDatabaseContext.Descriptors
                                      where qDescriptor.NodeUid == collision.CollisionPoint.NodeUid && qDescriptor.RelationshipUid == relationshipToReconnect.RelationshipUid
                                      select qDescriptor;

                    if (descriptors != null && descriptors.Count() > 0)
                    {
                        foreach (Descriptor descriptor in descriptors)
                        {
                            descriptor.NodeUid = copiedNode.NodeUid;
                        }
                    }

                    mapDatabaseContext.SubmitChanges();
                }
            }
        }
    }
}
