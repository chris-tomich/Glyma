using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SilverlightMappingToolBasic.UI.SuperGraph.Controller;

namespace SilverlightMappingToolBasic
{
    public interface ISuperGraphControllerContainer
    {
        SuperGraphController SuperGraphController { get; }
    }
}
