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

namespace SilverlightMappingToolBasic
{
    public class NodeViewInfo : INodeViewInfo
    {
        public NodeViewInfo()
        {
        }

        #region INodeViewInfo Members

        public bool IsFocalNode
        {
            get;
            set;
        }

        public INode Node
        {
            get;
            set;
        }

        #endregion
    }
}
