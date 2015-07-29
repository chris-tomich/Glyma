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
    public class NavigatorView
    {
        public NavigatorView()
        {
            NodeRenderers = new NodeRendererCollection();
            RelationshipRenderers = new RelationshipRendererCollection();
        }

        public INodeProxy ContextNode
        {
            get;
            set;
        }

        public NodeRendererCollection NodeRenderers
        {
            get;
            set;
        }

        public RelationshipRendererCollection RelationshipRenderers
        {
            get;
            set;
        }
    }
}
