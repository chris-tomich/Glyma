//using System;
//using System.Net;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Documents;
//using System.Windows.Ink;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Animation;
//using System.Windows.Shapes;
//using System.Windows.Browser;
//using System.ServiceModel;
//using System.Security.Principal;

//using SilverlightMappingToolBasic.LoggingService;


//namespace SilverlightMappingToolBasic
//{
//    public class Logger
//    {
//        private static object _lockObj = new object();

//        private static LoggingWCFServiceClient Client
//        {
//            get
//            {
//                string loggingSvcUrl = HtmlPage.Window.Invoke("getLoggingSvcUrl") as string;
//                BasicHttpBinding binding = new BasicHttpBinding();
//                binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
//                EndpointAddress address = new EndpointAddress(new Uri(loggingSvcUrl));
//                LoggingWCFServiceClient client = new LoggingWCFServiceClient(binding, address);
//                return client;
//            }
//        }

//        public static void WriteTrace(Category category, TraceSeverity traceSeverity, string message)
//        {
//            lock (_lockObj)
//            {
//                LoggingWCFServiceClient client = Client;
//                if (client != null)
//                {
//                    client.WriteTraceAsync(category, traceSeverity, message);
//                }
//            }
//        }
//    }
//}
