using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Mouse;
using SilverlightMappingToolBasic.UI.SuperGraph.View.MouseCursor;
using SilverlightMappingToolBasic.UI.SuperGraph.View.PropertiesEditorSupportClasses;
using SilverlightMappingToolBasic.UI.SuperGraph.ViewModel;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses
{
    public partial class NodeDetailsControl : UserControl
    {
        private NodeControl _parentControl;

        public NodeControl ParentControl
        {
            get
            {
                return _parentControl;
            }
            set
            {
                if (_parentControl != value)
                {
                    _parentControl = value;
                    if (_parentControl != null)
                    {
                        DataContext = null;
                        DataContext = _parentControl.ViewModelNode;
                        Canvas.SetTop(this,-65);
                        Canvas.SetLeft(this, 20);
                    }
                    
                }
            }
        }

        public Node Node
        {
            get
            {
                return DataContext as Node;
            }
        }

        public bool IsInitialised
        {
            get; set;
        }

        private List<NodeDetailIndicatorButton> _buttons;
        private List<Point> _defaultLocations;


        private List<Point> DefaultLocations
        {
            get
            {
                if (_defaultLocations == null)
                {
                    _defaultLocations = new List<Point>
                    {
                        new Point {Y = 70, X = 97},
                        new Point {Y = 44, X = 90},
                        new Point {Y = 22, X = 74},
                        new Point {Y = 7, X = 52},
                        new Point {Y = 0, X = 26},
                        new Point {Y = 7, X = 0}
                    };
                }
                return _defaultLocations;
            }
        }

        private List<NodeDetailIndicatorButton> Buttons
        {
            get
            {
                if (_buttons == null)
                {
                    _buttons = new List<NodeDetailIndicatorButton>();
                }
                return _buttons;
            }
        }

        public NodeDetailsControl()
        {
            InitializeComponent();
        }

        public void UnlinkParentControl()
        {
            DataContext = null;
            if (ParentControl != null)
            {
                ParentControl.LayoutRoot.Children.Remove(this);
                ParentControl = null;
            }
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var node = e.NewValue as Node;
            if (node != null)
            {
                ClearButtons();
                if (node.VideoInfo.HasVideo)
                {
                    Buttons.Add(Video);
                }

                if (node.DescriptionType != NodeDescriptionType.None)
                {
                    Buttons.Add(Content);
                }

                if (node.IsTranscluded)
                {
                    Buttons.Add(RelatedMap);
                }
                ShowButtons();
            }
        }

        private void ClearButtons()
        {
            foreach (var child in LayoutRoot.Children)
            {
                var button = child as NodeDetailIndicatorButton;
                if (button != null)
                {
                    button.Visibility = Visibility.Collapsed;
                }
            }
            Buttons.Clear();
        }

        private void CheckShowAllButton()
        {
            if (Buttons.Count > 1)
            {
                AddButton(ShowAll);
            }
        }

        private void AddButton(NodeDetailIndicatorButton button)
        {
            if (!Buttons.Contains(button))
            {
                Buttons.Add(button);
            }
        }

        private void ShowButtons()
        {
            CheckShowAllButton();
            if (Buttons.Count > 1)
            {
                for (var i = 0; i < Buttons.Count; i++)
                {
                    Canvas.SetLeft(Buttons[i], DefaultLocations[i].X);
                    Canvas.SetTop(Buttons[i], DefaultLocations[i].Y);
                    Buttons[i].Visibility = Visibility.Visible;
                }
                Visibility = Visibility.Visible;
            }
            else
            {
                Visibility = Visibility.Collapsed;
            }
            
        }

        private void ShowAll_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ParentControl.SelectNodeDetail(NodeSelectedEventArgs.NodeSelectedType.All, NodeSelectedEventArgs.ClickSource.CornerButton);
        }

        private void RelatedMap_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ParentControl.SelectNodeDetail(NodeSelectedEventArgs.NodeSelectedType.RelatedMap, NodeSelectedEventArgs.ClickSource.CornerButton);
        }

        private void Video_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ParentControl.SelectNodeDetail(NodeSelectedEventArgs.NodeSelectedType.Video, NodeSelectedEventArgs.ClickSource.CornerButton);
        }

        private void Content_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ParentControl.SelectNodeDetail(NodeSelectedEventArgs.NodeSelectedType.RelatedContent, NodeSelectedEventArgs.ClickSource.CornerButton);
        }

        private void Feed_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ParentControl.SelectNodeDetail(NodeSelectedEventArgs.NodeSelectedType.ActiveFeed, NodeSelectedEventArgs.ClickSource.CornerButton);
        }

        
    }
}
