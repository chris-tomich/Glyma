using System;
using System.Collections.Generic;
using TransactionalNodeService.Proxy.Universal;
using TransactionalNodeService.Proxy.Universal.Collections.NodeContainers;

namespace Glyma.UtilityService.Export.IBIS.Common.Model.Interface
{
    public interface IGlymaRelationship: IGlymaObject
    {
        IRelationship ProxyRelationship { get; }

        INode NodeFrom { get; }

        INode NodeTo { get; }

        IEnumerable<NodeTuple> ConnectedNodes { get; }

        Guid From { get; }

        Guid To { get; }

        string Created { get; }

        string LastModified { get; }
    }
}
