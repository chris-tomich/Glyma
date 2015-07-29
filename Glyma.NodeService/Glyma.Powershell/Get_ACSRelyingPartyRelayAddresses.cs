using Glyma.Powershell.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace Glyma.Powershell
{
    [Cmdlet(VerbsCommon.Get, "ACSRelyingPartyRelayAddresses")]
    public class Get_ACSRelyingPartyRelayAddresses : PSCmdlet
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

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            Get_ACSRelyingPartyRelayAddressesBase getACSRelyingPartyRelayAddressesBase = new Get_ACSRelyingPartyRelayAddressesBase();
            getACSRelyingPartyRelayAddressesBase.ServiceIdentityUsernameForManagement = ServiceIdentityUsernameForManagement;
            getACSRelyingPartyRelayAddressesBase.ServiceIdentityPasswordForManagement = ServiceIdentityPasswordForManagement;
            getACSRelyingPartyRelayAddressesBase.ServiceNamespace = ServiceNamespace;
            getACSRelyingPartyRelayAddressesBase.AcsHostName = AcsHostName;
            getACSRelyingPartyRelayAddressesBase.AcsManagementServicesRelativeUrl = AcsManagementServicesRelativeUrl;
            getACSRelyingPartyRelayAddressesBase.RelyingParty = RelyingParty;

            getACSRelyingPartyRelayAddressesBase.ExecuteCmdletBase(this);
        }
    }
}
