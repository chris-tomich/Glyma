using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransactionalNodeService
{
    public class QueryMap
    {
        private const string GetNodes = @"SELECT * FROM [Nodes] WHERE [DomainUid] = @DomainUid AND [NodeUid] = @NodeUid;
                                          SELECT * FROM [Relationships] INNER JOIN
                                            (SELECT * FROM [Nodes] INNER JOIN [Descriptors] ON [Nodes].[NodeUid] = [Descriptors].NodeUid WHERE [Nodes].[DomainUid] = @DomainUid AND [Nodes].[NodeUid] = @NodeUid) AS MatchingDescriptors
                                            ON [Relationships].[RelationshipUid] = [MatchingDescriptors].[RelationshipUid];
                                          SELECT * FROM [Metadata] WHERE [NodeUid] = @NodeUid;";

        public QueryMap()
        {
        }

        public void LoadNodeDetails()
        {
        }
    }
}