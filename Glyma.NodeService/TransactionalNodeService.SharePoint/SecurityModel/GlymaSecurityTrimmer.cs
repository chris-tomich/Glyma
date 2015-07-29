using Glyma.Security.Model;
using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using TransactionalNodeService.Common;
using TransactionalNodeService.Common.Model;

namespace TransactionalNodeService.SharePoint.SecurityModel
{
    public class GlymaSecurityTrimmer
    {
        public GlymaSecurityTrimmer(SPWeb web, IGlymaSession glymaSession)
        {
            Web = web;
            GlymaSession = glymaSession;

            GlymaUser = new SPGlymaUser(Web, GlymaSession);
        }

        private SPWeb Web
        {
            get;
            set;
        }

        private IGlymaSession GlymaSession
        {
            get;
            set;
        }

        private SPGlymaUser GlymaUser
        {
            get;
            set;
        }

        private SqlCommand BuildFindRootMapIdCommand(SqlConnection connection, Guid domainId, Guid nodeId)
        {
            SqlCommand findRootMapIdCommand = new SqlCommand("SELECT [RootMapUid] FROM [Nodes] WHERE [Nodes].[DomainUid] = @DomainUid AND [Nodes].[NodeUid] = @NodeUid", connection);

            findRootMapIdCommand.Parameters.Add(new SqlParameter("@DomainUid", domainId));
            findRootMapIdCommand.Parameters.Add(new SqlParameter("@NodeUid", nodeId));

            return findRootMapIdCommand;
        }

        public bool CheckRootMapAuthorisationBaseOnNode(Guid domainId, Guid nodeId, params IRight[] requiredRights)
        {
            using (IDbConnectionAbstraction connectionAbstraction = GlymaSession.ConnectionFactory.CreateMapDbConnection())
            {
                SqlCommand findRootMapIdCommand = BuildFindRootMapIdCommand(connectionAbstraction.Connection, domainId, nodeId);

                connectionAbstraction.Open();
                object rootMapIdQueryResult = findRootMapIdCommand.ExecuteScalar();
                connectionAbstraction.Close();

                if (rootMapIdQueryResult != null && rootMapIdQueryResult != DBNull.Value && rootMapIdQueryResult is Guid)
                {
                    Guid rootMapId = (Guid)rootMapIdQueryResult;

                    return GlymaUser.IsAuthorised(domainId, rootMapId, requiredRights);
                }
                else
                {
                    return GlymaUser.IsAuthorised(domainId, requiredRights);
                }
            }
        }

        public QueryResponse TrimResponse(QueryResponse response, params IRight[] requiredRights)
        {
            /// TODO: The performance on this can be improved.

            HashSet<Guid> rootMapUids = new HashSet<Guid>();

            foreach (Node node in response.Nodes.Values)
            {
                if (node.RootMapUid != null)
                {
                    if (!rootMapUids.Contains(node.RootMapUid.Value))
                    {
                        rootMapUids.Add(node.RootMapUid.Value);
                    }
                }
            }

            IEnumerable<Guid> securityTrimmedRootMapUids = GlymaUser.IsAuthorised(response.Domain.DomainUid, rootMapUids, requiredRights);

            HashSet<Guid> securityTrimmedRootMapUidsFinder = new HashSet<Guid>(securityTrimmedRootMapUids);

            HashSet<Guid> nodesToRemove = new HashSet<Guid>();
            HashSet<Guid> relationshipsToRemove = new HashSet<Guid>();
            HashSet<Guid> metadataToRemove = new HashSet<Guid>();
            HashSet<Guid> descriptorsToRemove = new HashSet<Guid>();

            foreach (Node node in response.Nodes.Values)
            {
                if (node.RootMapUid != null && !securityTrimmedRootMapUidsFinder.Contains(node.RootMapUid.Value))
                {
                    /// This node has been security trimmed.
                    nodesToRemove.Add(node.NodeUid);
                }
            }

            foreach (Relationship relationship in response.Relationships.Values)
            {
                if (relationship.RootMapUid != null && !securityTrimmedRootMapUidsFinder.Contains(relationship.RootMapUid.Value))
                {
                    /// This relationship has been security trimmed.
                    relationshipsToRemove.Add(relationship.RelationshipUid);
                }
            }

            foreach (Metadata metadata in response.Metadata.Values)
            {
                if (metadata.RootMapUid != null && !securityTrimmedRootMapUidsFinder.Contains(metadata.RootMapUid.Value))
                {
                    /// This metadata has been security trimmed.
                    metadataToRemove.Add(metadata.MetadataId);
                }
            }

            foreach (Descriptor descriptor in response.Descriptors.Values)
            {
                if (nodesToRemove.Contains(descriptor.NodeUid) || relationshipsToRemove.Contains(descriptor.RelationshipUid))
                {
                    /// This descriptor has been security trimmed as either it's Node or Relationship are to be removed.
                    descriptorsToRemove.Add(descriptor.DescriptorUid);
                }
            }

            foreach (Guid nodeToRemove in nodesToRemove)
            {
                response.Nodes.Remove(nodeToRemove);
            }

            foreach (Guid relationshipToRemove in relationshipsToRemove)
            {
                response.Relationships.Remove(relationshipToRemove);
            }

            foreach (Guid metadatumToRemove in metadataToRemove)
            {
                response.Metadata.Remove(metadatumToRemove);
            }

            foreach (Guid descriptorToRemove in descriptorsToRemove)
            {
                response.Descriptors.Remove(descriptorToRemove);
            }

            return response;
        }

        public Dictionary<Guid, QueryResponse> TrimResponse(Dictionary<Guid, QueryResponse> response, params IRight[] requiredRights)
        {
            IEnumerable<Guid> authorisedDomainUids = GlymaUser.IsAuthorised(response.Keys, requiredRights);
            Dictionary<Guid, QueryResponse> securityTrimmedDomains = new Dictionary<Guid, QueryResponse>();

            foreach (Guid authorisedDomainUid in authorisedDomainUids)
            {
                if (response.ContainsKey(authorisedDomainUid))
                {
                    securityTrimmedDomains[authorisedDomainUid] = response[authorisedDomainUid];
                }
            }

            return securityTrimmedDomains;
        }
    }
}
