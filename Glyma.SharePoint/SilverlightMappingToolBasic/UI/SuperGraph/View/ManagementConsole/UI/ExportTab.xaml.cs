using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows;
using Glyma.UtilityService.Proxy;
using SilverlightMappingToolBasic.UI.Extensions;
using SilverlightMappingToolBasic.UI.SuperGraph.View.ManagementConsole.ViewModel;
using SilverlightMappingToolBasic.UI.SuperGraph.View.MessageBox;
using SilverlightMappingToolBasic.UI.SuperGraph.View.RichTextSupportClasses;
using SimpleIoC;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using SelectionChangedEventArgs = Telerik.Windows.Controls.SelectionChangedEventArgs;
using System.Windows.Browser;
using System.IO;
using System.Net;
using Glyma.Debug;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.ManagementConsole.UI
{
    public partial class ExportTab : UserControl
    {
        private bool _isBusy;

        private SaveFileDialog _saveFileDialog;

        private bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                LoaderGrid.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public IExportServiceManager ExportServiceManager
        {
            get
            {
                return IoCContainer.GetInjectionInstance().GetInstance<IExportServiceManager>();
            }
        }

        private RootMap RootMap
        {
            get { return DataContext as RootMap; }
        }

        private IColumnFilterDescriptor TypeDescriptor
        {
            get
            {
                var typeColumns = ExportJobGridView.Columns["Type"];
                return typeColumns.ColumnFilterDescriptor;
            }
        }

        private IEnumerable<ExportType> Types
        {
            get
            {
                var types = new List<ExportType>();
                //if (GlymaXml.IsChecked.HasValue && GlymaXml.IsChecked.Value)
                //{
                //    types.Add(ExportType.GlymaXml);
                //}
                if (Word.IsChecked.HasValue && Word.IsChecked.Value)
                {
                    types.Add(ExportType.Word);
                }
                if (Pdf.IsChecked.HasValue && Pdf.IsChecked.Value)
                {
                    types.Add(ExportType.PDF);
                }
                if (Compendium.IsChecked.HasValue && Compendium.IsChecked.Value)
                {
                    types.Add(ExportType.Compendium);
                }
                return types;
            }
        } 


        private IColumnFilterDescriptor StatusDescriptor
        {
            get
            {
                var typeColumns = ExportJobGridView.Columns["Status"];
                return typeColumns.ColumnFilterDescriptor;
            }
        }

        private IEnumerable<ExportStatus> Statuses
        {
            get
            {
                var statuses = new List<ExportStatus>();
                if (Processing.IsChecked.HasValue && Processing.IsChecked.Value)
                {
                    statuses.Add(ExportStatus.Processing);
                }
                if (Completed.IsChecked.HasValue && Completed.IsChecked.Value)
                {
                    statuses.Add(ExportStatus.Completed);
                }
                if (Scheduled.IsChecked.HasValue && Scheduled.IsChecked.Value)
                {
                    statuses.Add(ExportStatus.Scheduled);
                }
                if (Error.IsChecked.HasValue && Error.IsChecked.Value)
                {
                    statuses.Add(ExportStatus.Error);
                }
                
                return statuses;
            }
        } 


        public ExportTab()
        {
            InitializeComponent();

            ExportTypeBox.ItemsSource = Types;
            //_filter = new CompositeFilterDescriptor();
            //_filter.LogicalOperator = FilterCompositionLogicalOperator.Or;
            //DataFilter.FilterDescriptors.Add(new CompositeFilterDescriptor());
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                var rootMap = e.NewValue as RootMap;
                if (rootMap != null)
                {
                    Visibility = Visibility.Visible;
                    ExportServiceManager.GetExportJobsCompleted.RegisterEvent(rootMap.Id, OnGetExportJobsCompleted);
                    ExportServiceManager.GetExportJobsAsync(rootMap.ParentId, rootMap.Id);
                    SelectAll();
                }
                else
                {
                    Visibility = Visibility.Collapsed;
                }
            }
        }

        private void OnGetExportJobsCompleted(object sender, ResultEventArgs<ExportJobCollection> e)
        {
            if (!e.HasError)
            {
                if (e.Context is Guid && (Guid)e.Context == RootMap.Id)
                {
                    if (RootMap != null)
                    {
                        RootMap.ExportJobs = new ObservableCollection<IExportJob>();
                        foreach (var pair in e.Result.OrderByDescending(q => q.Value.Created))
                        {
                            RootMap.ExportJobs.Add(pair.Value);
                        }
                        RefreshFilter();
                    }
                }
            }
            else
            {
                SuperMessageBoxService.ShowError("Error Occurred", "There was a failure reading the exports that exist for the selected map.");
            }
        }

        private void SelectAll()
        {
            //GlymaXml.IsChecked = true;
            Pdf.IsChecked = true;
            Word.IsChecked = true;
            Compendium.IsChecked = true;
            Processing.IsChecked = true;
            Completed.IsChecked = true;
            Scheduled.IsChecked = true;
            Error.IsChecked = true;
        }

        private void Filter_OnChanged(object sender, RoutedEventArgs e)
        {
            RefreshFilter();
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsBusy)
            {
                IsBusy = true;
                if (ExportTypeBox.SelectedIndex >= 0 && RootMap != null)
                {
                    //TODO: in the future when exporting different MapType's we may have different options
                    var type = EnumHelper.GetEnum<ExportType>(ExportTypeBox.SelectedValue.ToString());
                    if (type == ExportType.PDF || type == ExportType.Word)
                    {
                        var exportOptionDialog = new ExportOptionDialog(type, MapType.IBIS); //TODO: In the future other MapType's will be handled
                        exportOptionDialog.ExportOptionSelected += ExportOptionDialogOnExportOptionSelected;
                        exportOptionDialog.Closed += ExportOptionDialogOnClosed;
                        exportOptionDialog.Show();
                    }
                    else
                    {
                        ExportOptionDialogOnExportOptionSelected(sender,
                            new ExportOptionSelectedEventArgs {ExportOption = new ExportOption(type, MapType.IBIS)}); //TODO: In the future other MapType's will be handled
                    }
                }
            }
            else
            {
                SuperMessageBoxService.ShowInformation("Busy",
                            "Sorry we are currently busy, please try again later");
            }
        }

        private void ExportOptionDialogOnClosed(object sender, EventArgs e)
        {
            var dialog = sender as ExportOptionDialog;
            if (dialog != null)
            {
                if (!dialog.DialogResult.HasValue || !dialog.DialogResult.Value)
                {
                    IsBusy = false;
                }
            }
        }

        private void ExportOptionDialogOnExportOptionSelected(object sender, ExportOptionSelectedEventArgs e)
        {
            try
            {
                var exportType = e.ExportOption.ExportType;
                var mapType = e.ExportOption.MapType;
                IExportJob exist = null;
#if DEBUG
                foreach (var exportJob in RootMap.ExportJobs)
                {
                    var exportOption = new ExportOption(exportJob.ExportProperties, exportJob.Type, exportJob.MapType);
                    if (exportOption.Equals(e.ExportOption))
                    {
                        exist = exportJob;
                        break;
                    }
                }
                //exist = RootMap.ExportJobs.FirstOrDefault(q => new ExportOption(q.ExportProperties, q.Type, q.MapType).Equals(e.ExportOption));
#else
                exist = RootMap.ExportJobs.FirstOrDefault(q => new ExportOption(q.ExportProperties, q.Type, q.MapType).Equals(e.ExportOption) && q.IsCurrent);
#endif

                if (exist != null)
                {
                    switch (exist.Status)
                    {
                        case ExportStatus.Completed:
                            SuperMessageBoxService.ShowInformation("Export Exists",
                                "An existing export that matches is up-to-date and ready for download already");
                            break;
                        case ExportStatus.Error:
                            SuperMessageBoxService.ShowInformation("Export Exists",
                                "An existing export that matches did not complete successfully, delete the failed export and try again");
                            break;
                        case ExportStatus.Processing:
                        case ExportStatus.Scheduled:
                            SuperMessageBoxService.ShowInformation("Export Exists",
                                    string.Format("An existing export that matches is already {0}, please wait until it has completed", exist.Status.ToString().ToLower()));
                            break;
                        default:
                            break;
                    }
                    IsBusy = false;
                }
                else
                {
                    ExportServiceManager.CreateExportJobCompleted.RegisterEvent(RootMap.Id, OnCreateExportJobCompleted);
                    ExportServiceManager.CreateExportJobAsync(RootMap.ParentId, RootMap.Id, e.ExportOption.Value, mapType, exportType);
                    DebugLogger.Instance.LogMsg("Submitted export job details, Domain: {0}, RootMap: {1}, MapType: {2}, ExportType: {3}",RootMap.ParentId, RootMap.Id, mapType, exportType);
                }
            }
            catch (Exception ex)
            {
                DebugLogger.Instance.LogMsg("An error occurred processing the Export Options dialog. {0}.", ex.ToString());
                SuperMessageBoxService.ShowError("Error Occurred",
                    "You cannot export the map at the moment, please try again later.");
            }
        }

        private void OnCreateExportJobCompleted(object sender, ResultEventArgs<IExportJob> e)
        {
            RefreshExportJobs();
            if (e.HasError)
            {
                DebugLogger.Instance.LogMsg("There was an error creating the export job: {0}", e.ErrorMessage);
            }
            else
            {
                if (e.Result != null && e.Result.CreatedBy != null && !string.IsNullOrEmpty(e.Result.CreatedBy.DisplayName))
                {
                    DebugLogger.Instance.LogMsg("Export job was successfully created by {0} with the details, Job ID: {1}, MapType: {2}, ExportType: {3}", e.Result.CreatedBy.DisplayName, e.Result.Id, e.Result.MapType, e.Result.Type);
                }
                else
                {
                    DebugLogger.Instance.LogMsg("No error was reported when creating the export job but the returned details of the created were invalid.");
                }
            }
        }

        private void Delete_OnClick(object sender, RoutedEventArgs e)
        {
            var button = sender as RadButton;
            if (button != null)
            {
                var exportJob = button.DataContext as IExportJob;
                if (exportJob != null)
                {
                    SuperMessageBoxService.ShowConfirmation("Delete Export Confirmation",
                        "Are you sure to delete the exported file?", () =>
                        {
                            if (!IsBusy)
                            {
                                IsBusy = true;
                                ExportServiceManager.DeleteExportJobCompleted.RegisterEvent(exportJob.Id,
                                    OnDeleteExportJobCompleted);
                                ExportServiceManager.DeleteExportJobAsync(exportJob);
                            }
                        });

                }
            }
        }

        private void OnDeleteExportJobCompleted(object sender, ResultEventArgs<IExportJob> e)
        {
            RefreshExportJobs();
            if (e.HasError)
            {
                DebugLogger.Instance.LogMsg("An error occurred when deleting the export job: {0}", e.ErrorMessage);
            }
            else
            {
                if (e.Result != null)
                {
                    DebugLogger.Instance.LogMsg("The export job with the ID {0} was deleted.", e.Result.Id);
                }
                else
                {
                    DebugLogger.Instance.LogMsg("The export job deleted and didn't report an error but the details of the deleted job were null.", e.Result.Id);
                }
            }
        }

        private void RefreshFilter()
        {
            if (RootMap != null)
            {
                if (RootMap.ExportJobs.Count > 0)
                {
                    StatusDescriptor.SuspendNotifications();
                    StatusDescriptor.Clear();

                    if (Statuses.Any())
                    {
                        foreach (var status in Statuses)
                        {
                            StatusDescriptor.DistinctFilter.AddDistinctValue(status);
                        }
                    }
                    else
                    {
                        StatusDescriptor.DistinctFilter.AddDistinctValue(ExportStatus.Unknown);
                    }
                    StatusDescriptor.ResumeNotifications();

                    TypeDescriptor.SuspendNotifications();
                    TypeDescriptor.Clear();
                    if (Types.Any())
                    {
                        foreach (var type in Types)
                        {
                            TypeDescriptor.DistinctFilter.AddDistinctValue(type);
                        }
                    }
                    else
                    {
                        TypeDescriptor.DistinctFilter.AddDistinctValue(ExportType.Unknown);
                    }
                    TypeDescriptor.ResumeNotifications();

                }
                
                IsBusy = false;
            }
            
        }

        private void ExportTypeBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ExportButton.IsEnabled = ExportTypeBox.SelectedIndex >= 0;
        }

        private void Refresh_OnClick(object sender, RoutedEventArgs e)
        {
            if (!IsBusy)
            {
                IsBusy = true;
                RefreshExportJobs();
            }
        }

        private void RefreshExportJobs()
        {
            if (RootMap != null)
            {
                ExportServiceManager.GetExportJobsCompleted.RegisterEvent(RootMap.Id, OnGetExportJobsCompleted);
                ExportServiceManager.GetExportJobsAsync(RootMap.ParentId, RootMap.Id);
            }
        }

        /// <summary>
        /// NOTE: When debugging the SaveFileDialog will throw a SecurityException because it detects it's not being initiated by a user initiated action,
        /// this does not happen at runtime. It's well documented online that debugging will mess up how Silverlight detects the SaveFileDialog was initialised.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Download_OnClick(object sender, RoutedEventArgs e)
        {
            this._saveFileDialog = new SaveFileDialog();
            var button = sender as RadButton;
            if (button != null)
            {
                var exportJob = button.DataContext as IExportJob;
                if (exportJob != null && exportJob.Link != null)
                {
                    Uri uri;
                    if (Uri.TryCreate(exportJob.Link, UriKind.Absolute, out uri))
                    {
                        string extension = GetDefaultExtension(exportJob.Type);
                        if (!string.IsNullOrEmpty(extension))
                        {
                            this._saveFileDialog.DefaultExt = extension;
                        }
                        string filter = GetFilterForExportType(exportJob.Type);
                        if (!string.IsNullOrEmpty(filter)) 
                        {
                            this._saveFileDialog.Filter = filter;
                        }
                        string defaultFilename = GetDefaultFileName(uri);
                        if (!string.IsNullOrEmpty(defaultFilename))
                        {
                            //setting this property does cause a prompt if the user wants to save the file before the save file dialog is shown.
                            this._saveFileDialog.DefaultFileName = defaultFilename;
                        }

                        bool? dialogResult = this._saveFileDialog.ShowDialog();

                        if (dialogResult == true)
                        {
                            try
                            {
                                WebClient client = new WebClient();
                                client.OpenReadCompleted += (s, eventArgs) =>
                                    {
                                        try
                                        {
                                            if (eventArgs.Error == null)
                                            {
                                                using (Stream fs = (Stream)this._saveFileDialog.OpenFile())
                                                {
                                                    eventArgs.Result.CopyTo(fs);
                                                    fs.Flush();
                                                    fs.Close();
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            SuperMessageBoxService.ShowError("Download Export Error", "Saving the Glyma export failed.");
                                        }
                                    };
                                Uri relativeUri = GetRelativeUrl(uri); //make it so that it works with alternate URL mappings
                                client.OpenReadAsync(relativeUri);
                            }
                            catch (Exception ex)
                            {
                                SuperMessageBoxService.ShowError("Download Export Error", "Saving the Glyma export failed.");
                            }
                        }
                    }
                }
            }
        }

        private string GetDefaultFileName(Uri downloadLink)
        {
            string filename = null;
            if (downloadLink != null)
            {
                filename = Path.GetFileName(downloadLink.LocalPath);
            }
            return filename;
        }

        private string GetFilterForExportType(ExportType exportType)
        {
            string filters = null;
            switch (exportType)
            {
                case ExportType.Compendium:
                    filters = "Compendium XML File (*.xml)|*.xml|All Files|*.*";
                    break;
                case ExportType.GlymaXml:
                    filters = "Glyma XML File (*.glm)|*.glm|All Files|*.*";
                    break;
                case ExportType.PDF:
                    filters = "Adobe Acrobat File (*.pdf)|*.pdf|All Files|*.*";
                    break;
                case ExportType.Word:
                    filters = "Microsoft Word Document (*.docx)|*.docx|All Files|*.*";
                    break;

            }
            return filters;
        }

        private string GetDefaultExtension(ExportType exportType)
        {
            string defaultExtenstion = null;
            switch (exportType)
            {
                case ExportType.Compendium:
                    defaultExtenstion = ".xml";
                    break;
                case ExportType.GlymaXml:
                    defaultExtenstion = ".glm";
                    break;
                case ExportType.PDF:
                    defaultExtenstion = ".pdf";
                    break;
                case ExportType.Word:
                    defaultExtenstion = ".docx";
                    break;

            }
            return defaultExtenstion;
        }

        private Uri GetRelativeUrl(Uri exportUri)
        {
            string exportUriStr = exportUri.ToString();
            int thirdSlash = exportUriStr.IndexOf("/", 8); //supports looking after http:// or https://
            string relativeSiteUri = exportUriStr.Substring(thirdSlash);
            exportUri = new Uri(relativeSiteUri, UriKind.Relative);
            return exportUri;
        }

        private void OutDatedDownload_OnClick(object sender, RoutedEventArgs e)
        {
            SuperMessageBoxService.Show("Confirmation", 
                "The export file you've chosen is outdated, are you still want to download the file?",
                "Yes","No",MessageBoxType.Confirmation, () =>
                {
                    var button = sender as RadButton;
                    if (button != null)
                    {
                        var exportJob = button.DataContext as IExportJob;
                        if (exportJob != null && exportJob.Link != null)
                        {
                            Uri uri;
                            if (Uri.TryCreate(exportJob.Link, UriKind.Absolute, out uri))
                            {
                                System.Windows.Browser.HtmlPage.Window.Navigate(uri, "_blank");
                            }

                        }
                    }
                });
            
        }

        private void Cancel_OnClick(object sender, RoutedEventArgs e)
        {
            
            var button = sender as RadButton;
            if (button != null)
            {
                var exportJob = button.DataContext as IExportJob;
                if (exportJob != null)
                {
                    SuperMessageBoxService.ShowConfirmation("Cancel Export Confirmation",
                        "Are you sure to cancel the export?", () =>
                        {
                            if (!IsBusy)
                            {
                                IsBusy = true;
                                ExportServiceManager.DeleteExportJobCompleted.RegisterEvent(exportJob.Id,
                                    OnDeleteExportJobCompleted);
                                ExportServiceManager.DeleteExportJobAsync(exportJob);
                            }
                        });
                    
                }
            }
        }
    }
}
