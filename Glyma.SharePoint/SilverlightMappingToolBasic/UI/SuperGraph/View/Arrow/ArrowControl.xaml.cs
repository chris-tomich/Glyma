using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Media;
using SilverlightMappingToolBasic.Code.ColorsManagement;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Interface;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Mouse;
using SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses;
using SimpleIoC;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.Arrow
{
    public partial class ArrowControl : UserControl, IArrowControl
    {
        private bool _isCollapsed;
        private IIoCContainer _ioc;

        private readonly SolidColorBrush _selectedColor = new SolidColorBrush(ColorConverter.FromHex("#FF58aed1"));
        private readonly SolidColorBrush _nonSelectedColor = new SolidColorBrush(Colors.Black);

        public ArrowControl(NodeControl from, NodeControl to)
        {
            InitializeComponent();
            From = from;
            To = to;
        }

        public ISelectorControl Ref
        {
            get;
            set;
        }

        public ArrowContextMenu ArrowContextMenu
        {
            get
            {
                return Ref.ArrowContextMenu;
            }
        }


        public NodeControl From
        {
            get; private set;
        }

        public NodeControl To
        {
            get; private set;
        }

        public IIoCContainer IoC
        {
            get
            {
                if (_ioc == null)
                {
                    _ioc = IoCContainer.GetInjectionInstance();
                }

                return _ioc;
            }
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var notifyPropertyChangedDataContext = DataContext as INotifyPropertyChanged;

            if (notifyPropertyChangedDataContext != null)
            {
                notifyPropertyChangedDataContext.PropertyChanged += OnPropertyChanged;
            }

            var viewModel = DataContext as ViewModel.ArrowViewModel;

            if (viewModel != null)
            {
                if (viewModel.HasSpaceForLine && !IsCollapsed)
                {
                    Visibility = Visibility.Visible;
                }
                else
                {
                    Visibility = Visibility.Collapsed;
                }

                Canvas.SetLeft(this, viewModel.Location.X);
                Canvas.SetTop(this, viewModel.Location.Y);
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var viewModel = DataContext as ViewModel.ArrowViewModel;

            if (viewModel != null)
            {
                if (viewModel.HasSpaceForLine && !IsCollapsed)
                {
                    Visibility = Visibility.Visible;
                }
                else
                {
                    Visibility = Visibility.Collapsed;
                }

                Canvas.SetLeft(this, viewModel.Location.X);
                Canvas.SetTop(this, viewModel.Location.Y);
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {

        }

        private void OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (App.UserStyle == UserStyle.Reader) {return;}
            if (!IoC.GetInstance<ArrowCreationManager>().InMotion)
            {
                if (Ref != null && Ref.NodesSelector != null)
                {
                    if (!Ref.NodesSelector.HasArrow(this))
                    {
                        if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                        {
                            Ref.NodesSelector.AddArrow(this);
                        }
                        else
                        {
                            Ref.NodesSelector.Clear();
                            Ref.NodesSelector.AddArrow(this);
                        }
                    }

                    e.Handled = true;
                    var location = e.GetPosition((UIElement)Ref);
                    Ref.ArrowContextMenu.ArrowControl = this;
                    ((SuperGraphControl)Ref).ContextMenuContainer.ShowContextMenu<ArrowContextMenu>(location);
                }
            }
        }

        public void UnlinkRelatedNodeControls()
        {
            To.UnLinkFromNode(From);
        }

        public bool IsCollapsed
        {
            get
            {
                return _isCollapsed;
            }
            set
            {
                _isCollapsed = value;
                Visibility = _isCollapsed ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private void LayoutRoot_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        public void Select()
        {
            Arrow2.Fill = _selectedColor;
            Arrow2.Stroke = _selectedColor;
            Path2.Stroke = _selectedColor;
            Focus();
        }

        public void DeSelect()
        {
            Arrow2.Fill = _nonSelectedColor;
            Arrow2.Stroke = _nonSelectedColor;
            Path2.Stroke = _nonSelectedColor;
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (App.UserStyle == UserStyle.Reader && e.Key != Key.Ctrl && e.Key != Key.Shift && e.Key != Key.E) return;
            switch (e.Key)
            {
                case Key.Delete:
                    Delete();
                    break;
            }
        }

        void IArrowControl.Delete()
        {
            Delete();
        }

        private void Delete()
        {
            Ref.NodesSelector.Delete();
        }


        private void LayoutRoot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (App.UserStyle == UserStyle.Reader)
            {

            }
            else
            {
                e.Handled = true;
            }
            if (Ref != null && Ref.NodesSelector != null && !Ref.NodesSelector.HasArrow(this))
            {
                if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                {
                    Ref.NodesSelector.AddArrow(this);
                }
                else
                {
                    Ref.NodesSelector.Clear();
                    Ref.NodesSelector.AddArrow(this);
                }
            }
        }
    }
}
