using System;
using SilverlightMappingToolBasic.UI.SuperGraph.ViewModel;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses
{
    public class NodeSelectedEventArgs : EventArgs
    {
        public enum NodeSelectedType
        {
            RelatedContent,
            RelatedMap,
            ActiveFeed,
            Geography,
            Video,
            All,
        }

        public enum ClickSource
        {
            Node,
            CornerButton
        }

        public Node SelectedNode
        {
            get;
            set;
        }

        public NodeSelectedType Type
        {
            get;
            set;
        }

        public ClickSource Source
        {
            get;
            set;
        }
    }
}
