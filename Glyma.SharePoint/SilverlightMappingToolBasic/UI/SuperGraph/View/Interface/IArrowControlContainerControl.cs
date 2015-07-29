using System.Collections.Generic;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Arrow;
using SilverlightMappingToolBasic.UI.SuperGraph.ViewModel;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.Interface
{
    public interface IArrowControlContainerControl
    {
        IEnumerable<Relationship> GetRelationships();

        IEnumerable<ArrowControl> GetArrowControls();
    }
}
