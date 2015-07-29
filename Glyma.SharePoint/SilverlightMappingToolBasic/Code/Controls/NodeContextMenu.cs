using System;
using System.Collections;
using System.Collections.Generic;
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
using System.Xml.Serialization;
using System.Text;
using System.Xml;
using System.IO;

using VideoPlayerSharedLib;
using SilverlightMappingToolBasic.MappingService;
using System.Windows.Browser;
using SilverlightMappingToolBasic.UI.Extensions.VideoWebPart;

// NOTE: This class is not used anymore
namespace SilverlightMappingToolBasic.Controls
{
    public class NodeContextMenu : ContextMenu
    {
        private EventHandler<MessageReceivedEventArgs> messageHandler = null;
        MenuItem playVideoMenuItem;
        MenuItem pauseVideoMenuItem;

        public NodeContextMenu(INodeProxy nodeProxy, IMapControl map, INodeService service, Point location)
            : base()
        {
            messageHandler = new EventHandler<MessageReceivedEventArgs>(map_MessageReceived);

            NodeProxy = nodeProxy;
            //MessageSender = map.MessageSender;
            Navigator = map.Navigator;
            NodeService = service;
            map.MessageReceived += messageHandler;
            MapControl = map;
            Location = location;

            this.Loaded += new RoutedEventHandler(NodeContextMenu_Loaded);
            this.Unloaded += new RoutedEventHandler(NodeContextMenu_Unloaded);
        }

        

        private INodeProxy NodeProxy
        {
            get;
            set;
        }

        private LocalMessageSender MessageSender
        {
            get;
            set;
        }

        private INodeNavigator Navigator
        {
            get;
            set;
        }

        private INodeService NodeService
        {
            get;
            set;
        }

        private IMapControl MapControl
        {
            get;
            set;
        }

        private Point Location
        {
            get;
            set;
        }

