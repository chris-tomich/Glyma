using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.PowerShell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace Glyma.NodeServiceApp.PowerShellRegistration
{
    [Cmdlet(VerbsCommon.New, "GlymaNodeServiceApplicationProxy", SupportsShouldProcess = true)]
    public class NewGlymaNodeServiceApplicationProxy : SPCmdlet
    {
        private Uri _uri;

        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string Name
        {
            get;
            set;
        }

        [Parameter(Mandatory = true, ParameterSetName = "Uri")]
        [ValidateNotNullOrEmpty]
        public string Uri
        {
            get
            {
                return _uri.ToString();
            }
            set
            {
                _uri = new Uri(value);
            }
        }

        [Parameter(Mandatory = true, ParameterSetName = "ServiceApplication")]
        [ValidateNotNullOrEmpty]
        public SPServiceApplicationPipeBind ServiceApplication
        {
            get;
            set;
        }

        protected override bool RequireUserFarmAdmin()
        {
            return true;
        }

        protected override void InternalProcessRecord()
        {
            // ensure can hit farm
            SPFarm farm = SPFarm.Local;
            if (farm == null)
            {
                ThrowTerminatingError(new InvalidOperationException("SharePoint farm not found."), ErrorCategory.ResourceUnavailable, this);
                SkipProcessCurrentRecord();
            }

            // ensure proxy installed
            NodeServiceProxy serviceProxy = farm.ServiceProxies.GetValue<NodeServiceProxy>();
            if (serviceProxy == null)
            {
                ThrowTerminatingError(new InvalidOperationException("Glyma Node Service Proxy not found (likely not installed)."), ErrorCategory.NotInstalled, this);
                SkipProcessCurrentRecord();
            }

            // ensure can hit service application
            NodeServiceApplicationProxy existingServiceAppProxy = serviceProxy.ApplicationProxies.GetValue<NodeServiceApplicationProxy>();
            if (existingServiceAppProxy != null)
            {
                ThrowTerminatingError(new InvalidOperationException("Glyma Node Service Application Proxy already exists."), ErrorCategory.ResourceExists, this);
                SkipProcessCurrentRecord();
            }

            Uri serviceApplicationAddress = null;
            if (ParameterSetName == "Uri")
            {
                serviceApplicationAddress = _uri;
            }
            else if (ParameterSetName == "ServiceApplication")
            {
                // make sure can get a refernce to service app
                SPServiceApplication serviceApp = ServiceApplication.Read();
                if (serviceApp == null)
                {
                    WriteError(new InvalidOperationException("Service application not found."), ErrorCategory.ResourceExists, serviceApp);
                    SkipProcessCurrentRecord();
                }

                // make sure can connect to service app
                ISharedServiceApplication sharedServiceApp = serviceApp as ISharedServiceApplication;
                if (sharedServiceApp == null)
                {
                    WriteError(new InvalidOperationException("Service application not found."), ErrorCategory.ResourceExists, serviceApp);
                    SkipProcessCurrentRecord();
                }

                serviceApplicationAddress = sharedServiceApp.Uri;
            }
            else
                ThrowTerminatingError(new InvalidOperationException("Invalid parameter set."), ErrorCategory.InvalidArgument, this);

            // create the service app proxy
            if ((serviceApplicationAddress != null) && ShouldProcess(this.Name))
            {
                NodeServiceApplicationProxy serviceAppProxy = new NodeServiceApplicationProxy(
                    this.Name,
                    serviceProxy,
                    serviceApplicationAddress);

                // provision the service app proxy
                serviceAppProxy.Provision();

                // add the service app proxy to the default service application proxy group
                SPServiceApplicationProxyGroup defaultGroup = SPServiceApplicationProxyGroup.Default;
                defaultGroup.Add(serviceAppProxy);
                defaultGroup.Update(true);

                // pass service app proxy back to the PowerShell
                WriteObject(serviceAppProxy);
            }
        }
    }
}
