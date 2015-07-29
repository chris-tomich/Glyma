using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using SilverlightMappingToolBasic;
using SilverlightMappingToolBasic.MappingService;

namespace SilverlightMappingToolBasic.Controls
{
    public partial class PropertiesEditor : UserControl
    {
        ObservableCollection<MetadataViewModel> _metadata;

        private Dictionary<MetadataContext, MetadataViewModel> _original = null;
        private Dictionary<string, bool> _changes = null;
        private List<MetadataViewModel> _deletedItems = null;
        private List<string> _metadataNames = null;

        public PropertiesEditor()
        {
            InitializeComponent();
            _original = new Dictionary<MetadataContext, MetadataViewModel>();
            _changes = new Dictionary<string, bool>();
            _deletedItems = new List<MetadataViewModel>();
            _metadataNames = new List<string>();

            _metadata = new ObservableCollection<MetadataViewModel>();
            _metadata.CollectionChanged += Metadata_CollectionChanged;
            NodeMetadataDataGrid.ItemsSource = _metadata;

            this.Loaded += new RoutedEventHandler(PropertiesEditor_Loaded);
        }

        private INodeProxy NodeProxy
        {
            get
            {
                return DataContext as INodeProxy;
            }
        }

        public bool HasChanges
        {
            get
            {
                return !(_changes.Where(d => d.Value == true).Count() == 0 && _deletedItems.Count == 0 &&
                    _metadata.Where(md => md.OriginalMetadataName != md.MetadataName).Count() == 0);
            }
        }

        private void PropertiesEditor_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            if (!HasChanges)
            {
                _metadata.Clear();
                _changes.Clear();
                if (NodeProxy != null && NodeProxy.Metadata != null)
                {
                    List<MetadataViewModel> sortedList = new List<MetadataViewModel>();
                    foreach (MetadataContext context in NodeProxy.Metadata.Keys)
                    {
                        /* The 'Note' property has it's own editor panel intended for more text 
                         * The 'XPosition' and 'YPosition' are control via the map positioning
                         * The 'Name' is the node text and editing is done via the node directly
                         * The 'Created' and 'Modified' timestamps should not be able to be changed.
                         * TODO: Decide if the CreatedBy and ModifiedBy properties should be editable.
                         ***/
                        if (context.MetadataName != "Note" && context.MetadataName != "XPosition"
                            && context.MetadataName != "YPosition" && context.MetadataName != "Name"
                            && context.MetadataName != "Created" && context.MetadataName != "Modified"
                            && context.MetadataName != "CollapseState" && context.MetadataName != "Visibility")
                        {
                            MetadataViewModel model = new MetadataViewModel(context, NodeProxy.Metadata[context]);
                            if (!ContainsOriginal(context))
                            {
                                _original.Add(context, model.Clone());
                            }
                            _changes[model.OriginalMetadataName.ToLower()] = false;

                            model.PropertyChanged += new PropertyChangedEventHandler(model_PropertyChanged);
                            sortedList.Add(model);
                        }
                    }
                   
                    sortedList.Sort(new Comparison<MetadataViewModel>(CompareMetadata));
                    foreach (MetadataViewModel model in sortedList)
                    {
                        _metadata.Add(model);
                        _metadataNames.Add(model.OriginalMetadataName);
                    }
                }
            }
        }

        private int CompareMetadata(MetadataViewModel metadata1, MetadataViewModel metadata2)
        {
            return metadata1.MetadataName.CompareTo(metadata2.MetadataName);
        }

