using System.Collections.Generic;
using Glyma.UtilityService.Export.Common.Model;
using Glyma.UtilityService.Export.IBIS.Common.Model.Glyma;
using TransactionalNodeService.Proxy.Universal;
using TransactionalNodeService.Proxy.Universal.Types;

namespace Glyma.UtilityService.Export.IBIS.Common.Model.Interface
{
    public interface IGlymaNode: IGlymaObject
    {
        NodeType NodeType { get; }


        string Name { get; }

        List<IGlymaNode> Nodes { get; }

        List<IGlymaNode> ChildNodes { get; }

        IRelationship Relationship { get; }

        INode Proxy { get; }


        double XPosition { get; }
        double YPosition { get; }

        IEnumerable<IGlymaNode> GetAllChildNodes();

        int Depth { get; set; }

        NodeVideo NodeVideo { get; }

        NodeDescription NodeDescription { get; }
    }
}
