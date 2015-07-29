using System;
using System.Windows;
using System.Collections.Generic;
using SilverlightMappingToolBasic.Controls;

namespace SilverlightMappingToolBasic
{
    public interface INodeRenderer
    {
        event EventHandler NodePositionUpdated;
        event EventHandler NodePositionUpdating;
        event EventHandler<NodeClickedArgs> NodeDoubleClicked;

        bool IsSelected { get; set; }
        Point Location { get; }
        INodeSkin Skin { get; }
        INodeProxy Node { get; set; }
        NodeControl NodeControl { get; }
        string SkinName { get; }
        Dictionary<string, object> SkinProperties { get; }
        NavigatorView ParentNavigatorView { get; }
        ThemeManager ThemeManagementObject { get; }

        RenderingContextInfo Context { get; set; }

        void SetSkinName(string skinName);

        void CommitNodeName();

        void Refresh();
    }
}