        private void NodeContextMenu_Loaded(object sender, RoutedEventArgs e)
        {
            Command getPlayingStateCommand = new Command();
            getPlayingStateCommand.Name = "GetPlayingState";
            getPlayingStateCommand.Params = new List<Param>();
            getPlayingStateCommand.Params.Add(new Param() { Name = "NodeId", Value = NodeProxy.Id.ToString() });
            //Utilities.SendMessage<Command>(MessageSender, getPlayingStateCommand);

            playVideoMenuItem = new MenuItem();
            playVideoMenuItem.Header = "Play Video Segment";
            MetadataContext videoSourceKey = new MetadataContext()
            {
                MetadataName = "Video.Source",
                NodeUid = NodeProxy.Id
            };
            playVideoMenuItem.Click += new RoutedEventHandler(playVideoMenuItem_Click);

            pauseVideoMenuItem = new MenuItem();
            pauseVideoMenuItem.Header = "Pause Video Segment";
            pauseVideoMenuItem.Click += delegate(object sender2, RoutedEventArgs e2)
            {
                Command pauseCommand = new Command();
                pauseCommand.Name = "Pause";
                //Utilities.SendMessage<Command>(MessageSender, pauseCommand);
            };
            if (!NodeProxy.HasMetadata(videoSourceKey))
            {
                playVideoMenuItem.IsEnabled = false;
                pauseVideoMenuItem.IsEnabled = false;
            }
            this.Items.Add(playVideoMenuItem);
            this.Items.Add(pauseVideoMenuItem);

            Separator videoOpSep = new Separator();
            this.Items.Add(videoOpSep);

            MenuItem recordStartPosition = new MenuItem();
            recordStartPosition.Header = "Record Start Position";
            recordStartPosition.Click += delegate(object sender2, RoutedEventArgs e2)
            {
                Command getStartPositionCommand = new Command();
                getStartPositionCommand.Name = "GetSourceAndPosition";
                getStartPositionCommand.Params = new List<Param>();
                getStartPositionCommand.Params.Add(new Param() { Name = "CallbackId", Value = "StartPosition" });
                getStartPositionCommand.Params.Add(new Param(){Name="NodeId", Value=NodeProxy.Id.ToString()});
                //Utilities.SendMessage<Command>(MessageSender, getStartPositionCommand);
            };
            this.Items.Add(recordStartPosition);

            MenuItem recordEndPosition = new MenuItem();
            recordEndPosition.Header = "Record End Position";
            recordEndPosition.Click += delegate(object sender2, RoutedEventArgs e2)
            {
                Command getEndPositionCommand = new Command();
                getEndPositionCommand.Name = "GetSourceAndPosition";
                getEndPositionCommand.Params = new List<Param>();
                getEndPositionCommand.Params.Add(new Param() { Name="CallbackId", Value="EndPosition"});
                getEndPositionCommand.Params.Add(new Param() { Name="NodeId", Value=NodeProxy.Id.ToString()});
                //Utilities.SendMessage<Command>(MessageSender, getEndPositionCommand);
            };
            this.Items.Add(recordEndPosition);

            MenuItem clearVideoData = new MenuItem();
            clearVideoData.Header = "Clear Node Video Markers";
            clearVideoData.Click += delegate(object sender2, RoutedEventArgs e2)
            {
                TypeManager typeManager = IoC.IoCContainer.GetInjectionInstance().GetInstance<TypeManager>();
                IMetadataTypeProxy stringMetadataTypeProxy = typeManager.GetMetadataType("string");
                Navigator.UpdateNodeMetadataAsync(NodeProxy, Guid.Empty, null, "Video.Source", "", stringMetadataTypeProxy);
            };
            clearVideoData.IsEnabled = false;
            if (NodeProxy.HasMetadata(videoSourceKey))
            {
                if (!string.IsNullOrEmpty(NodeProxy.GetNodeMetadata(videoSourceKey).MetadataValue))
                {
                    clearVideoData.IsEnabled = true;
                }
            }
            this.Items.Add(clearVideoData);

            Separator videoOp2Sep = new Separator();
            this.Items.Add(videoOp2Sep);

            MenuItem deleteNodeMenuItem = new MenuItem();
            deleteNodeMenuItem.Header = "Delete Node";
            deleteNodeMenuItem.Click += new RoutedEventHandler(deleteNodeMenuItem_Click);
            this.Items.Add(deleteNodeMenuItem);

            Separator copyOpSep = new Separator();
            this.Items.Add(copyOpSep);

            MenuItem cloneMenuItem = new MenuItem();
            cloneMenuItem.Header = "Clone";
            cloneMenuItem.Click += new RoutedEventHandler(cloneMenuItem_Click);
            this.Items.Add(cloneMenuItem);

            MenuItem copyMenuItem = new MenuItem();
            copyMenuItem.Header = "Copy";
            copyMenuItem.Click += new RoutedEventHandler(copyMenuItem_Click);
            this.Items.Add(copyMenuItem);

            Separator propertiesSep = new Separator();
            this.Items.Add(propertiesSep);

            MenuItem propertiesMenuItem = new MenuItem();
            propertiesMenuItem.Header = "Properties...";
            propertiesMenuItem.Click += new RoutedEventHandler(propertiesMenuItem_Click);
            this.Items.Add(propertiesMenuItem);

            Separator sendSep = new Separator();
            this.Items.Add(sendSep);

            MenuItem sendMenuItem = new MenuItem();
            sendMenuItem.Header = "Send to Web Part Connection";
            sendMenuItem.Click += new RoutedEventHandler(sendMenuItem_Click);
            this.Items.Add(sendMenuItem);
        }

        private void playVideoMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MetadataContext videoSourceKey = new MetadataContext()
            {
                MetadataName = "Video.Source",
                NodeUid = NodeProxy.Id
            };
            string source = NodeProxy.GetNodeMetadata(videoSourceKey).MetadataValue;

            VideoSizeHelper sizeHelper = new VideoSizeHelper(NodeProxy);
            RelatedContentPanelUtil.Instance.LoadRelatedVideoContent(source, sizeHelper.Size);

