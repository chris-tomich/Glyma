using System;
using System.Linq;
using System.Windows.Controls;
using SilverlightMappingToolBasic.UI.SuperGraph.View.PropertiesEditorSupportClasses.Edit;
using SilverlightMappingToolBasic.UI.SuperGraph.ViewModel;
using SilverlightMappingToolBasic.UI.ViewModel;
using Telerik.Windows.Documents.Model;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses
{
    public partial class NodeTextInput : UserControl
    {
        private NodeControl _pareNodeControl;

        

        public NodeControl ParentControl
        {
            get
            {
                return _pareNodeControl;
            }
            set
            {
                if (_pareNodeControl != value)
                {
                    if (Node != null)
                    {
                        Node.Name = Editor.UIText;
                    }
                    _pareNodeControl = value;
                    if (_pareNodeControl != null)
                    {
                        Editor.UIText = Text;
                    }
                }
            }
        }

        public bool IsInitialised
        {
            get;
            set;
        }

        public string Text
        {
            get
            {
                if (Node == null || Node.Name == null)
                {
                    return string.Empty;
                }
                return Node.Name;
            }
            set
            {
                if (Node != null)
                {
                    Node.Name = value;
                }
            }
        }

        public Node Node
        {
            get
            {
                if (ParentControl == null)
                {
                    return null;
                }
                return ParentControl.ViewModelNode;
            }
        }

        public RadDocument Doc
        {
            get
            {
                return Editor.Document;
            }
            set
            {
                Editor.Document = value;
            }
        }

        public NodeTextInput()
        {
            InitializeComponent();
            Editor.TextUpdated += EditorOnTextUpdated;
            Editor.ClosedWithoutUpdate += EditorOnClosedWithoutUpdate;
        }

        private void EditorOnClosedWithoutUpdate(object sender, EventArgs eventArgs)
        {
            if (ParentControl != null)
            {
                ParentControl.ParentSurface.Focus();
            }

            UnlinkNodeControl();
        }

        private void EditorOnTextUpdated(object sender, EventArgs eventArgs)
        {
            if (ParentControl != null)
            {
                ParentControl.ParentSurface.Focus();
            }

            FinishEdit();
        }

        public void FinishEdit()
        {
            if (Node != null)
            {
                var oldName = Text;
                var newName = Editor.UIText;
                if (!oldName.Equals(newName))
                {
                    Node.Name = newName;
                    ((INodeProperties)Node.NodeProperties).EndEdit();
                    if (Node.Name == null)
                    {
                        ParentControl.NodeText.Text = newName;
                    }
                }
                if (ParentControl.ParentSurface.Selector.NodeControls.Any() && !ParentControl.ParentSurface.Selector.IsMultiSelect)
                {
                    ParentControl.ParentSurface.Selector.NodeControls.First().Focus();
                }
                else
                {
                    ParentControl.Focus();
                }
                
                UnlinkNodeControl();
            }
        }

        public void UnlinkNodeControl()
        {
            if (ParentControl != null)
            {
                ParentControl.TextArea.Children.Remove(this);
                //ParentControl = null;
            }
        }
    }
}
