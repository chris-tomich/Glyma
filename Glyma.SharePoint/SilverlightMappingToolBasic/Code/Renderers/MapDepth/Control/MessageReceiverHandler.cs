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
using VideoPlayerSharedLib;
using System.Linq;

namespace SilverlightMappingToolBasic.MapDepth
{
    public class MessageReceiverHandler
    {
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        public MessageReceiverHandler(INodeNavigator navigator, MapDepthViewManager viewManager, TypeManager typeManager)
        {
            Navigator = navigator;
            ViewManager = viewManager;
            TypeManager = typeManager;
        }

        private INodeNavigator Navigator
        {
            get;
            set;
        }

        private MapDepthViewManager ViewManager
        {
            get;
            set;
        }

        private TypeManager TypeManager
        {
            get;
            set;
        }

        public void MessageReceiver_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if (MessageReceived != null)
            {
                MessageReceived.Invoke(this, e);
            }

            Event receivedEvent = Utilities.Deserialize<Event>(e.Message);
            if (receivedEvent != null)
            {
                switch (receivedEvent.Name)
                {
                    case "GetSourceAndPositionCallback":
                        HandleGetSourceAndPositionCallback(receivedEvent);
                        break;
                    case "CurrentStateChanged":
                        HandledCurrentStateChanged(receivedEvent);
                        break;
                    case "GetPlayingStateCallback":
                        HandleGetPlayingStateCallback(receivedEvent);
                        break;
                    case "Seek":
                        HandleSeekEvent(receivedEvent);
                        break;
                    case "ReplayingLastNode":
                        HandleReplayingLastNodeEvent(receivedEvent);
                        break;
                    default:
                        //Debug.WriteLine("Unhandled message received");
                        break;
                }
            }
        }

        private void HandleGetSourceAndPositionCallback(Event callback)
        {
            if (callback.EventArgs != null && callback.EventArgs.ContainsKey("CallbackId")
                && callback.EventArgs.ContainsKey("Position") && callback.EventArgs.ContainsKey("Source")
                && callback.EventArgs.ContainsKey("NodeId"))
            {
                string callbackId = callback.EventArgs["CallbackId"];
                string source = callback.EventArgs["Source"];
                string nodeId = callback.EventArgs["NodeId"];
                switch (callbackId)
                {
                    case "StartPosition":
                        TimeSpan startPosition = TimeSpan.Parse(callback.EventArgs["Position"]);
                        SaveMediaInformation(nodeId, source, callbackId, startPosition);
                        break;
                    case "EndPosition":
                        TimeSpan endPosition = TimeSpan.Parse(callback.EventArgs["Position"]);
                        SaveMediaInformation(nodeId, source, callbackId, endPosition);
                        break;
                    default:
                        break;
                }
            }
        }

        private void HandleGetPlayingStateCallback(Event callback)
        {
            if (callback.EventArgs != null && callback.EventArgs.ContainsKey("CallbackId")
                && callback.EventArgs.ContainsKey("State") && callback.EventArgs.ContainsKey("NodeId"))
            {
                string callbackId = callback.EventArgs["CallbackId"];
                string state = callback.EventArgs["State"];
                string currentlyPlayingNodeId = callback.EventArgs["NodeId"];

                if (ViewManager != null && ViewManager.CurrentView != null)
                {
                    if (callbackId == Navigator.FocalNodeId.ToString()) //the request was made for this map refresh
                    {
                        if (state == "Playing" || state == "Buffering")
                        {
                            foreach (NodeRenderer nodeRenderer in ViewManager.CurrentView.NodeRenderers.Values)
                            {
                                if (nodeRenderer.Node.Id.ToString() == currentlyPlayingNodeId)
                                {
                                    nodeRenderer.SetIsPlaying(true);
                                    nodeRenderer.ResetMediaIcon(true);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void HandledCurrentStateChanged(Event receivedEvent)
        {
            if (receivedEvent.EventArgs != null && receivedEvent.EventArgs.ContainsKey("State"))
            {
                string state = receivedEvent.EventArgs["State"];
                Guid nodeId = Guid.Empty;
                if (receivedEvent.EventArgs.ContainsKey("NodeId"))
                {
                    Guid.TryParse(receivedEvent.EventArgs["NodeId"], out nodeId);
                }
                if (!string.IsNullOrEmpty(state) && state.ToLower() == "idle")
                {
                    ResetNodeRendererMediaIcon(nodeId, false);
                }
                else if (!string.IsNullOrEmpty(state) && state.ToLower() == "playing")
                {
                    ResetNodeRendererMediaIcon(nodeId, true);
                }
                else if (!string.IsNullOrEmpty(state) && state.ToLower() == "buffering")
                {
                    ResetNodeRendererMediaIcon(nodeId, true);
                }
            }
        }

        private void HandleSeekEvent(Event receivedEvent)
        {
            if (receivedEvent.EventArgs != null && receivedEvent.EventArgs.ContainsKey("NewPosition"))
            {
                if (ViewManager != null && ViewManager.CurrentView != null && ViewManager.CurrentView.NodeRenderers != null)
                {
                    TimeSpan newPosition = TimeSpan.MinValue;
                    if (TimeSpan.TryParse(receivedEvent.EventArgs["NewPosition"], out newPosition))
                    {
                        foreach (NodeRenderer nodeRenderer in ViewManager.CurrentView.NodeRenderers.Values)
                        {
                            if (nodeRenderer.ResetMediaIcon(newPosition))
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void ResetNodeRendererMediaIcon(Guid nodeId, bool playing)
        {
            if (ViewManager != null && ViewManager.CurrentView != null && ViewManager.CurrentView.NodeRenderers != null && nodeId != Guid.Empty)
            {
                foreach (NodeRenderer nodeRenderer in ViewManager.CurrentView.NodeRenderers.Values)
                {
                    if (nodeRenderer.Node.Id == nodeId)
                    {
                        nodeRenderer.ResetMediaIcon(playing);
                    }
                    else
                    {
                        //all other nodes are not playing
                        nodeRenderer.ResetMediaIcon(false);
                    }
                }
            }
        }

        private void HandleReplayingLastNodeEvent(Event receivedEvent)
        {
            if (ViewManager != null && ViewManager.CurrentView != null && ViewManager.CurrentView.NodeRenderers != null
                && receivedEvent.EventArgs != null && receivedEvent.EventArgs.ContainsKey("NodeId"))
            {
                Guid nodeId = Guid.Empty;
                if (Guid.TryParse(receivedEvent.EventArgs["NodeId"], out nodeId))
                {
                    NodeRenderer playingNode = ViewManager.CurrentView.NodeRenderers[nodeId];
                    {
                        playingNode.SetIsPlaying(true);
                        playingNode.ResetMediaIcon(true);
                    }
                }
            }
        }

        private void SaveMediaInformation(string nodeId, string source, string markerName, TimeSpan position)
        {
            var nodeRenderers = ViewManager.Nodes.Where(n => n.Node.Id.ToString() == nodeId);
            INodeRenderer nodeRenderer = nodeRenderers.First() as INodeRenderer;
            IMetadataTypeProxy timeSpanType = TypeManager.GetMetadataType("timespan");
            IMetadataTypeProxy stringType = TypeManager.GetMetadataType("string");
            Navigator.UpdateNodeMetadataAsync(nodeRenderer.Node, Guid.Empty, null, "Video." + markerName, position.ToString(), timeSpanType);
            Navigator.UpdateNodeMetadataAsync(nodeRenderer.Node, Guid.Empty, null, "Video.Source", source, stringType);
        }
    }
}
