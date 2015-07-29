using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glyma.HttpHandlers
{
    public partial class QueryMapDescriptor
    {
        public QueryMapDescriptor(QueryMapMultiDepthResult result)
        {
            DescriptorUid = result.DescriptorUid.Value;
            NodeUid = result.NodeUid;
            RelationshipUid = result.RelationshipUid;
            DescriptorTypeUid = result.DescriptorTypeUid;
        }
    }
}
