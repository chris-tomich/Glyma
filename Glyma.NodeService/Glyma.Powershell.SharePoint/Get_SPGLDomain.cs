using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using Microsoft.SharePoint;
using Glyma.Powershell.Base;

namespace Glyma.Powershell.SharePoint
{
    [Cmdlet(VerbsCommon.Get, "SPGLDomain")]
    public class Get_SPGLDomain : PSCmdlet
    {
        private IdentityLoader _loader;

        private IdentityLoader Loader
        {
            get
            {
                if (_loader == null)
                {
                    _loader = new IdentityLoader(Identity);
                }

                return _loader;
            }
        }

        [Parameter(Position = 0, Mandatory = true)]
        public object Identity
        {
            get;
            set;
        }

        [Parameter(Position = 2, Mandatory = false)]
        public Guid DomainId
        {
            get;
            set;
        }

        [Parameter(Position = 1, Mandatory = false)]
        public string DomainName
        {
            get;
            set;
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            Get_GLDomainBase getGLDomainBase = new Get_GLDomainBase();

            if (!Loader.PopulateCmdletBase(this, ref getGLDomainBase))
            {
                return;
            }

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
