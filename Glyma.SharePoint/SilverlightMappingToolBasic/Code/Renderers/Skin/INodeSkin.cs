using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Media;
using SilverlightMappingToolBasic.Controls;

namespace SilverlightMappingToolBasic
{
    public interface INodeSkin
    {
        double NodeSkinWidth { get; }
        double NodeSkinHeight { get; }
        INodeTypeProxy NodeType { get; }
        PathGeometry SkinClippingGeometry { get; }
        Dictionary<string, object> SkinProperties { get; }

        void InitialiseNodeType(INodeTypeProxy nodeType);
        NodeControl RenderSkinElements(INodeProxy node, string skinName, Dictionary<string, object> skinProperties);
    }
}
