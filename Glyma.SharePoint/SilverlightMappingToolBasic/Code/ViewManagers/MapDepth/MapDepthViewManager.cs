using System;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Linq;
using SilverlightMappingToolBasic.MappingService;
using SilverlightMappingToolBasic.Controls;
using System.Windows.Browser;

namespace SilverlightMappingToolBasic.MapDepth
{
    public enum MapDepthViewManagerUpdateOperations
    {
        Create,
        Read,
        Update,
        Delete,
        CreateOrUpdate,
        Clear
    }

    public class MapTransaction
    {
        public MapTransaction()
        {
            Node = null;
            NodeId = Guid.Empty;
            Relationship = null;
            RelationshipId = Guid.Empty;
        }

        public MapDepthViewManagerUpdateOperations Operation
        {
            get;
            set;
        }

        public Guid NodeId
        {
            get;
            set;
        }

        public INodeProxy Node
        {
            get;
            set;
        }

        public Guid RelationshipId
        {
            get;
            set;
        }

        public IRelationshipProxy Relationship
        {
            get;
            set;
        }
    }

    public class NodeTransactionCollection : Queue<MapTransaction>
    {
        public NodeTransactionCollection()
            : base()
        {
        }

        public NodeTransactionCollection(int capacity)
            : base(capacity)
        {
        }

        public NodeTransactionCollection(IEnumerable<MapTransaction> collection)
            : base(collection)
        {
        }
    }

    public class MapDepthViewManager
    {
        //private MapDepthNavigator _navigator;
        //private NavigatorView _currentView;
        //private NavigatorViewCollection _cachedViews;

        public event EventHandler NodesUpdated;
        private Guid _lastAddedNode;

        public MapDepthViewManager(MapDepthNavigator navigator, ThemeManager themeManagementObject)
        {
            Navigator = navigator;
            Navigator.AddNodeCompleted += new EventHandler<AddedNodeEventArgs>(OnAddNodeCompleted);
            Navigator.PasteNodeCloneCompleted += new EventHandler<NodesEventArgs>(OnPasteNodeCloneCompleted);
            Navigator.ConnectNodesCompleted += new EventHandler<NodesEventArgs>(OnConnectNodesCompleted);
            Navigator.GetCurrentNodesCompleted += new EventHandler<NodesEventArgs>(OnGetCurrentNodesCompleted);
            Navigator.UpdateNodeMetadataCompleted += new EventHandler<NodesEventArgs>(OnUpdateNodeMetadataCompleted);
            Navigator.DeleteNodeCompleted += new EventHandler<NodesEventArgs>(OnDeleteNodeCompleted);
            Navigator.DeleteRelationshipCompleted += new EventHandler<NodesEventArgs>(OnDeleteRelationshipCompleted);

            UnactionedNodeActions = new NodeTransactionCollection();
            ThemeManagementObject = themeManagementObject;
        }

        private void OnDeleteRelationshipCompleted(object sender, NodesEventArgs e)
        {
            if (CurrentView != null)
            {
                MapTransaction transaction = new MapTransaction();
                transaction.Operation = MapDepthViewManagerUpdateOperations.Delete;
                transaction.RelationshipId = e.ContextRelationshipId;

                UnactionedNodeActions.Enqueue(transaction);
            }

            OnNodesUpdated();
        }

        private void OnAddNodeCompleted(object sender, AddedNodeEventArgs e)
        {
            _lastAddedNode = e.Node.Id;
        }

        private void OnPasteNodeCloneCompleted(object sender, NodesEventArgs e)
        {
            MapTransaction transaction = new MapTransaction();
            transaction.Operation = MapDepthViewManagerUpdateOperations.CreateOrUpdate;
            transaction.Node = e.ContextNode;

            UnactionedNodeActions.Enqueue(transaction);

            OnNodesUpdated();
        }

        private void OnConnectNodesCompleted(object sender, NodesEventArgs e)
        {
            foreach (INodeProxy node in e.Nodes)
            {
                MapTransaction transaction = new MapTransaction();
                transaction.Operation = MapDepthViewManagerUpdateOperations.CreateOrUpdate;
                transaction.Node = node;

                UnactionedNodeActions.Enqueue(transaction);
            }

            OnNodesUpdated();
        }

        private void OnGetCurrentNodesCompleted(object sender, NodesEventArgs e)
        {
            if (CurrentView == null || CurrentView.ContextNode.Id != e.ContextNode.Id)
            {
                CurrentView = new NavigatorView();

                MapTransaction transaction = new MapTransaction();
                transaction.Operation = MapDepthViewManagerUpdateOperations.Clear;
                transaction.Node = e.ContextNode;

                UnactionedNodeActions.Enqueue(transaction);
            }

            foreach (INodeProxy node in e.Nodes)
            {
                if (!CurrentView.NodeRenderers.ContainsKey(node.Id))
                {
                    MapTransaction transaction = new MapTransaction();
                    transaction.Operation = MapDepthViewManagerUpdateOperations.CreateOrUpdate;
                    transaction.Node = node;

                    UnactionedNodeActions.Enqueue(transaction);
                }
            }

            OnNodesUpdated();
        }