        private bool ContainsOriginal(MetadataContext context)
        {
            bool result = false;
            foreach (KeyValuePair<MetadataContext, MetadataViewModel> pair in _original)
            {
                if (pair.Key.MetadataName == context.MetadataName &&
                    pair.Key.NodeUid == context.NodeUid &&
                    pair.Key.DescriptorTypeUid == context.DescriptorTypeUid &&
                    pair.Key.RelationshipUid == context.RelationshipUid)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        private bool HasNameChanged(string newValue)
        {
            if (_metadataNames.Contains(newValue))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            MetadataViewModel model = sender as MetadataViewModel;

            DataGridRow changedRow = GetDataGridRowByDataContext(NodeMetadataDataGrid, model);
            if (e.PropertyName == "MetadataName")
            {
                bool nameChanged = HasNameChanged(model.MetadataName);

                if (nameChanged)
                {
                    MarkChanged(model, changedRow, true);
                }
                else
                {
                    MarkChanged(model, changedRow, false);
                }
            }
            else
            {
                if (IsEqualToOriginal(model))
                {
                    MarkChanged(model, changedRow, false);
                    _changes[model.OriginalMetadataName.ToLower()] = false;
                }
                else
                {
                    MarkChanged(model, changedRow, true);
                    _changes[model.OriginalMetadataName.ToLower()] = true;
                }
            }
        }

        private void MarkChanged(MetadataViewModel model, DataGridRow changedRow, bool changed)
        {
            if (changed)
            {
                SaveButton.IsEnabled = true;
                changedRow.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                if (!HasChanges) //checks if there are any other changes before resetting the Save button
                {
                    SaveButton.IsEnabled = false; //if there are no other changes
                }
                changedRow.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void NodeMetadataDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MetadataViewModel selectedMetadata = NodeMetadataDataGrid.SelectedItem as MetadataViewModel;
            if (selectedMetadata != null)
            {
                EditMetadataButton.IsEnabled = true;
                DeleteMetadataButton.IsEnabled = true;
            }
            else
            {
                EditMetadataButton.IsEnabled = false;
                DeleteMetadataButton.IsEnabled = false;
            }
        }

        private void Metadata_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (HasChanges)
            {
                SaveButton.IsEnabled = true;
            }
            else
            {
                SaveButton.IsEnabled = false;
            }
        }

        private bool IsEqualToOriginal(MetadataViewModel metadata)
        {
            bool result = false;
            foreach (KeyValuePair<MetadataContext, MetadataViewModel> pair in _original)
            {
                MetadataContext context = pair.Key;
                MetadataViewModel model = pair.Value;
                if (context.NodeUid.Value == metadata.NodeUid && model.MetadataName == metadata.OriginalMetadataName)
                {
                    if (context.DescriptorTypeUid.HasValue && context.DescriptorTypeUid.Value != metadata.DescriptorTypeUid)
                    {
                        continue;
                    }
                    if (context.RelationshipUid.HasValue && context.RelationshipUid.Value != metadata.RelationshipUid)
                    {
                        continue;
                    }

                    //the context is the same so compare the items
                    if (model.Equals(metadata))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return result;
        }

        private void AddMetadataButton_Click(object sender, RoutedEventArgs e)
        {
            EditMetadataDialog addMetadataDialog = new EditMetadataDialog();
            addMetadataDialog.Closed += new EventHandler(AddMetadataDialog_Closed);
            addMetadataDialog.Show();
        }

        private void AddMetadataDialog_Closed(object sender, EventArgs e)
        {
            EditMetadataDialog dialog = sender as EditMetadataDialog;
            if (dialog != null && dialog.DialogResult.Value == true)
            {
                //only allow a key to go into the properties once
                if (!_changes.ContainsKey(dialog.MetadataName.ToLower()))
                {
                    MetadataContext newContext = new MetadataContext();
                    newContext.MetadataName = dialog.MetadataName;
                    newContext.NodeUid = NodeProxy.Id;
                    SoapMetadata metadata = new SoapMetadata();
                    metadata.MetadataName = dialog.MetadataName;
                    TypeManager typeManager = IoC.IoCContainer.GetInjectionInstance().GetInstance<TypeManager>();
                    IMetadataTypeProxy[] metaDataTypes = typeManager.GetAllMetadataTypes();
                    foreach (IMetadataTypeProxy metadataType in metaDataTypes)
                    {
                        if (metadataType.Id == dialog.MetadataTypeUid)
                        {
                            SoapMetadataType soapMetadataType = new SoapMetadataType();
                            soapMetadataType.Id = metadataType.Id;
                            soapMetadataType.Name = metadataType.Name;
                            metadata.MetadataType = soapMetadataType;
                            break;
                        }
                    }
                    metadata.MetadataValue = dialog.Value;

                    MetadataViewModel newData = new MetadataViewModel(newContext, metadata);
                    _changes[newData.OriginalMetadataName.ToLower()] = true;
                    _metadata.Add(newData);
                    _metadataNames.Add(newData.MetadataName);
                    newData.PropertyChanged += new PropertyChangedEventHandler(model_PropertyChanged);
                }
                else
                {
                    MessageBox.Show("Metadata could not be added because the property name already existed", "Error Adding Metadata", MessageBoxButton.OK);
                }
            }
        }

        private void EditMetadataButton_Click(object sender, RoutedEventArgs e)
        {
            MetadataViewModel selectedMetadata = NodeMetadataDataGrid.SelectedItem as MetadataViewModel;
            if (selectedMetadata != null)
            {
                EditMetadataDialog editMetadataDialog = new EditMetadataDialog();
                editMetadataDialog.Closed += new EventHandler(EditMetadataDialog_Closed);
                editMetadataDialog.MetadataTypeUid = selectedMetadata.MetadataTypeUid;
                editMetadataDialog.Value = selectedMetadata.MetadataValue;
                editMetadataDialog.MetadataName = selectedMetadata.MetadataName;
                editMetadataDialog.Show();
            }
        }

        private void EditMetadataDialog_Closed(object sender, EventArgs e)
        {
            MetadataViewModel selectedMetadata = NodeMetadataDataGrid.SelectedItem as MetadataViewModel;
            EditMetadataDialog dialog = sender as EditMetadataDialog;
            if (dialog != null && dialog.DialogResult.Value == true)
            {
                selectedMetadata.MetadataValue = dialog.Value;
                selectedMetadata.MetadataTypeUid = dialog.MetadataTypeUid;
                if (!IsEqualToOriginal(selectedMetadata))
                {
                    _changes[selectedMetadata.OriginalMetadataName.ToLower()] = true;
                }
            }
        }

        private void DeleteMetadataButton_Click(object sender, RoutedEventArgs e)
        {
            MetadataViewModel selectedMetadata = NodeMetadataDataGrid.SelectedItem as MetadataViewModel;
            if (selectedMetadata != null)
            {
                MessageBoxResult confirmDelete = MessageBox.Show(string.Format("Pressing OK will remove the metadata with the property name '{0}'.\r\nAre you sure?", selectedMetadata.MetadataName), "Confirm deletion", MessageBoxButton.OKCancel);
                if (confirmDelete == MessageBoxResult.OK)
                {
                    if (_changes.ContainsKey(selectedMetadata.OriginalMetadataName.ToLower()))
                    {
                        _changes.Remove(selectedMetadata.OriginalMetadataName.ToLower());
                    }
                    if (!_deletedItems.Contains(selectedMetadata))
                    {
                        _deletedItems.Add(selectedMetadata);
                    }
                    _metadata.Remove(selectedMetadata);
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (MetadataViewModel metadata in _metadata.Where(md => md.OriginalMetadataName != md.MetadataName))
            {
                NodeProxy.RenameNodeMetadata(metadata.GetContext(), metadata.MetadataName);
                _metadataNames.Remove(metadata.OriginalMetadataName);
                _metadataNames.Add(metadata.MetadataName);
                metadata.UpdateName();
            }

            if (HasChanges)
            {
                TypeManager typeManager = IoC.IoCContainer.GetInjectionInstance().GetInstance<TypeManager>();
                IMetadataTypeProxy[] metaDataTypes = typeManager.GetAllMetadataTypes();

                foreach (KeyValuePair<string, bool> changedMetadata in _changes.Where(changed => changed.Value == true))
                {
                    //only update the changed items
                    try
                    {
                        MetadataViewModel metadata = _metadata.Single(m => m.MetadataName.ToLower() == changedMetadata.Key.ToLower());
                        if (metadata != null)
                        {
                            NodeProxy.SetNodeMetadata(metadata.GetContext(), metadata.MetadataValue, metaDataTypes.Single(md => md.Id == metadata.MetadataTypeUid));
                        }
                    }
                    catch (Exception)
                    {
                        //ignore any errors if the changed metadata isn't found
                    }
                }

                _changes.Clear();

                foreach (MetadataViewModel deletedItem in _deletedItems)
                {
                    NodeProxy.DeleteNodeMetadata(deletedItem.GetContext());
                }

                _deletedItems.Clear();
            }

            SaveButton.IsEnabled = false;
            ResetOriginals();
            ClearChangeIndicator();
        }

        private void ResetOriginals()
        {
            lock (_original)
            {
                _original.Clear();
                foreach (MetadataContext context in NodeProxy.Metadata.Keys)
                {
                    /* The 'Note' property has it's own editor panel intended for more text 
                     * The 'XPosition' and 'YPosition' are control via the map positioning
                     * The 'Name' is the node text and editing is done via the node directly
                     * The 'Created' and 'Modified' timestamps should not be able to be changed.
                     * TODO: Decide if the CreatedBy and ModifiedBy properties should be editable.
                     ***/
                    if (context.MetadataName != "Note" && context.MetadataName != "XPosition"
                        && context.MetadataName != "YPosition" && context.MetadataName != "Name"
                        && context.MetadataName != "Created" && context.MetadataName != "Modified"
                        && context.MetadataName != "CollapseState" && context.MetadataName != "Visibility")
                    {
                        MetadataViewModel model = new MetadataViewModel(context, NodeProxy.Metadata[context]);
                        if (!ContainsOriginal(context))
                        {
                            _original.Add(context, model.Clone());
                        }
                    }
                }
            }
        }

        private void ClearChangeIndicator()
        {
            DataGridAutomationPeer automationPeer = (DataGridAutomationPeer)DataGridAutomationPeer.CreatePeerForElement(NodeMetadataDataGrid);

            // Get the DataGridRowsPresenterAutomationPeer so we can find the rows in the data grid...
            DataGridRowsPresenterAutomationPeer dataGridRowsPresenterAutomationPeer = automationPeer.GetChildren().
                Where(a => (a is DataGridRowsPresenterAutomationPeer)).
                Select(a => (a as DataGridRowsPresenterAutomationPeer)).
                FirstOrDefault();

            if (null != dataGridRowsPresenterAutomationPeer)
            {
                foreach (var item in dataGridRowsPresenterAutomationPeer.GetChildren())
                {
                    // loop to find the DataGridCellAutomationPeer from which we can interrogate the owner -- which is a DataGridRow
                    foreach (var subitem in (item as DataGridItemAutomationPeer).GetChildren())
                    {
                        if ((subitem is DataGridCellAutomationPeer))
                        {
                            // At last -- the only public method for finding a row....
                            DataGridRow row = DataGridRow.GetRowContainingElement(((subitem as DataGridCellAutomationPeer).Owner as FrameworkElement));
                            row.Foreground = new SolidColorBrush(Colors.Black);
                        }
                    }
                }
            }
        }

        public DataGridRow GetDataGridRowByDataContext(DataGrid dataGrid, object dataContext)
        {
            if (null != dataContext)
            {
                DataGridAutomationPeer automationPeer = (DataGridAutomationPeer)DataGridAutomationPeer.CreatePeerForElement(dataGrid);

                // Get the DataGridRowsPresenterAutomationPeer so we can find the rows in the data grid...
                DataGridRowsPresenterAutomationPeer dataGridRowsPresenterAutomationPeer = automationPeer.GetChildren().
                    Where(a => (a is DataGridRowsPresenterAutomationPeer)).
                    Select(a => (a as DataGridRowsPresenterAutomationPeer)).
                    FirstOrDefault();

                if (null != dataGridRowsPresenterAutomationPeer)
                {
                    foreach (var item in dataGridRowsPresenterAutomationPeer.GetChildren())
                    {
                        // loop to find the DataGridCellAutomationPeer from which we can interrogate the owner -- which is a DataGridRow
                        foreach (var subitem in (item as DataGridItemAutomationPeer).GetChildren())
                        {
                            if ((subitem is DataGridCellAutomationPeer))
                            {
                                // At last -- the only public method for finding a row....
                                DataGridRow row = DataGridRow.GetRowContainingElement(((subitem as DataGridCellAutomationPeer).Owner as FrameworkElement));

                                // check this row to see if it is bound to the requested dataContext.
                                if ((row.DataContext) == dataContext)
                                {
                                    return row;
                                }

                                break; // Only need to check one cell in each row
                            }
                        }
                    }
                }
            }

            return null;
        }  
    }
}
