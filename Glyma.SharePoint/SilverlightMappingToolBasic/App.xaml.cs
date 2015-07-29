using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.Windows;
using System.Windows.Browser;
//using SilverlightMappingToolBasic.Code.Authorization;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using SilverlightMappingToolBasic.Code.ColorsManagement;
using SilverlightMappingToolBasic.UI.Extensions.CookieManagement;
using SilverlightMappingToolBasic.UI.Extensions.Security;
using SilverlightMappingToolBasic.UI.SuperGraph.View.MessageBox;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Mouse;
using SilverlightMappingToolBasic.UI.View;
using Telerik.Windows.Controls;
using TransactionalNodeService.Proxy.Exceptions;
using System.ComponentModel;
using SilverlightMappingToolBasic.UI.SuperGraph.ViewModel;

namespace SilverlightMappingToolBasic
{
    public partial class App : Application
    {
        private static UserStyle _userStyle = UserStyle.Reader;
        public static Guid? DomainId;
        public static GlymaParameters Params;
        public static PermissionLevel PermissionLevel = PermissionLevel.None;
        public static SidebarModel SidebarModel= new SidebarModel();
        public static bool IsExportEnabled = false;


        public static bool IsDesignTime
        {
            get { return (Current == null) || (Current.GetType() == typeof (Application)); }
        }


        public static UserStyle UserStyle
        {
            get
            {
                return App._userStyle;
            }
            set
            {
                if (_userStyle != value)
                {
                    App._userStyle = value;
                    App.SidebarModel.UserStyle = value;
                }
            }
        }

        public App()
        {
            Startup += Application_Startup;
            Exit += Application_Exit;
            Telerik.Windows.Controls.StyleManager.ApplicationTheme = new Windows8Theme();
            Windows8Palette.Palette.AccentColor = ColorConverter.FromHex("#FF58aed1");
            InitializeComponent();
            UnhandledException += OnAppUnhandledException;
        }

        private void OnAppUnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            // If the app is running outside of the debugger then report the exception using
            // the browser's exception mechanism. On IE this will display it a yellow alert 
            // icon in the status bar and Firefox will display a script error.
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                // NOTE: This will allow the application to continue running after an exception has been thrown
                // but not handled. 
                // For production applications this error handling should be replaced with something that will 
                // report the error to the website and stop the application.
                e.Handled = true;
                Deployment.Current.Dispatcher.BeginInvoke(() => ReportErrorToDOM(e));
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {

            Params = new GlymaParameters(e.InitParams);

            var mapLoadParamsManager = new MapLoadParamsManager();
            //if the query string contains values pass them on

            if (mapLoadParamsManager.IsValid(HtmlPage.Document.QueryString, MapLoadType.QueryString))
            {
                RootVisual = new MainPage(mapLoadParamsManager);
            }
            else if (mapLoadParamsManager.IsValid(e.InitParams, MapLoadType.InitParams))
            {
                RootVisual = new MainPage(mapLoadParamsManager);
            }
            else if (mapLoadParamsManager.IsValid(CookieManager.ReadAll(), MapLoadType.Cookie))
            {
                RootVisual = new MainPage(mapLoadParamsManager);
            }
            else
            {
                RootVisual = new MainPage();
            }
            RootVisual.MouseRightButtonDown += RootVisualOnMouseRightButtonDown;
        }

        private void RootVisualOnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void Application_Exit(object sender, EventArgs e)
        {

        }

        private void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
        {
            try
            {
                string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
                errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

                System.Windows.Browser.HtmlPage.Window.Eval(
                    "throw new Error(\"Unhandled Error in Silverlight Application " + errorMsg + "\");");
            }
            catch (Exception)
            {
            }
        }

        private void OnUnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is NodeNotFoundException)
            {
                ((MainPage) RootVisual).Loader.Visibility = Visibility.Collapsed;
                SuperMessageBoxService.Show(
                    "Load Map Failed",
                    "Sorry, unfortunately we can’t find the map you’re looking for",
                    "Select a new map",
                    "Cancel",
                    MessageBoxType.Error,
                    () => ((MainPage) RootVisual).HomeScreen()
                    );
            }
            else if (e.ExceptionObject.InnerException != null)
            {
                if (e.ExceptionObject.InnerException is TimeoutException)
                {
                    ((MainPage) RootVisual).Loader.Visibility = Visibility.Collapsed;
                    SuperMessageBoxService.Show(
                        "Communication Failed",
                        "Unfortunately it seems your internet connection is not working",
                        "Refresh",
                        "Cancel",
                        MessageBoxType.Error,
                        () => HtmlPage.Document.Submit()
                        );
                }
            }
            else
            {
                var unhandledExceptionDialog = new UnhandledExceptionDialog();
                unhandledExceptionDialog.UnhandledException = e.ExceptionObject;
                unhandledExceptionDialog.Show();
            }
        }

    }
}
