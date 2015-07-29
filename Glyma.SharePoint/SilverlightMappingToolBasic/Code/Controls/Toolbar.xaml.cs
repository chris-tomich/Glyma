using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using System.Diagnostics;

namespace SilverlightMappingToolBasic.Controls
{
    public partial class Toolbar : UserControl
    {
        ToolbarItemZoomer mapItem;
        ToolbarItemZoomer questionItem;
        ToolbarItemZoomer ideaItem;
        ToolbarItemZoomer proItem;
        ToolbarItemZoomer conItem;
        ToolbarItemZoomer decisionItem;

        public Toolbar()
        {
            InitializeComponent();

            mapItem = new ToolbarItemZoomer(MapImage);
            questionItem = new ToolbarItemZoomer(QuestionImage);
            ideaItem = new ToolbarItemZoomer(IdeaImage);
            proItem = new ToolbarItemZoomer(ProImage);
            conItem = new ToolbarItemZoomer(ConImage);
            decisionItem = new ToolbarItemZoomer(DecisionImage);
        }

        private void DoMouseEnter(object sender, MouseEventArgs e)
        {
            if (sender == MapImage)
            {
                mapItem.IsMouseOver = true;
                mapItem.Direction = ZoomDirection.In;
            }
            else if (sender == QuestionImage)
            {
                questionItem.IsMouseOver = true;
                questionItem.Direction = ZoomDirection.In;
            }
            else if (sender == IdeaImage)
            {
                ideaItem.IsMouseOver = true;
                ideaItem.Direction = ZoomDirection.In;
            }
            else if (sender == ProImage)
            {
                proItem.IsMouseOver = true;
                proItem.Direction = ZoomDirection.In;
            }
            else if (sender == ConImage)
            {
                conItem.IsMouseOver = true;
                conItem.Direction = ZoomDirection.In;
            }
            else if (sender == DecisionImage)
            {
                decisionItem.IsMouseOver = true;
                decisionItem.Direction = ZoomDirection.In;
            }
        }

        private void DoMouseLeave(object sender, MouseEventArgs e)
        {
            if (sender == MapImage) 
            {
                mapItem.IsMouseOver = false;
            }
            else if (sender == QuestionImage)
            {
                questionItem.IsMouseOver = false;
            }
            else if (sender == IdeaImage)
            {
                ideaItem.IsMouseOver = false;
            }
            else if (sender == ProImage)
            {
                proItem.IsMouseOver = false;
            }
            else if (sender == ConImage)
            {
                conItem.IsMouseOver = false;
            }
            else if (sender == DecisionImage)
            {
                decisionItem.IsMouseOver = false;
            }
        }

        bool isMouseCaptured;
        bool droppingNewItem;

        private void DoMouseDown(object sender, MouseButtonEventArgs e)
        {
            Image item = sender as Image;
            isMouseCaptured = true;
            item.CaptureMouse();
        }

        private void DoMouseUp(object sender, MouseButtonEventArgs e)
        {
            Image item = sender as Image;
            isMouseCaptured = false;
            item.ReleaseMouseCapture();
            droppingNewItem = false;
            Grid parentGrid = this.Parent as Grid;
            IMapControl map = parentGrid.Children[1] as IMapControl;
            AddNodeToMap(map, item, e);
            Canvas mapSurface = map.MapSurface;
            mapSurface.Children.Remove(clonedImage);
        }

        private void AddNodeToMap(IMapControl map, Image item, MouseButtonEventArgs eventArgs)
        {
            TypeManager typeManager = IoC.IoCContainer.GetInjectionInstance().GetInstance<TypeManager>();
            INodeTypeProxy nodeType = null;

            if (item.Name == "IdeaImage")
            {
                nodeType = typeManager.GetNodeType("CompendiumIdeaNode");
            }
            else if (item.Name == "ProImage")
            {
                nodeType = typeManager.GetNodeType("CompendiumProNode");
            }
            else if (item.Name == "ConImage")
            {
                nodeType = typeManager.GetNodeType("CompendiumConNode");
            }
            else if (item.Name == "QuestionImage")
            {
                nodeType = typeManager.GetNodeType("CompendiumQuestionNode");
            }
            else if (item.Name == "MapImage")
            {
                nodeType = typeManager.GetNodeType("CompendiumMapNode");
            }
            else if (item.Name == "DecisionImage")
            {
                nodeType = typeManager.GetNodeType("CompendiumDecisionNode");
            }

            map.Navigator.AddNode(nodeType, "", eventArgs.GetPosition(map as UIElement));
        }

        private static Image clonedImage;

        private void DoMouseMove(object sender, MouseEventArgs e)
        {
            Image item = sender as Image;
            if (isMouseCaptured)
            {
                Grid parentGrid = this.Parent as Grid;
                if (!droppingNewItem)
                {
                    UserControl map = parentGrid.Children[1] as UserControl;
                    Canvas mapSurface = map.FindName("uxMapSurface") as Canvas;

                    if (clonedImage == null)
                    {
                        clonedImage = new Image();
                    }
                    clonedImage.Source = item.Source;
                    clonedImage.Opacity = 0.5;
                    clonedImage.Height = 42;
                    clonedImage.Width = 42;
                    mapSurface.Children.Add(clonedImage);
                    droppingNewItem = true;

                    // Set new position of object.
                    clonedImage.SetValue(Canvas.TopProperty, e.GetPosition(null).Y - (clonedImage.Height/2) - parentGrid.RowDefinitions[0].Height.Value);
                    clonedImage.SetValue(Canvas.LeftProperty, e.GetPosition(null).X - (clonedImage.Width/2));
                }
                else
                {
                    // Set new position of object.
                    clonedImage.SetValue(Canvas.TopProperty, e.GetPosition(null).Y - (clonedImage.Height / 2) - parentGrid.RowDefinitions[0].Height.Value);
                    clonedImage.SetValue(Canvas.LeftProperty, e.GetPosition(null).X - (clonedImage.Width / 2));
                }
            }
        }
    }

    internal class ToolbarItemZoomer
    {
        public ToolbarItemZoomer(Image image)
        {
            Timer = new Storyboard();
            Timer.Duration = TimeSpan.FromMilliseconds(50);
            Timer.Completed += new EventHandler(Timer_Completed);
            Timer.Begin();
            ScaleTransform = image.RenderTransform as ScaleTransform;
        }

        private ScaleTransform ScaleTransform
        {
            get;
            set;
        }

        private Storyboard Timer
        {
            get;
            set;
        }

        public bool IsMouseOver
        {
            get;
            set;
        }

        public ZoomDirection Direction
        {
            get;
            set;
        }

        [DebuggerStepThrough]
        private void Timer_Completed(object sender, EventArgs e)
        {
            Direction = AdjustScale(Direction, ScaleTransform, IsMouseOver, Timer);
        }

        [DebuggerStepThrough]
        private ZoomDirection AdjustScale(ZoomDirection direction, ScaleTransform scale, bool mouseOver, Storyboard timer)
        {
            if (direction == ZoomDirection.In)
            {
                if (scale.ScaleX < 1.3)
                {
                    scale.ScaleX += 0.05;
                    scale.ScaleY += 0.05;
                }
                else if (mouseOver == false)
                {
                    direction = ZoomDirection.Out;
                }
            }
            else if (scale.ScaleX > 1.0)
            {
                scale.ScaleX -= 0.05;
                scale.ScaleY -= 0.05;
            }

            timer.Begin();
            return direction;
        }
    }

    internal enum ZoomDirection
    {
        In, Out, None
    }
    
}
