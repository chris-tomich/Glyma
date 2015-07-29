using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using SilverlightMappingToolBasic.Controls;
using SilverlightMappingToolBasic.MappingService;

namespace SilverlightMappingToolBasic.MapDepth
{
    public class NodePropertiesDialogHelper
    {
        public NodePropertiesDialogHelper(INodeNavigator navigator)
        {
            Navigator = navigator;
        }

        private INodeNavigator Navigator
        {
            get;
            set;
        }

        public void ShowNodePropertiesDialog(INodeProxy node)
        {
            NodePropertiesDialog npd = new NodePropertiesDialog();
            npd.DataContext = node;
            MetadataContext noteKey = new MetadataContext()
            {
                NodeUid = node.Id,
                MetadataName = "Note"
            };
            if (node.HasMetadata(noteKey))
            {
                npd.Note = node.GetNodeMetadata(noteKey).MetadataValue;
            }
            npd.Closed += new EventHandler(NodePropertiesDialog_Close);
            npd.Show();
        }

        private void NodePropertiesDialog_Close(object sender, EventArgs e)
        {
            NodePropertiesDialog dialog = sender as NodePropertiesDialog;
            if (dialog != null)
            {
                if (dialog.DialogResult.Value)
                {
                    TypeManager typeManager = IoC.IoCContainer.GetInjectionInstance().GetInstance<TypeManager>();
                    IMetadataTypeProxy stringMetadataTypeProxy = typeManager.GetMetadataType("string");
                    Navigator.UpdateNodeMetadataAsync(dialog.NodeProxy, Guid.Empty, null, "Note", dialog.Note, stringMetadataTypeProxy);
                }
                else
                {
                    //cancel was selected
                }
            }
        }
    }
}
