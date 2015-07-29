using Glyma.Powershell.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace Glyma.Powershell
{
    [Cmdlet(VerbsCommon.Add, "ACSRelyingPartyRelayAddress")]
    public class Add_ACSRelyingPartyRelayAddress : PSCmdlet
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

        [Parameter(Position = 5, Mandatory = true)]
        public string RelyingParty
        {
            get;
            set;
        }

        [Parameter(Position = 6, Mandatory = true)]
        public string RelyingPartyRelayAddress
        {
            get;
            set;
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            Add_ACSRelyingPartyRelayAddressBase addACSRelyingPartyRelayAddressBase = new Add_ACSRelyingPartyRelayAddressBase();
            addACSRelyingPartyRelayAddressBase.ServiceIdentityUsernameForManagement = ServiceIdentityUsernameForManagement;
            addACSRelyingPartyRelayAddressBase.ServiceIdentityPasswordForManagement = ServiceIdentityPasswordForManagement;
            addACSRelyingPartyRelayAddressBase.ServiceNamespace = ServiceNamespace;
            addACSRelyingPartyRelayAddressBase.AcsHostName = AcsHostName;
            addACSRelyingPartyRelayAddressBase.AcsManagementServicesRelativeUrl = AcsManagementServicesRelativeUrl;
            addACSRelyingPartyRelayAddressBase.RelyingParty = RelyingParty;
            addACSRelyingPartyRelayAddressBase.RelyingPartyRelayAddress = RelyingPartyRelayAddress;

            addACSRelyingPartyRelayAddressBase.ExecuteCmdletBase(this);
        }
    }
}
