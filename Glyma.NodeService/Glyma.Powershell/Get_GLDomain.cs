using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using Glyma.Powershell.Base;

namespace Glyma.Powershell
{
    [Cmdlet(VerbsCommon.Get, "GLDomain")]
    public class Get_GLDomain : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public string DatabaseServer
        {
            get;
            set;
        }

        [Parameter(Position = 1, Mandatory = true)]
        public string DatabaseName
        {
            get;
            set;
        }

        [Parameter(Position = 4, Mandatory = false)]
        public Guid DomainId
        {
            get;
            set;
        }

        [Parameter(Position = 3, Mandatory = false)]
        public string DomainName
        {
            get;
            set;
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            Get_GLDomainBase getGLDomainBase = new Get_GLDomainBase();
            getGLDomainBase.DatabaseServer = DatabaseServer;
            getGLDomainBase.DatabaseName = DatabaseName;

            if (DomainId != Guid.Empty)
            {
                getGLDomainBase.DomainId = DomainId;
            }
            else if (!string.IsNullOrEmpty(DomainName))
            {
                getGLDomainBase.DomainName = DomainName;
            }

            getGLDomainBase.ExecuteCmdletBase(this);
        }
    }
}
