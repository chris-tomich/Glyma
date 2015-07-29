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
using System.Diagnostics;

using SilverlightMappingToolBasic.Controls;

namespace SilverlightMappingToolBasic
{
    public class RelationshipRenderer : Canvas, IRelationshipRenderer
    {
        private ArrowControl _arrowControl;

        public RelationshipRenderer(NavigatorView parentNavigatorView, IRelationshipProxy relationshipProxy)
        {
            Relationship = relationshipProxy;
            ParentNavigatorView = parentNavigatorView;

            this.Loaded += new RoutedEventHandler(RelationshipRenderer_Loaded);
        }

        private void RelationshipRenderer_Loaded(object sender, RoutedEventArgs e)
        {
            _arrowControl = new ArrowControl();
            _arrowControl.DataContext = Relationship;
            _arrowControl.ParentNavigatorView = ParentNavigatorView;
            _arrowControl.GlowColor = Colors.Red;
            _arrowControl.MouseLeftButtonDown += new MouseButtonEventHandler(_arrowControl_MouseLeftButtonDown);
            _arrowControl.MouseRightButtonDown += new MouseButtonEventHandler(_arrowControl_MouseRightButtonDown);
            _arrowControl.DragFromAnchorEnded += new RoutedEventHandler(_arrowControl_DragFromAnchorEnded);
            _arrowControl.DragToAnchorEnded += new RoutedEventHandler(_arrowControl_DragToAnchorEnded);
            this.Children.Add(_arrowControl);
        }

        public ArrowControl Arrow
        {
            get { return _arrowControl; }
            set { _arrowControl = value; }
        }

        private void _arrowControl_DragToAnchorEnded(object sender, RoutedEventArgs e)
        {
            NodeRelationshipHelper nrh = IoC.IoCContainer.GetInjectionInstance().GetInstance<NodeRelationshipHelper>();
            if (nrh != null)
            {
                nrh.Relationship = _arrowControl;
                nrh.EdittingSide = RelationshipSide.To;
                nrh.IsEditting = true;
            }
        }

        private void _arrowControl_DragFromAnchorEnded(object sender, RoutedEventArgs e)
        {
            NodeRelationshipHelper nrh = IoC.IoCContainer.GetInjectionInstance().GetInstance<NodeRelationshipHelper>();
            if (nrh != null)
            {
                nrh.Relationship = _arrowControl;
                nrh.EdittingSide = RelationshipSide.From;
                nrh.IsEditting = true;
            }
        }

        private void _arrowControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            NodeRelationshipHelper nrh = IoC.IoCContainer.GetInjectionInstance().GetInstance<NodeRelationshipHelper>();
            if (nrh != null)
            {
                nrh.Relationship = _arrowControl;
            }
        }

        private void _arrowControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            foreach (INodeRenderer renderer in ParentNavigatorView.NodeRenderers.Values)
            {
                renderer.CommitNodeName();
            }

            if (Keyboard.Modifiers != ModifierKeys.Shift)
            {
                foreach (IRelationshipRenderer renderer in ParentNavigatorView.RelationshipRenderers.Values)
                {
                    if (renderer.IsSelected && renderer.Relationship.Id != this.Relationship.Id)
                    {
                        renderer.IsSelected = false;
                    }
                }
                foreach (INodeRenderer nodeRenderer in ParentNavigatorView.NodeRenderers.Values)
                {
                    if (nodeRenderer.IsSelected)
                    {
                        nodeRenderer.IsSelected = false;
                    }
                }
            }
            IsSelected = !IsSelected;
            e.Handled = true;
        }

        #region IRelationshipRenderer Members

        public IRelationshipProxy Relationship
        {
            get;
            protected set;
        }

        public NavigatorView ParentNavigatorView
        {
            get;
            protected set;
        }

        public INodeRenderer To
        {
            get
            {
                return _arrowControl.To;
            }
        }

        public INodeRenderer From
        {
            get
            {
                return _arrowControl.From;
            }
        }

        public bool IsSelected
        {
            get
            {
                return _arrowControl.IsSelected;
            }
            set
            {
                _arrowControl.IsSelected = value;
            }
        }

        public bool IsEditting
        {
            get
            {
                return _arrowControl.IsEditting;
            }
            set
            {
                _arrowControl.IsEditting = value;
            }
        }

        public void UpdateRelationship()
        {
            _arrowControl.UpdateArrow();
        }

        public void UpdateRelationship(Point movingPosition)
        {
            _arrowControl.UpdateArrow(movingPosition);
        }

        #endregion
    }
}
