using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.PowerShell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace Glyma.NodeServiceApp.PowerShellRegistration
{
    [Cmdlet(VerbsCommon.New, "GlymaNodeServiceApplication", SupportsShouldProcess = true)]
    public class NewGlymaNodeServiceApplication : SPCmdlet
    {
        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string Name
        {
            get;
            set;
        }

        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public SPIisWebServiceApplicationPoolPipeBind ApplicationPool
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

            // ensure can hit local server
            SPServer server = SPServer.Local;
            if (server == null)
            {
                ThrowTerminatingError(new InvalidOperationException("SharePoint local server not found."), ErrorCategory.ResourceUnavailable, this);
                SkipProcessCurrentRecord();
            }

            // ensure can hit service application
            NodeService service = farm.Services.GetValue<NodeService>();
            if (service == null)
            {
                ThrowTerminatingError(new InvalidOperationException("Glyma Node Service not found (likely not installed)."), ErrorCategory.ResourceUnavailable, this);
                SkipProcessCurrentRecord();
            }

            // ensure can hit app pool
            SPIisWebServiceApplicationPool appPool = this.ApplicationPool.Read();
            if (appPool == null)
            {
                ThrowTerminatingError(new InvalidOperationException("Application pool not found."), ErrorCategory.ResourceUnavailable, this);
                SkipProcessCurrentRecord();
            }

            // verify a service app doesn't already exist
            NodeServiceApplication existingServiceApp = service.Applications.GetValue<NodeServiceApplication>();
            if (existingServiceApp != null)
            {
                WriteError(new InvalidOperationException("Glyma Node Service Application already exists."),
                    ErrorCategory.ResourceExists,
                    existingServiceApp);
                SkipProcessCurrentRecord();
            }

            // create & provision the service app
            if (ShouldProcess(this.Name))
            {
                NodeServiceApplication serviceApp = NodeServiceApplication.Create(
                    this.Name,
                    service,
                    appPool);

                // provision the service app
                serviceApp.Provision();

                // pass service app back to the PowerShell
                WriteObject(serviceApp);
            }
        }
    }
}
