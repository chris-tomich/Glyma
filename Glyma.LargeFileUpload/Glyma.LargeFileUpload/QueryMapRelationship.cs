using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glyma.HttpHandlers
{
    public partial class QueryMapRelationship
    {
        public QueryMapRelationship(QueryMapMultiDepthResult result)
        {
            RelationshipUid = result.RelationshipUid.Value;
            DomainUid = result.DomainUid.Value;
            RelationshipTypeUid = result.RelationshipTypeUid;
        }
    }
}
