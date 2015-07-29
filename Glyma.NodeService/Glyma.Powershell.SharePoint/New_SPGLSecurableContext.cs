using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace Glyma.Powershell.SharePoint
{
    [Cmdlet(VerbsCommon.New, "SPGLSecurableContext")]
    public class New_SPGLSecurableContext : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public object Identity
        {
            get;
            set;
        }

        [Parameter(Position = 1, Mandatory = true)]
        public string SecurableContextName
        {
            get;
            set;
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            using (GlymaConfiguration configuration = new GlymaConfiguration(Identity, this))
            {
                if (string.IsNullOrEmpty(configuration.SecurityDbName) || string.IsNullOrEmpty(configuration.SecurityDbServer))
                {
                    WriteWarning("The provided site doesn't have a valid Security database server or Security database name declared. Aborting securable context creation.");
                    return;
                }

                string connectionString = "Data Source=" + configuration.SecurityDbServer + ";Initial Catalog=" + configuration.SecurityDbName + ";Integrated Security=True";

                using (SecurityDatabaseDataContext dataContext = new SecurityDatabaseDataContext(connectionString))
                {
                    SecurableContext securableContext = new SecurableContext();
                    securableContext.SecurableContextName = SecurableContextName;
                    securableContext.SecurableContextUid = Guid.NewGuid();
                    securableContext.SiteSPID = configuration.Site.ID;

                    dataContext.SecurableContexts.InsertOnSubmit(securableContext);
                    dataContext.SubmitChanges();

                    SPPropertyBag properties = configuration.SecurityWeb.Properties;

                    properties["Glyma.SecurableContextId"] = securableContext.SecurableContextId.ToString();
                    properties.Update();
                    configuration.SecurityWeb.Update();
                }
            }
        }
            
    }
}
