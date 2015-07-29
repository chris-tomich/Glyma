using System;
using System.Runtime.InteropServices;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.Win32;

namespace SevenSigma.Logging.Features.SP2010LoggingFeature
{
	[Guid("1F31E073-5FF2-4F88-9A65-C767A799B0D8")]
	public class FeatureEventReceiver : SPFeatureReceiver
	{
		const string EventLogApplicationRegistryKeyPath = @"SYSTEM\CurrentControlSet\services\eventlog\Application";

		public override void FeatureActivated(SPFeatureReceiverProperties properties)
		{
			RegisterLoggingService(properties);
		}

		public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
		{
			UnRegisterLoggingService(properties);
		}

		static void RegisterLoggingService(SPFeatureReceiverProperties properties)
		{
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                SPFarm farm = properties.Definition.Farm;

                if (farm != null)
                {
                    LoggingService service = LoggingService.Local;

                    if (service == null)
                    {
                        service = new LoggingService();
                        service.Update();

                        if (service.Status != SPObjectStatus.Online)
                            service.Provision();
                    }
                    //foreach (SPServer server in farm.Servers)
                    //{
                    //    RegistryKey baseKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, server.Address);

                    //    if (baseKey != null)
                    //    {
                    //        RegistryKey eventLogKey = baseKey.OpenSubKey(EventLogApplicationRegistryKeyPath, true);

                    //        if (eventLogKey != null)
                    //        {
                    //            RegistryKey loggingServiceKey = eventLogKey.OpenSubKey(LoggingService.AreaName);

                    //            if (loggingServiceKey == null)
                    //            {
                    //                loggingServiceKey = eventLogKey.CreateSubKey(LoggingService.AreaName, RegistryKeyPermissionCheck.ReadWriteSubTree);
                    //                loggingServiceKey.SetValue("EventMessageFile", @"C:\Windows\Microsoft.NET\Framework\v2.0.50727\EventLogMessages.dll", RegistryValueKind.String);
                    //            }
                    //        }
                    //    }
                    //}
                }
            });
		}

		static void UnRegisterLoggingService(SPFeatureReceiverProperties properties)
		{
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                SPFarm farm = properties.Definition.Farm;

                if (farm != null)
                {
                    LoggingService service = LoggingService.Local;

                    if (service != null)
                        service.Delete();

                    //foreach (SPServer server in farm.Servers)
                    //{
                    //    RegistryKey baseKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, server.Address);

                    //    if (baseKey != null)
                    //    {
                    //        RegistryKey eventLogKey = baseKey.OpenSubKey(EventLogApplicationRegistryKeyPath, true);

                    //        if (eventLogKey != null)
                    //        {
                    //            RegistryKey loggingServiceKey = eventLogKey.OpenSubKey(LoggingService.AreaName);

                    //            if (loggingServiceKey != null)
                    //                eventLogKey.DeleteSubKey(LoggingService.AreaName);
                    //        }
                    //    }
                    //}
                }
            });
		}
	}
}
