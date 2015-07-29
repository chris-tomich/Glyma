using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Administration;
using System.ServiceModel.Activation;
using System.ServiceModel;

using Microsoft.SharePoint.Client.Services;
using Microsoft.SharePoint.Utilities;

namespace SevenSigma.Logging
{
    [BasicHttpBindingServiceMetadataExchangeEndpointAttribute]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
    public class LoggingWCFService : ILoggingWCFService
    {
        public void WriteTrace(Category category, TraceSeverity traceSeverity, string message)
        {
            string categoryName = GetCategoryName(category);
            LoggingService.WriteTrace(categoryName, traceSeverity, message);
        }

        private static string GetCategoryName(Category category)
        {
            string categoryName = LoggingService.Categories.ApplicationPages;
            switch (category)
            {
                case Category.ApplicationPage:
                    categoryName = LoggingService.Categories.ApplicationPages;
                    break;
                case Category.WCFService:
                    categoryName = LoggingService.Categories.WcfServices;
                    break;
                default:
                    categoryName = "Unknown";
                    break;
            }
            return categoryName;
        }
    }
}
