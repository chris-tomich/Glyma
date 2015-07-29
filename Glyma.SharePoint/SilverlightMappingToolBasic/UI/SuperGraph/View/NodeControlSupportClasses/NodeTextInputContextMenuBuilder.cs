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
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.RichTextBoxUI.Menus;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses
{
    public class NodeTextInputContextMenuBuilder : ContextMenuContentBuilder
    {
        private RadRichTextBox _radRichTextBox;

        public NodeTextInputContextMenuBuilder(RadRichTextBox radRichTextBox)
        {
            _radRichTextBox = radRichTextBox;
        }

        protected override ContextMenuGroup CreateHyperlinkCommands(bool forExistingHyperlink)
        {
            return null;
        }

        protected override ContextMenuGroup CreateTableCommands()
        {
            return null;
        }

        protected override ContextMenuGroup CreateCodeBlockCommands()
        {
            return null;
        }

        protected override ContextMenuGroup CreateImageCommands()
        {
            return null;
        }

        protected override ContextMenuGroup CreateTextEditCommands()
        {
            return null;
        }

        protected override ContextMenuGroup CreateHeaderFooterCommands(bool forHeader)
        {
            return null;
        }

        protected override ContextMenuGroup CreateFieldCommands()
        {
            return null;
        }

        protected override ContextMenuGroup CreateFloatingBlockCommands()
        {
            return null;
        }

        protected override ContextMenuGroup CreateListCommands()
        {
            return null;
        }
    }
}
