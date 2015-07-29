using System.Collections.Generic;
using SilverlightMappingToolBasic.UI.Extensions.Security;

namespace SilverlightMappingToolBasic.UI.SuperGraph.Controller
{
    public interface ISuperGraphNodeBatchOperations
    {
        void CommitLocations(IEnumerable<ViewModel.Node> nodesToBeCommitted);
        void CommitCollapseStates(IEnumerable<ViewModel.Node> pendingVisibilityNodes, IEnumerable<ViewModel.Node> pendingCollapseStateNodes, PermissionLevel group);
    }
}
