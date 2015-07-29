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
using System.Windows.Messaging;

using SilverlightMappingToolBasic.Controls;

namespace SilverlightMappingToolBasic
{
    public interface IMapControl
    {
        string DomainUid
        {
            get;
            set;
        }

        string NodeUid
        {
            get;
            set;
        }

        string VideoSource
        {
            get;
            set;
        }

        string TransactionalMappingToolSvcUrl
        {
            get;
            set;
        }

        string MappingToolSvcUrl
        {
            get;
            set;
        }

        string ThemeSvcUrl
        {
            get;
            set;
        }

        LocalMessageSender MessageSender
        {
            get;
            set;
        }

        EventHandler<MessageReceivedEventArgs> MessageReceivedHandler
        {
            get;
            set;
        }

        event EventHandler<MessageReceivedEventArgs> MessageReceived;

        INodeNavigator Navigator
        {
            get;
        }

        Canvas MapSurface
        {
            get;
        }

        INodeProxy[] SelectedNodes
        {
            get;
        }

        IRelationshipProxy[] SelectedRelationships
        {
            get;
        }

        event EventHandler NavigatorInitialised;
    }
}
