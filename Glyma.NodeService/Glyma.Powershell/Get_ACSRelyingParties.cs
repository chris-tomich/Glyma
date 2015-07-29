using Glyma.Powershell.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace Glyma.Powershell
{
    [Cmdlet(VerbsCommon.Get, "ACSRelyingParties")]
    public class Get_ACSRelyingParties : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public string ServiceIdentityUsernameForManagement
        {
            get;
            set;
        }

        [Parameter(Position = 1, Mandatory = true)]
        public string ServiceIdentityPasswordForManagement
        {
            get;
            set;
        }

        [Parameter(Position = 2, Mandatory = true)]
        public string ServiceNamespace
        {
            get;
            set;
        }

        [Parameter(Position = 3, Mandatory = true)]
        public string AcsHostName
        {
            get;
            set;
        }

        [Parameter(Position = 4, Mandatory = true)]
        public string AcsManagementServicesRelativeUrl
        {
            get;
            set;
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            Get_ACSRelyingPartiesBase getACSRelyingPartiesBase = new Get_ACSRelyingPartiesBase();
            getACSRelyingPartiesBase.ServiceIdentityUsernameForManagement = ServiceIdentityUsernameForManagement;
            getACSRelyingPartiesBase.ServiceIdentityPasswordForManagement = ServiceIdentityPasswordForManagement;
            getACSRelyingPartiesBase.ServiceNamespace = ServiceNamespace;
            getACSRelyingPartiesBase.AcsHostName = AcsHostName;
            getACSRelyingPartiesBase.AcsManagementServicesRelativeUrl = AcsManagementServicesRelativeUrl;

            getACSRelyingPartiesBase.ExecuteCmdletBase(this);
        }
    }
}
