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
using System.Xml;
using System.IO;

using SilverlightMappingToolBasic;

using VideoPlayerSharedLib;
using System.Windows.Browser;
using SilverlightMappingToolBasic.UI.Extensions.VideoWebPart;

// NOTE: This class is not used anymore
namespace SilverlightMappingToolBasic.Controls
{
    public class CanvasContextMenu : ContextMenu
    {
        public CanvasContextMenu(TypeManager typeManger, LocalMessageSender sender, INodeNavigator navigator, Point location)
            : base()
        {
            TypeManager = typeManger;
            MessageSender = sender;
            Navigator = navigator;
            Location = location;

            this.Loaded += new RoutedEventHandler(CanvasContextMenu_Loaded);
        }

        private TypeManager TypeManager
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

        private Point Location
        {
            get;
            set;
        }


        private void AddAddNodeMenuItem(string nodeTypeName, RoutedEventHandler clickHandler)
        {
            MenuItem menuItem = new MenuItem() { Header = "\t" + nodeTypeName };
            menuItem.Click += clickHandler;
            this.Items.Add(menuItem);
        }

        private void CanvasContextMenu_Loaded(object sender, RoutedEventArgs e)
        {
            MenuItem addNodeMenuItem = new MenuItem();
            addNodeMenuItem.Header = new TextBlock() { Text = "Load Nodes", FontWeight = FontWeights.Bold };
            this.Items.Add(addNodeMenuItem);

            AddAddNodeMenuItem("Map Node", delegate(object sender2, RoutedEventArgs e2)
            {
                INodeTypeProxy nodeType = TypeManager.GetNodeType("CompendiumMapNode");
                Navigator.AddNode(nodeType, "", Location);
            });

            AddAddNodeMenuItem("Idea Node", delegate(object sender2, RoutedEventArgs e2)
            {
                INodeTypeProxy nodeType = TypeManager.GetNodeType("CompendiumIdeaNode");
                Navigator.AddNode(nodeType, "", Location);
            });

            AddAddNodeMenuItem("Question Node", delegate(object sender2, RoutedEventArgs e2)
            {
                INodeTypeProxy nodeType = TypeManager.GetNodeType("CompendiumQuestionNode");
                Navigator.AddNode(nodeType, "", Location);
            });

            AddAddNodeMenuItem("Pro Node", delegate(object sender2, RoutedEventArgs e2)
            {
                INodeTypeProxy nodeType = TypeManager.GetNodeType("CompendiumProNode");
                Navigator.AddNode(nodeType, "", Location);
            });

            AddAddNodeMenuItem("Con Node", delegate(object sender2, RoutedEventArgs e2)
            {
                INodeTypeProxy nodeType = TypeManager.GetNodeType("CompendiumConNode");
                Navigator.AddNode(nodeType, "", Location);
            });

           AddAddNodeMenuItem("Decision Node", delegate(object sender2, RoutedEventArgs e2)
            {
                INodeTypeProxy nodeType = TypeManager.GetNodeType("CompendiumDecisionNode");
                Navigator.AddNode(nodeType, "", Location);
            });

            Separator seperator1 = new Separator();
            this.Items.Add(seperator1);

            MenuItem pasteMenuItem = new MenuItem();
            pasteMenuItem.Header = "Paste";
            pasteMenuItem.Click += new RoutedEventHandler(pasteMenuItem_Click);
            this.Items.Add(pasteMenuItem);

            Separator seperator2 = new Separator();
            this.Items.Add(seperator2);

            MenuItem assignVideoMenuItem = new MenuItem();
            assignVideoMenuItem.Header = "Choose Video...";
            assignVideoMenuItem.Click += new RoutedEventHandler(assignVideoMenuItem_Click);
            this.Items.Add(assignVideoMenuItem);
        }

        private void assignVideoMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AssignVideoDialog videoDialog = new AssignVideoDialog();
            videoDialog.Closed += new EventHandler(videoDialog_Closed);
            videoDialog.Show();
        }

        private void pasteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            string clipboardText = Clipboard.GetText();
            using (XmlReader reader = XmlReader.Create(new StringReader(clipboardText)))
            {
                try
                {
                    // Parse the file and display each of the nodes.
                    while (reader.ReadToFollowing("Node"))
                    {
                        reader.MoveToAttribute("Id");
                        string nodeId = reader.Value;
                        Guid copiedGuid = Guid.Empty;

                        //if it's a valid Guid do the operation
                        if (Guid.TryParse(nodeId, out copiedGuid))
                        {
                            //TODO: Do Paste operation which creates a reference copy (shortcut) of the copied node in the same map
                            TypeManager typeManager = IoC.IoCContainer.GetInjectionInstance().GetInstance<TypeManager>();
                            IDescriptorTypeProxy toDescriptorTypeProxy = typeManager.GetDescriptorType("To");
                            IDescriptorTypeProxy transMapDescriptorTypeProxy = typeManager.GetDescriptorType("TransclusionMap");

                            Dictionary<IDescriptorTypeProxy, Guid> nodes = new Dictionary<IDescriptorTypeProxy, Guid>();
                            nodes.Add(transMapDescriptorTypeProxy, Navigator.FocalNodeId);
                            nodes.Add(toDescriptorTypeProxy, copiedGuid);

                            IRelationshipTypeProxy relationshipTypeProxy = typeManager.GetRelationshipType("TransclusionRelationship");

                            Navigator.ConnectNodesAsync(nodes, relationshipTypeProxy, Location, string.Empty);
                        }
                    }
                }
                catch (XmlException)
                {
                    //safe to ignore if it's not valid XML
                }
            }
        }

        private void videoDialog_Closed(object sender, EventArgs e)
        {
            AssignVideoDialog videoDialog = sender as AssignVideoDialog;
            if (videoDialog.DialogResult.Value)
            {
                RelatedContentPanelUtil.Instance.LoadRelatedVideoContent(videoDialog.Source, VideoSize.Small);

                string source = videoDialog.Source;
                Command setSourceCommand = new Command();
                setSourceCommand.Name = "Play";
                setSourceCommand.Params = new List<Param>();
                setSourceCommand.Params.Add(new Param() { Name = "Source", Value = source });
                setSourceCommand.Params.Add(new Param() { Name = "StartTimeCode", Value = "00:00:00" });
                //Utilities.SendMessage<Command>(MessageSender, setSourceCommand);
            }
        }
    }
}
