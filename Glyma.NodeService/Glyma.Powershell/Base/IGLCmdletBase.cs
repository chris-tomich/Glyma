using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;

namespace Glyma.Powershell.Base
{
    public interface IGLCmdletBase
    {
        void ExecuteCmdletBase(PSCmdlet callingCmdlet);
    }
}
