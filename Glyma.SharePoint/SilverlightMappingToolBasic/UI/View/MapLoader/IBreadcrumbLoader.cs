using System;
using System.Collections.Generic;
using TransactionalNodeService.Proxy;

namespace SilverlightMappingToolBasic.UI.View.MapLoader
{
    public interface IBreadcrumbLoader
    {
        void LoadParentBreadcrumb(INode node, int index = 0);
        void LoadParentMaps(List<INode> maps, Guid mapUid);
    }
}
