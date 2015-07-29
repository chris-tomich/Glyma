using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glyma.HttpHandlers
{
    public partial class QueryMapMetadata
    {
        public QueryMapMetadata(QueryMapMultiDepthResult result)
        {
            MetadataId = result.MetadataId.Value;
            NodeUid = result.NodeUid;
            RelationshipUid = result.RelationshipUid;
            DescriptorTypeUid = result.DescriptorTypeUid;
            MetadataTypeUid = result.MetadataTypeUid;
            MetadataName = result.MetadataName;
            MetadataValue = result.MetadataValue;
        }
    }
}