        private void OnUpdateNodeMetadataCompleted(object sender, NodesEventArgs e)
        {
            foreach (INodeProxy node in e.Nodes)
            {
                MapTransaction transaction = new MapTransaction();
                transaction.Operation = MapDepthViewManagerUpdateOperations.Update;
                transaction.Node = node;
                transaction.NodeId = node.Id;

                UnactionedNodeActions.Enqueue(transaction);
            }

            OnNodesUpdated();
        }

        private void OnDeleteNodeCompleted(object sender, NodesEventArgs e)
        {
            MapTransaction transaction = new MapTransaction();
            transaction.Operation = MapDepthViewManagerUpdateOperations.Delete;
            transaction.NodeId = e.ContextNodeId;

            UnactionedNodeActions.Enqueue(transaction);

            OnNodesUpdated();
        }

        public List<NodeRenderer> Nodes
        {
            get
            {
                return CurrentView.NodeRenderers.Values.ToList();
            }
        }

        public List<RelationshipRenderer> Relationships
        {
            get
            {
                List<RelationshipRenderer> results = null;
                if (CurrentView != null)
                {
                    results = CurrentView.RelationshipRenderers.Values.ToList();
                }
                return results;
            }
        }

        public ThemeManager ThemeManagementObject
        {
            get;
            protected set;
        }

        public MapDepthNavigator Navigator
        {
            get;
            protected set;
        }

        public NavigatorView CurrentView
        {
            get;
            protected set;
        }

        public NodeTransactionCollection UnactionedNodeActions
        {
            get;
            protected set;
        }


        public INodeRenderer GetNodeRenderer(Point location)
        {
            INodeRenderer result = null;
            foreach (NodeRenderer nodeRenderer in Nodes)
            {
                if (location.X > nodeRenderer.Location.X && location.X < (nodeRenderer.Location.X + nodeRenderer.Skin.NodeSkinWidth)
                    && location.Y > nodeRenderer.Location.Y && location.Y < (nodeRenderer.Location.Y + nodeRenderer.Skin.NodeSkinHeight))
                {
                    result = nodeRenderer;
                    break;
                }
            }
            return result;
        }

