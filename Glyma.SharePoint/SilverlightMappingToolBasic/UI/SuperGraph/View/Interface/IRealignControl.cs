using SilverlightMappingToolBasic.UI.Extensions.CookieManagement;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.Interface
{
    public interface IRealignControl : INodeControlContainerControl, IArrowControlContainerControl
    {
        MapInformation MapInformation { get; }
    }
}
