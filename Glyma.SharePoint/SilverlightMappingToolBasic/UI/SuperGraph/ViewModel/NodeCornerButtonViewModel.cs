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
using SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses;

namespace SilverlightMappingToolBasic.UI.SuperGraph.ViewModel
{
    public class NodeCornerButtonViewModel : ViewModelBase
    {
        public ImageSource Icon
        {
            get;
            set;
        }
        public NodeCornerButtonType ButtonType
        {
            get;
            set;
        }

        public string Name
        {
            get
            {
                switch (ButtonType)
                {
                    case NodeCornerButtonType.Content:
                        return "Show Related Content";
                    case NodeCornerButtonType.Feed:
                        return "Show Activities";
                    case NodeCornerButtonType.Map:
                        return "Show Transcluded Maps";
                    case NodeCornerButtonType.Pause:
                        return "Pause Video";
                    case NodeCornerButtonType.Play:
                        return "Play Video";
                    default:
                        return "None";
                }
            }
        }
    
    }   
}