        public void SelectAllWithinBounds(Point TopLeft, Point BottomRight)
        {
            foreach (INodeRenderer nodeRenderer in Nodes)
            {
                if (nodeRenderer.Location.X > TopLeft.X && nodeRenderer.Location.X < BottomRight.X
                    && nodeRenderer.Location.Y > TopLeft.Y && nodeRenderer.Location.Y < BottomRight.Y)
                {
                    if (!nodeRenderer.IsSelected)
                    {
                        nodeRenderer.IsSelected = true;
                    }
                }
                else
                {
                    if (Keyboard.Modifiers != ModifierKeys.Shift)
                    {
                        if (nodeRenderer.IsSelected)
                        {
                            nodeRenderer.IsSelected = false;
                        }
                    }
                }
            }

            foreach (RelationshipRenderer relationshipRenderer in Relationships)
            {
                if (IntersectionTest(relationshipRenderer.Arrow, BottomRight.X, BottomRight.Y, TopLeft.X, TopLeft.Y))
                {
                    if (!relationshipRenderer.IsSelected)
                    {
                        relationshipRenderer.IsSelected = true;
                    }
                }
                else
                {
                    if (Keyboard.Modifiers != ModifierKeys.Shift)
                    {
                        if (relationshipRenderer.IsSelected)
                        {
                            relationshipRenderer.IsSelected = false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Tests if an arrow passes through a rectangle (the selection rectangle).
        /// It does this by using a sweeping algorithm moving along the arrow from start to finish until it hits the rectangle,
        /// this was simpler to implement than one that did bounds testing of conditions and can be more efficient in most selection cases
        /// where the selection rectangle encases the start position.
        /// </summary>
        /// <param name="arrow">The arrow being tested</param>
        /// <param name="maxX">X coordinate of the the bottom right corner of the rectangle</param>
        /// <param name="maxY">Y coordinate of the the bottom right corner of the rectangle</param>
        /// <param name="minX">X coordinate of the the top left corner of the rectangle</param>
        /// <param name="minY">Y coordinate of the the top left corner of the rectangle</param>
        /// <returns>True if the arrow intersects with the rectangle</returns>
        private bool IntersectionTest(ArrowControl arrow, double maxX, double maxY, double minX, double minY)
        {
            bool result = false;
            switch (arrow.Direction)
            {
                case ArrowDirection.North:
                    if (minY <= arrow.EndPoint.Y &&
                        maxY >= arrow.StartPoint.Y &&
                        minX <= arrow.EndPoint.X &&
                        maxX >= arrow.EndPoint.X)
                    {
                        result = true;
                    }
                    break;
                case ArrowDirection.South:
                    if (minY <= arrow.EndPoint.Y &&
                        maxY >= arrow.StartPoint.Y &&
                        minX <= arrow.EndPoint.X &&
                        maxX >= arrow.EndPoint.X)
                    {
                        result = true;
                    }
                    break;
                case ArrowDirection.West:
                    if (minY <= arrow.EndPoint.Y &&
                        maxY >= arrow.StartPoint.Y &&
                        minX <= arrow.EndPoint.X &&
                        maxX >= arrow.EndPoint.X)
                    {
                        result = true;
                    }
                    break;
                case ArrowDirection.East:
                    if (minY <= arrow.EndPoint.Y &&
                        maxY >= arrow.StartPoint.Y &&
                        minX <= arrow.EndPoint.X &&
                        maxX >= arrow.EndPoint.X)
                    {
                        result = true;
                    }
                    break;
                case ArrowDirection.NorthWest:
                    if (minX < arrow.StartPoint.X && maxX > arrow.EndPoint.X)
                    {
                        for (double x = 0, y = 0, intX = arrow.EndPoint.X, intY = 0; intX >= arrow.EndPoint.X; x--)
                        {
                            y = x / arrow.Coeffiencient;
                            intX = arrow.StartPoint.X + x;
                            intY = arrow.StartPoint.Y + y;
                            if (intX <= maxX && intX >= minX &&
                                intY >= minY && intY <= maxY)
                            {
                                result = true;
                                break;
                            }
                        }
                    }
                    break;
                case ArrowDirection.NorthEast:
                    if (minX < arrow.EndPoint.X && maxX > arrow.StartPoint.X)
                    {
                        for (double x = 0, y = 0, intX = arrow.StartPoint.X, intY = 0; intX <= arrow.EndPoint.X; x++)
                        {
                            y = x / arrow.Coeffiencient;
                            intX = arrow.StartPoint.X + x;
                            intY = arrow.StartPoint.Y - y;
                            if (intX <= maxX && intX >= minX &&
                                intY >= minY && intY <= maxY)
                            {
                                result = true;
                                break;
                            }
                        }
                    }
                    break;
                case ArrowDirection.SouthWest:
                    if (minX < arrow.StartPoint.X && maxX > arrow.EndPoint.X)
                    {
                        for (double x = 0, y = 0, intX = arrow.StartPoint.X, intY = 0; intX >= arrow.EndPoint.X; x--)
                        {
                            y = x / arrow.Coeffiencient;
                            intX = arrow.StartPoint.X + x;
                            intY = arrow.StartPoint.Y - y;
                            if (intX <= maxX && intX >= minX &&
                                intY >= minY && intY <= maxY)
                            {
                                result = true;
                                break;
                            }
                        }
                    }
                    break;
                case ArrowDirection.SouthEast:
                    if (minX < arrow.EndPoint.X && maxX > arrow.StartPoint.X)
                    {
                        for (double x = 0, y = 0, intX = arrow.StartPoint.X, intY = 0; intX <= arrow.EndPoint.X; x++)
                        {
                            y = x / arrow.Coeffiencient;
                            intX = arrow.StartPoint.X + x;
                            intY = arrow.StartPoint.Y + y;
                            if (intX <= maxX && intX >= minX &&
                                intY >= minY && intY <= maxY)
                            {
                                result = true;
                                break;
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
            return result;
        }

        public void UnselectAllNodes()
        {
            foreach (INodeRenderer nodeRenderer in Nodes)
            {
                if (nodeRenderer.IsSelected)
                {
                    nodeRenderer.IsSelected = false;
                }
            }
        }

        public void UnselectAllRelationships()
        {
            foreach (IRelationshipRenderer relationshipRenderer in Relationships)
            {
                if (relationshipRenderer.IsSelected)
                {
                    relationshipRenderer.IsSelected = false;
                }
            }
        }

        protected void OnNodePositionUpdating(object sender, EventArgs e)
        {
            NodeRenderer nodeRenderer = (NodeRenderer)sender;

            if (nodeRenderer != null)
            {
                double xOffset = 0;
                double yOffset = 0;

                Point newLocation = nodeRenderer.Location;
                StoreNodeLocationMetadata(nodeRenderer, ref xOffset, ref yOffset, newLocation);

                CurrentView.RelationshipRenderers.UpdateRelationship(nodeRenderer);

                //foreach selected node exclusing the one triggering this event
                foreach (NodeRenderer selectedNode in 
                    CurrentView.NodeRenderers.Values.Where(nr => nr.IsSelected == true 
                                                             && nr.Node.Id != nodeRenderer.Node.Id)) 
                {
                    Point selectionsNewLocation = new Point(selectedNode.Location.X + xOffset, 
                                                                 selectedNode.Location.Y + yOffset);
                    selectedNode.MoveNode(selectionsNewLocation, false);
                    CurrentView.RelationshipRenderers.UpdateRelationship(selectedNode);
                    double u1 = 0, u2 = 0;
                    StoreNodeLocationMetadata(selectedNode, ref u1, ref u2, selectionsNewLocation);
                }
            }
        }

        private void StoreNodeLocationMetadata(NodeRenderer nodeRenderer, ref double xOffset, ref double yOffset, Point newLocation)
        {
            TypeManager typeManager = IoC.IoCContainer.GetInjectionInstance().GetInstance<TypeManager>();
            MetadataTypeProxy metaDataTypeProxy = typeManager.GetMetadataType("double") as MetadataTypeProxy;

            INodeProxy nodeProxy = nodeRenderer.Node;

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
            IRelationshipProxy relationshipProxy = null;

            //If the node is being drawn in a map that isn't it's owner it is a transclusion of the node we are managing
            if (nodeProxy.ParentMapNodeUid != CurrentView.ContextNode.Id)
            {
                descriptorType = IoC.IoCContainer.GetInjectionInstance().GetInstance<TypeManager>().GetDescriptorType("TransclusionMap");
                xPosKey.DescriptorTypeUid = descriptorType.Id;
                yPosKey.DescriptorTypeUid = descriptorType.Id;

                foreach (IRelationshipProxy relationship in GetTransclusionRelationship(nodeProxy))
                {
                    xPosKey.RelationshipUid = relationship.Id;
                    yPosKey.RelationshipUid = relationship.Id;

                    relationshipProxy = relationship;

                    if (nodeProxy.Metadata != null && nodeProxy.HasMetadata(xPosKey) && nodeProxy.GetNodeMetadata(xPosKey) != null)
                    {
                        xOffset = Double.Parse(nodeRenderer.Node.GetNodeMetadata(xPosKey).MetadataValue);
                        xOffset = newLocation.X - xOffset;
                        nodeRenderer.Node.GetNodeMetadata(xPosKey).MetadataValue = newLocation.X.ToString();
                    }
                    else
                    {
                        if (metaDataTypeProxy != null)
                        {
                            SoapMetadata soapMetadata = new SoapMetadata();
                            soapMetadata.MetadataName = "XPosition";
                            soapMetadata.MetadataType = metaDataTypeProxy.BaseSoapNodeType;
                            soapMetadata.MetadataValue = newLocation.X.ToString();
                            nodeRenderer.Node.Metadata.Add(xPosKey, soapMetadata);
                        }
                    }

                    if (nodeProxy.Metadata != null && nodeProxy.HasMetadata(yPosKey) && nodeProxy.GetNodeMetadata(yPosKey) != null)
                    {
                        yOffset = Double.Parse(nodeRenderer.Node.GetNodeMetadata(yPosKey).MetadataValue);
                        yOffset = newLocation.Y - yOffset;
                        nodeRenderer.Node.GetNodeMetadata(yPosKey).MetadataValue = newLocation.Y.ToString();
                    }
                    else
                    {
                        if (metaDataTypeProxy != null)
                        {
                            SoapMetadata soapMetadata = new SoapMetadata();
                            soapMetadata.MetadataName = "YPosition";
                            soapMetadata.MetadataType = metaDataTypeProxy.BaseSoapNodeType;
                            soapMetadata.MetadataValue = newLocation.Y.ToString();
                            nodeRenderer.Node.Metadata.Add(yPosKey, soapMetadata);
                        }
                    }
                }
            }
            else
            {
                //The node wasn't a transclusion as it was being rendered within its original map context
                relationshipProxy = GetMapRelationship(nodeProxy);

                descriptorType = IoC.IoCContainer.GetInjectionInstance().GetInstance<TypeManager>().GetDescriptorType("From");
                xPosKey.DescriptorTypeUid = descriptorType.Id;
                yPosKey.DescriptorTypeUid = descriptorType.Id;

                if (relationshipProxy != null)
                {
                    xPosKey.RelationshipUid = relationshipProxy.Id;
                    yPosKey.RelationshipUid = relationshipProxy.Id;
                }

                if (nodeProxy.Metadata != null && nodeProxy.HasMetadata(xPosKey) && nodeProxy.GetNodeMetadata(xPosKey) != null)
                {
                    xOffset = Double.Parse(nodeRenderer.Node.GetNodeMetadata(xPosKey).MetadataValue);
                    xOffset = newLocation.X - xOffset;
                    nodeRenderer.Node.GetNodeMetadata(xPosKey).MetadataValue = newLocation.X.ToString();
                }
                else
                {
                    if (metaDataTypeProxy != null)
                    {
                        SoapMetadata soapMetadata = new SoapMetadata();
                        soapMetadata.MetadataName = "XPosition";
                        soapMetadata.MetadataType = metaDataTypeProxy.BaseSoapNodeType;
                        soapMetadata.MetadataValue = newLocation.X.ToString();
                        nodeRenderer.Node.Metadata.Add(xPosKey, soapMetadata);
                    }
                }

                if (nodeProxy.Metadata != null && nodeProxy.HasMetadata(yPosKey) && nodeProxy.GetNodeMetadata(yPosKey) != null)
                {
                    yOffset = Double.Parse(nodeRenderer.Node.GetNodeMetadata(yPosKey).MetadataValue);
                    yOffset = newLocation.Y - yOffset;
                    nodeRenderer.Node.GetNodeMetadata(yPosKey).MetadataValue = newLocation.Y.ToString();
                }
                else
                {
                    if (metaDataTypeProxy != null)
                    {
                        SoapMetadata soapMetadata = new SoapMetadata();
                        soapMetadata.MetadataName = "YPosition";
                        soapMetadata.MetadataType = metaDataTypeProxy.BaseSoapNodeType;
                        soapMetadata.MetadataValue = newLocation.Y.ToString();
                        nodeRenderer.Node.Metadata.Add(yPosKey, soapMetadata);
                    }
                }
            }
        }

        protected void OnNodePositionUpdated(object sender, EventArgs e)
        {
            NodeRenderer nodeRenderer = (NodeRenderer)sender;
            StoreNodePosition(nodeRenderer);

            //foreach selected node exclusing the one triggering this event
            foreach (NodeRenderer selectedNode in
                CurrentView.NodeRenderers.Values.Where(nr => nr.IsSelected == true
                                                         && nr.Node.Id != nodeRenderer.Node.Id))
            {
                StoreNodePosition(selectedNode);
            }
        }

        private void StoreNodePosition(NodeRenderer nodeRenderer)
        {
            if (nodeRenderer != null)
            {
                TypeManager typeManager = IoC.IoCContainer.GetInjectionInstance().GetInstance<TypeManager>();
                IMetadataTypeProxy doubleMetadataTypeProxy = typeManager.GetMetadataType("double");
                
                INodeProxy nodeProxy = nodeRenderer.Node;

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
                IRelationshipProxy relationshipProxy = null;
                if (nodeProxy.ParentMapNodeUid != CurrentView.ContextNode.Id)
                {
                    descriptorType = IoC.IoCContainer.GetInjectionInstance().GetInstance<TypeManager>().GetDescriptorType("TransclusionMap");
                    xPosKey.DescriptorTypeUid = descriptorType.Id;
                    yPosKey.DescriptorTypeUid = descriptorType.Id;

                    foreach (IRelationshipProxy relationship in GetTransclusionRelationship(nodeProxy))
                    {
                        xPosKey.RelationshipUid = relationship.Id;
                        yPosKey.RelationshipUid = relationship.Id;

                        relationshipProxy = relationship;

                        if (nodeProxy.Metadata != null && nodeProxy.HasMetadata(xPosKey) && nodeProxy.HasMetadata(yPosKey))
                        {
                            Navigator.UpdateNodeMetadataAsync(nodeRenderer.Node, relationshipProxy.Id, descriptorType, "YPosition", nodeProxy.GetNodeMetadata(yPosKey).MetadataValue, doubleMetadataTypeProxy);
                            Navigator.UpdateNodeMetadataAsync(nodeRenderer.Node, relationshipProxy.Id, descriptorType, "XPosition", nodeProxy.GetNodeMetadata(xPosKey).MetadataValue, doubleMetadataTypeProxy);
                        }
                    }
                }
                else
                {
                    relationshipProxy = GetMapRelationship(nodeProxy);

                    descriptorType = IoC.IoCContainer.GetInjectionInstance().GetInstance<TypeManager>().GetDescriptorType("From");
                    xPosKey.DescriptorTypeUid = descriptorType.Id;
                    yPosKey.DescriptorTypeUid = descriptorType.Id;

                    if (relationshipProxy != null)
                    {
                        xPosKey.RelationshipUid = relationshipProxy.Id;
                        yPosKey.RelationshipUid = relationshipProxy.Id;
                    }

                    Navigator.UpdateNodeMetadataAsync(nodeRenderer.Node, relationshipProxy.Id, descriptorType, "YPosition", nodeProxy.GetNodeMetadata(yPosKey).MetadataValue, doubleMetadataTypeProxy);
                    Navigator.UpdateNodeMetadataAsync(nodeRenderer.Node, relationshipProxy.Id, descriptorType, "XPosition", nodeProxy.GetNodeMetadata(xPosKey).MetadataValue, doubleMetadataTypeProxy);
                }
            }
        }

        protected void OnNodeDoubleClicked(object sender, NodeClickedArgs e)
        {
            if (e.Node.NodeType.Name == "CompendiumMapNode")
            {
                Navigator.SetCurrentNode(e.Node);
                Navigator.GetCurrentNodesAsync();
            }
            else if (e.Node.NodeType.Name == "CompendiumReferenceNode")
            {
                //call a javascript funtion that opens a window displaying the referenced file in the document library.
                MetadataContext metaDataContext = new MetadataContext() { MetadataName = "LinkedFile.Source", NodeUid = e.Node.Id };
                if (e.Node.HasMetadata(metaDataContext) && !string.IsNullOrEmpty(e.Node.GetNodeMetadata(metaDataContext).MetadataValue))
                {
                    string documentUrl = e.Node.GetNodeMetadata(metaDataContext).MetadataValue;
                    HtmlPage.Window.Invoke("openDocument", documentUrl);
                }
            }
        }

        public void CommitNodeName()
        {
            if (CurrentView != null)
            {
                foreach (NodeRenderer nodeRenderer in CurrentView.NodeRenderers.Values) 
                {
                    nodeRenderer.CommitNodeName();
                }
            }
        }

        public void UpdateSurface(IMapControl mapSurface, RenderingContextInfo contextInfo)
        {
            while (UnactionedNodeActions.Count > 0)
            {
                MapTransaction nodeTransaction = UnactionedNodeActions.Dequeue();

                switch (nodeTransaction.Operation)
                {
                    case MapDepthViewManagerUpdateOperations.Create:
                        goto case MapDepthViewManagerUpdateOperations.CreateOrUpdate;

                    case MapDepthViewManagerUpdateOperations.Read:
                        break;

                    case MapDepthViewManagerUpdateOperations.Update:
                        {
                            if (CurrentView != null) 
                            {
                                if (CurrentView.NodeRenderers.ContainsKey(nodeTransaction.NodeId))
                                {
                                    NodeRenderer nodeRenderer = CurrentView.NodeRenderers[nodeTransaction.Node.Id];
                                    nodeRenderer.Node = nodeTransaction.Node;
                                    if (nodeRenderer.Node.ParentMapNodeUid != Navigator.FocalNodeId)
                                    {
                                        nodeRenderer.Node.IsTransclusion = true;
                                    }
                                    else
                                    {
                                        nodeRenderer.Node.IsTransclusion = false;
                                    }

                                    nodeRenderer.Refresh();
                                }
                            }
                        }
                        break;

                    case MapDepthViewManagerUpdateOperations.Delete:
                        {
                            if (nodeTransaction.NodeId != Guid.Empty)
                            {
                                if (CurrentView.NodeRenderers.ContainsKey(nodeTransaction.NodeId))
                                {
                                    NodeRenderer removedNodeRenderer = CurrentView.NodeRenderers[nodeTransaction.NodeId];
                                    INodeProxy removedNode = removedNodeRenderer.Node;
                                    mapSurface.MapSurface.Children.Remove(removedNodeRenderer);
                                    CurrentView.NodeRenderers.Remove(nodeTransaction.NodeId);

                                    IList<Guid> removedRelationships = new List<Guid>();
                                    foreach (RelationshipRenderer relationshipRenderer in CurrentView.RelationshipRenderers.Values)
                                    {
                                        IRelationshipProxy relationship = relationshipRenderer.Relationship;
                                        foreach (IDescriptorProxy descriptor in relationship.Descriptors)
                                        {
                                            if (descriptor.NodeId == nodeTransaction.NodeId)
                                            {
                                                removedRelationships.Add(relationship.Id);
                                            }
                                        }
                                    }
                                    foreach (Guid relationshipId in removedRelationships)
                                    {
                                        if (CurrentView.RelationshipRenderers.ContainsKey(relationshipId))
                                        {
                                            RelationshipRenderer relationshipRenderer = CurrentView.RelationshipRenderers[relationshipId];
                                            mapSurface.MapSurface.Children.Remove(relationshipRenderer);
                                            CurrentView.RelationshipRenderers.Remove(relationshipId);
                                        }
                                        foreach (NodeRenderer nodeRenderer in CurrentView.NodeRenderers.Values)
                                        {
                                            IDescriptorProxy descriptorToRemove = null;
                                            foreach (IDescriptorProxy descriptor in nodeRenderer.Node.Descriptors)
                                            {
                                                if (nodeRenderer.Node.Id != nodeTransaction.NodeId)
                                                {
                                                    if (descriptor.Relationship.Id == relationshipId)
                                                    {
                                                        descriptorToRemove = descriptor;
                                                    }
                                                }
                                            }
                                            if (descriptorToRemove != null)
                                            {
                                                nodeRenderer.Node.Descriptors.Remove(descriptorToRemove);
                                            }
                                        }
                                    }
                                }
                            }
                            if (nodeTransaction.RelationshipId != Guid.Empty)
                            {
                                if (CurrentView.RelationshipRenderers.ContainsKey(nodeTransaction.RelationshipId))
                                {
                                    RelationshipRenderer removedRelationshipRenderer = CurrentView.RelationshipRenderers[nodeTransaction.RelationshipId];
                                    IRelationshipProxy removedRelationship = removedRelationshipRenderer.Relationship;
                                    mapSurface.MapSurface.Children.Remove(removedRelationshipRenderer);
                                    CurrentView.RelationshipRenderers.Remove(nodeTransaction.RelationshipId);

                                    foreach (IDescriptorProxy descriptor in removedRelationship.Descriptors)
                                    {
                                        if (descriptor.Relationship.RelationshipType.Name != "MapContainerRelationship" &&
                                            descriptor.Relationship.RelationshipType.Name != "TransclusionRelationship")
                                        {
                                            if (CurrentView.NodeRenderers.ContainsKey(descriptor.NodeId))
                                            {
                                                NodeRenderer nodeRenderer = CurrentView.NodeRenderers[descriptor.NodeId];
                                                if (nodeRenderer != null && nodeRenderer.Node.Descriptors.Contains(descriptor))
                                                {
                                                    nodeRenderer.Node.Descriptors.Remove(descriptor);
                                                }
                                            }
                                        }
                                        if (descriptor.Relationship.RelationshipType.Name == "TransclusionRelationship")
                                        {
                                            if (descriptor.DescriptorType.Name != "TransclusionMap" && CurrentView.NodeRenderers.ContainsKey(descriptor.NodeId))
                                            {
                                                NodeRenderer nodeRenderer = CurrentView.NodeRenderers[descriptor.NodeId];
                                                if (nodeRenderer != null && nodeRenderer.Node.Descriptors.Contains(descriptor))
                                                {
                                                    nodeRenderer.Node.Descriptors.Remove(descriptor);
                                                }
                                            }
                                            else
                                            {
                                                if (descriptor.Node != null && descriptor.Node.Descriptors.Contains(descriptor))
                                                {
                                                    descriptor.Node.Descriptors.Remove(descriptor);
                                                }
                                            }
                                        }
                                        //else if (descriptor.Relationship.RelationshipType.Name == "MapContainerRelationship")
                                        //{
                                        //    if (CurrentView.NodeRenderers.ContainsKey(descriptor.NodeId))
                                        //    {
                                        //        NodeRenderer nodeRenderer = CurrentView.NodeRenderers[descriptor.NodeId];
                                        //        CurrentView.NodeRenderers.Remove(descriptor.NodeId);
                                        //        mapSurface.MapSurface.Children.Remove(nodeRenderer);
                                        //    }
                                        //}
                                    }
                                }
                                else
                                {
                                    Guid nodeRemoved = Guid.Empty;

                                    //the relationship isn't a visible one but nodes hanging off it will be
                                    foreach (INodeRenderer nodeRenderer in CurrentView.NodeRenderers.Values)
                                    {
                                        INodeProxy node = nodeRenderer.Node;
                                        
                                        //test if node is transcluded - transcluded nodes are only held by their relationship to the map
                                        if (node.ParentMapNodeUid != CurrentView.ContextNode.Id)
                                        {
                                            foreach (IDescriptorProxy descriptor in node.Descriptors)
                                            {
                                                if (descriptor.Relationship.Id == nodeTransaction.RelationshipId &&
                                                    descriptor.Relationship.RelationshipType.Name == "TransclusionRelationship")
                                                {
                                                    foreach (IDescriptorProxy transMapDescriptor in descriptor.Relationship.Descriptors.GetByDescriptorTypeName("TransclusionMap"))
                                                    {
                                                        if (transMapDescriptor.NodeId == CurrentView.ContextNode.Id)
                                                        {
                                                            foreach (IDescriptorProxy toDescriptor in descriptor.Relationship.Descriptors.GetByDescriptorTypeName("To"))
                                                            {
                                                                if (CurrentView.NodeRenderers.Keys.Contains(toDescriptor.NodeId))
                                                                {
                                                                    NodeRenderer removedTrancludedNode = CurrentView.NodeRenderers[toDescriptor.NodeId];
                                                                    nodeRemoved = toDescriptor.NodeId;
                                                                    mapSurface.MapSurface.Children.Remove(removedTrancludedNode);
                                                                    break;
                                                                }
                                                            }
                                                            break;
                                                        }
                                                    }
                                                    break;
                                                }
                                            }
                                        }
                                        if (nodeRemoved != Guid.Empty)
                                        {
                                            break; //removed the node already, there will only be one.
                                        }
                                    }
                                    if (nodeRemoved != Guid.Empty)
                                    {
                                        if (CurrentView.NodeRenderers.ContainsKey(nodeRemoved))
                                        {
                                            CurrentView.NodeRenderers.Remove(nodeRemoved);
                                        }
                                    }
                                }
                            }
                        }
                        break;

                    case MapDepthViewManagerUpdateOperations.CreateOrUpdate:
                        {
                            if (nodeTransaction.Node == null)
                            {
                                break;
                            }

                            if (CurrentView.NodeRenderers.ContainsKey(nodeTransaction.Node.Id))
                            {
                                mapSurface.MapSurface.Children.Remove(CurrentView.NodeRenderers[nodeTransaction.Node.Id]);
                                CurrentView.NodeRenderers.Remove(nodeTransaction.Node.Id);
                            }

                            NodeRenderer nodeRenderer = new MapDepthNodeRenderer(CurrentView, nodeTransaction.Node, ThemeManagementObject, "Default");
                            nodeRenderer.Context = contextInfo;

                            nodeRenderer.NodePositionUpdated += new EventHandler(OnNodePositionUpdated);
                            nodeRenderer.NodePositionUpdating += new EventHandler(OnNodePositionUpdating);
                            nodeRenderer.NodeDoubleClicked += new EventHandler<NodeClickedArgs>(OnNodeDoubleClicked);

                            mapSurface.MapSurface.Children.Add(nodeRenderer);
                            CurrentView.NodeRenderers.Add(nodeRenderer.Node.Id, nodeRenderer);
                            if (_lastAddedNode == nodeRenderer.Node.Id)
                            {
                                nodeRenderer.InEditState = true;
                                _lastAddedNode = Guid.Empty; //handle it once
                            }
                        }
                        break;

                    case MapDepthViewManagerUpdateOperations.Clear:
                        {
                            CurrentView.ContextNode = nodeTransaction.Node;
                            mapSurface.MapSurface.Children.Clear();
                        }
                        break;

                    default:
                        break;
                }
            }
            if (CurrentView != null)
            {
                foreach (NodeRenderer nodeRenderer in CurrentView.NodeRenderers.Values)
                {
                    foreach (IDescriptorProxy descriptor in nodeRenderer.Node.Descriptors)
                    {
                        if (descriptor.Relationship.RelationshipType.Name != "MapContainerRelationship")
                        {
                            if (!CurrentView.RelationshipRenderers.ContainsKey(descriptor.Relationship.Id))
                            {
                                bool relationshipIsPresent = true;

                                if (descriptor.Relationship.RelationshipType.Name == "TransclusionRelationship")
                                {
                                    if (descriptor.Relationship.Descriptors.Count == 2)
                                    {
                                        // the relationship isn't complete if both the To and From descriptors are not null, 
                                        // the TransclusionMap descriptor will be the second decriptor
                                        relationshipIsPresent = false;
                                    }
                                    else if (descriptor.Relationship.Descriptors.Count == 3)
                                    {
                                        foreach (IDescriptorProxy transMapDesc in descriptor.Relationship.Descriptors.GetByDescriptorTypeName("TransclusionMap"))
                                        {
                                            if (transMapDesc.NodeId != CurrentView.ContextNode.Id)
                                            {
                                                relationshipIsPresent = false;
                                                break;
                                            }
                                        }
                                    }
                                }

                                if (relationshipIsPresent)
                                {
                                    foreach (DescriptorProxy alternateDescriptor in descriptor.Relationship.Descriptors)
                                    {
                                        if (alternateDescriptor.DescriptorType.Name == "To" || alternateDescriptor.DescriptorType.Name == "From")
                                        {
                                            if (alternateDescriptor.Node != null && !CurrentView.NodeRenderers.ContainsKey(alternateDescriptor.NodeId))
                                            {
                                                relationshipIsPresent = false;
                                                break;
                                            }
                                            else
                                            {
                                                if (alternateDescriptor.Node == null &&
                                                    CurrentView.NodeRenderers.ContainsKey(alternateDescriptor.NodeId))
                                                {
                                                    alternateDescriptor.Node = CurrentView.NodeRenderers[alternateDescriptor.NodeId].Node;
                                                }
                                                if (alternateDescriptor.Node == null)
                                                {
                                                    relationshipIsPresent = false; //this cancels out the transclusion relationships on a node.
                                                }
                                                if (alternateDescriptor.Node != null && CurrentView.ContextNode.Id != alternateDescriptor.Node.ParentMapNodeUid
                                                    && descriptor.Node != null && CurrentView.ContextNode.Id != descriptor.Node.ParentMapNodeUid
                                                    && descriptor.Relationship.RelationshipType.Name != "TransclusionRelationship")
                                                {
                                                    relationshipIsPresent = false; // this cancels out relationships on maps the node has been transcluded to.
                                                }
                                            }
                                        }
                                    }
                                }

                                if (relationshipIsPresent)
                                {
                                    RelationshipRenderer arrow = new RelationshipRenderer(CurrentView, descriptor.Relationship);

                                    mapSurface.MapSurface.Children.Add(arrow);
                                    CurrentView.RelationshipRenderers.Add(descriptor.Relationship.Id, arrow);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void OnNodesUpdated()
        {
            if (NodesUpdated != null)
            {
                NodesUpdated.Invoke(this, null);
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
                        if (alternateDescriptor.NodeId == CurrentView.ContextNode.Id)
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
                        if (alternateDescriptor.NodeId == CurrentView.ContextNode.Id)
                        {
                            yield return descriptor.Relationship;
                        }
                    }
                }
            }
        }
    }
}
