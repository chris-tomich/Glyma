using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using Glyma.Powershell.Base;

namespace Glyma.Powershell
{
    [Cmdlet(VerbsCommon.Copy, "GLMap")]
    public class Copy_GLMap : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public Model.Map SourceMap
        {
            get;
            set;
        }

        [Parameter(Position = 2, Mandatory = true)]
        public Model.Domain DestinationDomain
        {
            get;
            set;
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            if (SourceMap == null)
            {
                WriteWarning("No source map has been provided.");

                return;
            }

            if (DestinationDomain == null)
            {
                WriteWarning("No destination domain has been provided.");

                return;
            }

            Copy_GLMapBase copyGLMapBase = new Copy_GLMapBase();
            copyGLMapBase.SourceMap = SourceMap;
            copyGLMapBase.DestinationDomain = DestinationDomain;

            copyGLMapBase.ExecuteCmdletBase(this);
        }
    }
}
