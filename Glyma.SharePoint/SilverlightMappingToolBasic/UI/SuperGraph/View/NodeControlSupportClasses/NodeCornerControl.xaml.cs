using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses.Interface;
using SilverlightMappingToolBasic.UI.SuperGraph.ViewModel;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses
{
    public partial class NodeCornerControl : UserControl
    {
        public event EventHandler ExtendButtonClicked;
        public event EventHandler<NodeSelectedEventArgs> NodeSelected;

        public bool HasMultipleButton { get; set; }

        public Node Node
        {
            get
            {
                return DataContext as Node;
            }
        }

        public NodeCornerControl()
        {
            InitializeComponent();
            
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //var node = e.NewValue as Node;
            //if (node != null)
            //{
            //    //VideoRemoteControl.DataContext = node.VideoInfo;
            //    var count = 0;
            //    if (node.VideoInfo.HasVideo)
            //    {
            //        //VideoRemoteControl.Visibility = Visibility.Visible;
            //        count ++;

            //    }
                
            //    if (!string.IsNullOrWhiteSpace(node.Description))
            //    {
            //        if (count == 0)
            //        {
            //            ContentButton.Visibility = Visibility.Visible;
            //            NodeCornerButtonLoader.DressButton(ContentButton);
            //        }
            //        else
            //        {
            //            count ++;
            //        }
            //    }

            //    if (node.IsTranscluded)
            //    {
            //        if (count == 0)
            //        {
            //            RelatedNodesButton.Visibility = Visibility.Visible;
            //            NodeCornerButtonLoader.DressButton(RelatedNodesButton);
            //        }
            //        else
            //        {
            //            count++;
            //        }
                    
            //    }

            //    if(count > 1) 
            //    {
            //        //MultiButtonIndicator.Visibility = Visibility.Visible;
            //    }
            //    else
            //    {
            //       // MultiButtonIndicator.Visibility = Visibility.Collapsed;
            //    }
            //}
        }

        private void OnExtendButtonClicked(object sender, RoutedEventArgs e)
        {
            if (ExtendButtonClicked != null)
            {
                ExtendButtonClicked(sender, e);
            }
        }

        
    }
}
