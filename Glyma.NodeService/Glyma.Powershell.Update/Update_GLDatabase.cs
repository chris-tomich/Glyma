using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using Glyma.Powershell.Base;

namespace Glyma.Powershell.Update
{
    [Cmdlet(VerbsData.Update, "GLDatabase")]
    public class Update_GLDatabase : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public string Version
        {
            get;
            set;
        }

        [Parameter(Position = 1, Mandatory = true)]
        public string DatabaseServer
        {
            get;
            set;
        }

        [Parameter(Position = 2, Mandatory = true)]
        public string MapDatabaseName
        {
            get;
            set;
        }

        [Parameter(Position = 3, Mandatory = true)]
        public string TransactionDatabaseName
        {
            get;
            set;
        }

        [Parameter(Position = 4, Mandatory = false)]
        public string TransactionDatabaseServer
        {
            get;
            set;
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            IUpdateGLDatabase cmdletBase = null;

            switch (Version)
            {
                case "v1.5.0r1":
                    cmdletBase = new v1_5_0_r1.Update_v1_5_0_r1Base();
                    break;

                case "v1.5.0r2":
                    cmdletBase = new v1_5_0_r2.Update_v1_5_0_r2Base();
                    break;

                case "v1.5.0r3":
                    cmdletBase = new v1_5_0_r3.Update_v1_5_0_r3Base();
                    break;

                case "v1.5.0r4":
                    cmdletBase = new v1_5_0_r4.Update_v1_5_0_r4Base();
                    break;

                default:
                    WriteWarning("Unknown version number.");
                    return;
            }

            cmdletBase.DatabaseServer = DatabaseServer;
            cmdletBase.MapDatabaseName = MapDatabaseName;
            cmdletBase.TransactionDatabaseServer = TransactionDatabaseServer;
            cmdletBase.TransactionDatabaseName = TransactionDatabaseName;

            cmdletBase.ExecuteCmdletBase(this);
        }
    }
}
