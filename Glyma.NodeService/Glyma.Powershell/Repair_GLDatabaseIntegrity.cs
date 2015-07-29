using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using Glyma.Powershell.Base;

namespace Glyma.Powershell
{
    [Cmdlet(VerbsDiagnostic.Repair, "GLDatabaseIntegrity")]
    public class Repair_GLDatabaseIntegrity : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public Model.Map RootMap
        {
            get;
            set;
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            if (RootMap == null)
            {
                WriteWarning("No root map has been provided.");

                return;
            }

            Repair_GLDatabaseIntegrityBase repairGLDatabaseIntegrityBase = new Repair_GLDatabaseIntegrityBase();
            repairGLDatabaseIntegrityBase.RootMap = RootMap;

            repairGLDatabaseIntegrityBase.ExecuteCmdletBase(this);
        }
    }
}