            string startPosition = null;
            string endPosition = null;
            Command playCommand = new Command();
            playCommand.Name = "Play";
            playCommand.Params = new List<Param>();
            playCommand.Params.Add(new Param() { Name = "Source", Value = source });
            MetadataContext videoStartKey = new MetadataContext()
            {
                MetadataName = "Video.StartPosition",
                NodeUid = NodeProxy.Id
            };
            if (NodeProxy.HasMetadata(videoStartKey))
            {
                startPosition = NodeProxy.GetNodeMetadata(videoStartKey).MetadataValue;
                playCommand.Params.Add(new Param() { Name="StartTimeCode", Value=startPosition});
            }
            MetadataContext videoEndKey = new MetadataContext()
            {
                MetadataName = "Video.EndPosition",
                NodeUid = NodeProxy.Id
            };
            if (NodeProxy.HasMetadata(videoEndKey))
            {
                endPosition = NodeProxy.GetNodeMetadata(videoEndKey).MetadataValue;
                playCommand.Params.Add(new Param() { Name="EndTimeCode", Value=endPosition});
            }
            playCommand.Params.Add(new Param() { Name="NodeId", Value=NodeProxy.Id.ToString()});
            //Utilities.SendMessage<Command>(MessageSender, playCommand);
        }

        private void deleteNodeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (MapControl.SelectedNodes != null && MapControl.SelectedNodes.Length <= 1)
            {
                //single node deletion
                if (Navigator.FocalNodeId != NodeProxy.ParentMapNodeUid)
                {
                    NodeService.DeleteNodeTransclusion(Navigator.DomainId, Navigator.FocalNodeId, NodeProxy);
                }
                else
                {
                    if (NodeProxy.TransclusionCount == 1)
                    {
                        NodeService.DeleteNode(Navigator.DomainId, NodeProxy.Id);
                    }
                    else
                    {
                        NodeService.DeleteNodePromoteTransclusion(Navigator.DomainId, Navigator.FocalNodeId, NodeProxy);
                    }
                }
            }
            else
            {
                //multi node deletion
                if (MapControl.SelectedNodes != null)
                {
                    foreach (INodeProxy nodeProxy in MapControl.SelectedNodes)
                    {
                        if (Navigator.FocalNodeId != nodeProxy.ParentMapNodeUid)
                        {
                            NodeService.DeleteNodeTransclusion(Navigator.DomainId, Navigator.FocalNodeId, nodeProxy);
                        }
                        else
                        {
                            if (nodeProxy.TransclusionCount == 1)
                            {
                                NodeService.DeleteNode(Navigator.DomainId, nodeProxy.Id);
                            }
                            else
                            {
                                NodeService.DeleteNodePromoteTransclusion(Navigator.DomainId, Navigator.FocalNodeId, nodeProxy);
                            }
                        }
                    }
                }
            }
        }

        private void sendMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<Guid, INodeProxy> nodes = new Dictionary<Guid, INodeProxy>();
            nodes.Add(NodeProxy.Id, NodeProxy);
            //send a basic XML representation of the node
            if (MapControl.SelectedNodes.Length > 0)
            {
                foreach (INodeProxy nodeProxy in MapControl.SelectedNodes)
                {
                    if (!nodes.ContainsKey(nodeProxy.Id))
                    {
                        nodes.Add(nodeProxy.Id, nodeProxy);
                    }
                }
            }

            INodeProxy[] nodesArray = new INodeProxy[nodes.Values.Count];
            nodes.Values.CopyTo(nodesArray, 0);
            HtmlPage.Window.Invoke("sendMessage", "'" + NodeSerializer.SerializeNode(nodesArray) + "'");
        }

        private void propertiesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            NodePropertiesDialog npd = new NodePropertiesDialog();
            npd.DataContext = NodeProxy;
            MetadataContext noteKey = new MetadataContext()
            {
                MetadataName = "Note",
                NodeUid = NodeProxy.Id
            };
            if (NodeProxy.HasMetadata(noteKey))
            {
                npd.Note = NodeProxy.GetNodeMetadata(noteKey).MetadataValue;
            }
            npd.Closed += new EventHandler(NodePropertiesDialog_Close);
            npd.Show();
        }

        private void copyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (MapControl.SelectedNodes != null && MapControl.SelectedNodes.Length <= 1)
            {
                using (StringWriter sw = new StringWriter())
                {
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true;
                    XmlWriter xw = XmlWriter.Create(sw, settings);
                    xw.WriteStartDocument();
                    xw.WriteStartElement("SelectedNodes");
                    xw.WriteStartElement("Node");
                    xw.WriteAttributeString("Id", NodeProxy.Id.ToString());
                    xw.WriteValue(NodeProxy.Name);
                    xw.WriteEndElement();
                    xw.WriteEndElement();
                    xw.Flush();
                    Clipboard.SetText(sw.ToString());
                }
            }
            else
            {
                //multi node copying
                if (MapControl.SelectedNodes != null)
                {
                    using (StringWriter sw = new StringWriter())
                    {
                        XmlWriterSettings settings = new XmlWriterSettings();
                        settings.Indent = true;
                        XmlWriter xw = XmlWriter.Create(sw, settings);
                        xw.WriteStartDocument();
                        xw.WriteStartElement("SelectedNodes");
                        foreach (INodeProxy nodeProxy in MapControl.SelectedNodes)
                        {
                            xw.WriteStartElement("Node");
                            xw.WriteAttributeString("Id", nodeProxy.Id.ToString());
                            xw.WriteValue(nodeProxy.Name);
                            xw.WriteEndElement();
                        }
                        xw.WriteEndElement();
                        xw.Flush();
                        Clipboard.SetText(sw.ToString());
                    }
                }
            }
        }

        private void cloneMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (MapControl.SelectedNodes != null && MapControl.SelectedNodes.Length <= 1)
            {
                Point cloneLocation = new Point(Location.X + 50, Location.Y + 50);
                Navigator.PasteNodeClone(NodeProxy.Id, cloneLocation);
            }
            else
            {
                //multi node cloning
                if (MapControl.SelectedNodes != null)
                {
                    foreach (INodeProxy nodeProxy in MapControl.SelectedNodes)
                    {
                        double x = 0.0;
                        double y = 0.0;

                        MetadataContext xPosKey = new MetadataContext()
                        {
                            MetadataName = "XPosition",
                            NodeUid = nodeProxy.Id
                        };
                        MetadataContext yPosKey = new MetadataContext()
                        {
                            MetadataName = "YPosition",
                            NodeUid = nodeProxy.Id
                        };

                        IDescriptorTypeProxy descriptorType;

                        //The is being drawn as a transclusion representation (i.e. not in it's original parent map)
                        if (nodeProxy.ParentMapNodeUid != Guid.Empty && nodeProxy.ParentMapNodeUid != Navigator.FocalNodeId)
                        {
                            descriptorType = IoC.IoCContainer.GetInjectionInstance().GetInstance<TypeManager>().GetDescriptorType("TransclusionMap");
                            xPosKey.DescriptorTypeUid = descriptorType.Id;
                            yPosKey.DescriptorTypeUid = descriptorType.Id;

                            foreach (IRelationshipProxy relationship in GetTransclusionRelationship(nodeProxy))
                            {
                                xPosKey.RelationshipUid = relationship.Id;
                                yPosKey.RelationshipUid = relationship.Id;

                                if (nodeProxy.Metadata != null && nodeProxy.HasMetadata(xPosKey) && nodeProxy.HasMetadata(yPosKey))
                                {
                                    double.TryParse(nodeProxy.GetNodeMetadata(xPosKey).MetadataValue, out x);
                                    double.TryParse(nodeProxy.GetNodeMetadata(yPosKey).MetadataValue, out y);
                                    break;
                                }
                            }
                        }

                        //The node is being drawn in it's parent map (i.e. not a transclusion representation)
                        else
                        {
                            IRelationshipProxy mapRelationship = GetMapRelationship(nodeProxy);
                            descriptorType = IoC.IoCContainer.GetInjectionInstance().GetInstance<TypeManager>().GetDescriptorType("From");
                            xPosKey.DescriptorTypeUid = descriptorType.Id;
                            yPosKey.DescriptorTypeUid = descriptorType.Id;

                            if (mapRelationship != null)
                            {
                                xPosKey.RelationshipUid = mapRelationship.Id;
                                yPosKey.RelationshipUid = mapRelationship.Id;
                            }

                            if (nodeProxy.Metadata != null && nodeProxy.HasMetadata(xPosKey) && nodeProxy.HasMetadata(yPosKey))
                            {
                                double.TryParse(nodeProxy.GetNodeMetadata(xPosKey).MetadataValue, out x);
                                double.TryParse(nodeProxy.GetNodeMetadata(yPosKey).MetadataValue, out y);
                            }
                        }

                        Point cloneLocation = new Point(x + 50, y + 50);
                        Navigator.PasteNodeClone(nodeProxy.Id, cloneLocation);
                    }
                }
            }
        }

        private void NodeContextMenu_Unloaded(object sender, RoutedEventArgs e)
        {
            if (MapControl != null)
            {
                MapControl.MessageReceived -= messageHandler;
            }
        }

        private void NodePropertiesDialog_Close(object sender, EventArgs e)
        {
            NodePropertiesDialog dialog = sender as NodePropertiesDialog;
            if (dialog != null)
            {
                if (dialog.DialogResult.Value)
                {
                    TypeManager typeManager = IoC.IoCContainer.GetInjectionInstance().GetInstance<TypeManager>();
                    IMetadataTypeProxy stringMetadataTypeProxy = typeManager.GetMetadataType("string");
                    Navigator.UpdateNodeMetadataAsync(dialog.NodeProxy, Guid.Empty, null, "Note", dialog.Note, stringMetadataTypeProxy);
                }
                else
                {
                    //cancel was selected
                }
            }
        }

        private void map_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Event receivedEvent = Utilities.Deserialize<Event>(e.Message);
            if (receivedEvent != null)
            {
                switch (receivedEvent.Name)
                {
                    case "GetPlayingStateCallback":
                        HandleGetPlayingStateCallback(receivedEvent);
                        break;
                    default:
                        //Debug.WriteLine("Unhandled message received");
                        break;
                }
            }
        }

        private void HandleGetPlayingStateCallback(Event e)
        {
            if (e.EventArgs != null && e.ContainsEventArg("State") && e.ContainsEventArg("NodeId"))
            {
                if (NodeProxy.Id.ToString() == e.GetEventArgValue("NodeId"))
                {
                    switch (e.GetEventArgValue("State"))
                    {
                        case "Playing":
                            pauseVideoMenuItem.IsEnabled = true;
                            break;
                        case "Paused":
                            pauseVideoMenuItem.IsEnabled = false;
                            break;
                        default:
                            pauseVideoMenuItem.IsEnabled = false;
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Helper method for getting the relationship that matches nodes relationship to the map currently being rendered
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IRelationshipProxy GetMapRelationship(INodeProxy node)
        {
            foreach (IDescriptorProxy descriptor in node.Descriptors.GetByDescriptorTypeName("From"))
            {
                if (descriptor.Relationship.RelationshipType.Name == "MapContainerRelationship")
                {
                    //filter to MapContainerRelationships only
                    foreach (IDescriptorProxy alternateDescriptor in descriptor.Relationship.Descriptors.GetByDescriptorTypeName("To"))
                    {
                        if (alternateDescriptor.NodeId == Navigator.FocalNodeId)
                        {
                            return descriptor.Relationship;
                        }
                    }
                }
            }
            return null;
        }

        private IEnumerable<IRelationshipProxy> GetTransclusionRelationship(INodeProxy node)
        {
            foreach (IDescriptorProxy descriptor in node.Descriptors.GetByDescriptorTypeName("To"))
            {
                //filter to MapContainerRelationships only
                if (descriptor.Relationship.RelationshipType.Name == "TransclusionRelationship")
                {
                    foreach (IDescriptorProxy alternateDescriptor in descriptor.Relationship.Descriptors.GetByDescriptorTypeName("TransclusionMap"))
                    {
                        if (alternateDescriptor.NodeId == Navigator.FocalNodeId)
                        {
                            yield return descriptor.Relationship;
                        }
                    }
                }
            }
        }

    }
}
