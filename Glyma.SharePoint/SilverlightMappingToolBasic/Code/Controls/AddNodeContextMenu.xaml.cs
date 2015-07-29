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

namespace SilverlightMappingToolBasic.Controls
{
    public partial class AddNodeContextMenu : UserControl
    {
        public event EventHandler<AddNodeEventArgs> NodeAdded;

        public AddNodeContextMenu()
        {
            InitializeComponent();
        }

        private void ConImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Canvas fe = Parent as Canvas;
            fe.Children.Remove(this);

            AddNodeEventArgs eventArgs = new AddNodeEventArgs();
            eventArgs.NodeTypeName = "CompendiumConNode";
            Point location = e.GetPosition(null);
            eventArgs.XPosition = location.X - (this.ActualWidth / 2);
            eventArgs.YPosition = location.Y - (this.ActualHeight / 2); // - 64; (toolbar row)
            if (NodeAdded != null)
            {
                NodeAdded.Invoke(this, eventArgs);
            }
        }

        private void ProImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Canvas fe = Parent as Canvas;
            fe.Children.Remove(this);

            AddNodeEventArgs eventArgs = new AddNodeEventArgs();
            eventArgs.NodeTypeName = "CompendiumProNode";
            Point location = e.GetPosition(null);
            eventArgs.XPosition = location.X - (this.ActualWidth / 2);
            eventArgs.YPosition = location.Y - (this.ActualHeight / 2); // -64; (toolbar row)
            if (NodeAdded != null)
            {
                NodeAdded.Invoke(this, eventArgs);
            }
        }

        private void IdeaImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Canvas fe = Parent as Canvas;
            fe.Children.Remove(this);

            AddNodeEventArgs eventArgs = new AddNodeEventArgs();
            eventArgs.NodeTypeName = "CompendiumIdeaNode";
            Point location = e.GetPosition(null);
            eventArgs.XPosition = location.X - (this.ActualWidth / 2);
            eventArgs.YPosition = location.Y - (this.ActualHeight / 2); // -64; (toolbar row)
            if (NodeAdded != null)
            {
                NodeAdded.Invoke(this, eventArgs);
            }
        }

        private void QuestionImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Canvas fe = Parent as Canvas;
            fe.Children.Remove(this);

            AddNodeEventArgs eventArgs = new AddNodeEventArgs();
            eventArgs.NodeTypeName = "CompendiumQuestionNode";
            Point location = e.GetPosition(null);
            eventArgs.XPosition = location.X - (this.ActualWidth / 2);
            eventArgs.YPosition = location.Y - (this.ActualHeight / 2); // -64; (toolbar row)
            if (NodeAdded != null)
            {
                NodeAdded.Invoke(this, eventArgs);
            }
        }

        private void MapImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Canvas fe = Parent as Canvas;
            fe.Children.Remove(this);

            AddNodeEventArgs eventArgs = new AddNodeEventArgs();
            eventArgs.NodeTypeName = "CompendiumMapNode";
            Point location = e.GetPosition(null);
            eventArgs.XPosition = location.X - (this.ActualWidth / 2);
            eventArgs.YPosition = location.Y - (this.ActualHeight / 2); // -64; (toolbar row)
            if (NodeAdded != null)
            {
                NodeAdded.Invoke(this, eventArgs);
            }
        }
    }

    public class AddNodeEventArgs : EventArgs
    {
        public string NodeTypeName;
        public double XPosition;
        public double YPosition;
    }
}
