using System;
using System.Globalization;
using System.Windows;
using SilverlightMappingToolBasic.GlymaSecurityService;
using SilverlightMappingToolBasic.UI.Extensions.Security;
using SilverlightMappingToolBasic.UI.SuperGraph.View.MessageBox;
using SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses;
using SilverlightMappingToolBasic.UI.SuperGraph.ViewModel;
using Proxy = TransactionalNodeService.Proxy;
using SilverlightMappingToolBasic.UI.ViewModel;
using System.Linq;
using System.Collections.Generic;
using TransactionFramework = TransactionalNodeService.Soap.TransactionFramework;
using SilverlightMappingToolBasic.UI.SuperGraph.Controller.Operations;
using System.ComponentModel;
using SilverlightMappingToolBasic.UI.Extensions.VideoWebPart;

namespace SilverlightMappingToolBasic.UI.SuperGraph.Controller
{
    public class SuperGraphController : CommonSuperGraphBase, IMapController<Node, Relationship>, ISuperGraphNodeFactory, ISuperGraphRelationshipFactory, ISuperGraphNodeBatchOperations, ISuperGraphNodeOperations
    {
        private readonly SecurityManager _securityManager;

        private VideoController _videoController;

        public SuperGraphController(Proxy.IMapManager mapManager, SecurityManager securityManager, ThemeManager themeManager, string videoSource): base(mapManager)
        {
            _securityManager = securityManager;
            ThemeManager = themeManager;
            CurrentVideoSource = videoSource;
        }

        public event EventHandler<ModelChangedEventArgs<Node, Relationship>> ModelChanged;
        public event EventHandler MapLoadCompleted;
        public event EventHandler<TransclusionLoadEventArgs> TransclusionsLoadCompleted;
        public event EventHandler<UserPermissionEventArgs> PermissionLoaded;

        private string CurrentVideoSource
        {
            get;
            set;
        }

        public bool IsSameDomain { get; set; }

        public INode Context
        {
            get;
            private set;
        }

        public Guid DomainId
        {
            get
            {
                return Context.DomainId;
            }
        }

        public ThemeManager ThemeManager
        {
            get;
            private set;
        }

        public IMap ActiveModel
        {
            get;
            private set;
        }

        public VideoController VideoController
        {
            get
            {
                if (_videoController == null)
                {
                    _videoController = new VideoController();
                    _videoController.Initialised();

                    if (!string.IsNullOrEmpty(CurrentVideoSource))
                    {
                        _videoController.VideoSource = CurrentVideoSource;
                        _videoController.DefaultVideoSource = CurrentVideoSource;
                        _videoController.SendSetSourceCommand(Context.Proxy);
                    }
                }

                return _videoController;
            }
        }

        public void RefreshAsync()
        {
            throw new NotImplementedException();
        }

        public void ChangeContextAsync(IMapObject mapObject)
        {
            if (mapObject is INode)
            {
                var node = (INode)mapObject;

                Context = node;
                LoadMap(Context);
            }
        }


        public void ChangeContextAsync(Proxy.INode mapObject)
        {
            var node = new Node(MapManager);
            node.LoadNode(null, mapObject);

            node.PropertyChanged += ViewModelNode_PropertyChanged;
            node.LocationChanged += ViewModelNode_LocationChanged;


            Context = node;

            LoadMap(Context);
        }

        private void LoadMap(INode node)
        {
            _securityManager.GetPermissionNameForObjectAsync(node, ClientOnGetPermissionNameForObjectCompleted, new GlymaSecurableObject
            {
                SecurableParentUid = node.DomainId,
                SecurableObjectUid = node.Proxy.RootMapId.Value
            });
        }

        private void ClientOnGetPermissionNameForObjectCompleted(object sender, GetPermissionNameForObjectCompletedEventArgs e)
        {
            var permissionLevel = PermissionLevel.None;
            if (!e.Result.HasError && Context == sender)
            {
                var permission = PermissionLevel.Convert(e.Result.Result);
                if (permission >= PermissionLevel.Author || App.PermissionLevel == PermissionLevel.OldAuthor || App.PermissionLevel == PermissionLevel.SecurityManager)
                {
                    permissionLevel = PermissionLevel.Author;
                }
                else if (permission >= PermissionLevel.Reader || App.PermissionLevel == PermissionLevel.OldReader)
                {
                    permissionLevel = PermissionLevel.Reader;
                }
                else if (permission == PermissionLevel.None)
                {
                    SuperMessageBoxService.Show("Access Denied", "It seems that you don't have permission to access this map, please contact your system administrator for assistance.", MessageBoxType.ErrorWithNoInput);
                    return;
                }
            }
            else
            {
                SuperMessageBoxService.ShowError("Error Occurred", "There was a problem reading Glyma permissions, please try again later");
                return;
            }

            if (PermissionLoaded != null)
            {
                PermissionLoaded(this, new UserPermissionEventArgs { Permission = permissionLevel });
            }

            ActiveModel = new Map();

            var nodeType = MapManager.NodeTypes[Context.MapObjectType];

            if (nodeType == MapManager.NodeTypes["CompendiumMapNode"])
            {
                MapManager.QueryMapByNodeCompleted.RegisterEvent(Context.Proxy, LoadNodesCompleted, Context.Proxy);
                MapManager.QueryMapByNodeAsync(Context.Proxy, 1);
            }

            VideoController.Clear();
        }

        private void LoadNodesCompleted(object sender, Proxy.NodesEventArgs eventArgs)
        {
            if (eventArgs.State != Context.Proxy)
            {
                throw new UnexpectedMapException("The state of the event doesn't match with this controller.");
            }

            Proxy.RelationshipSet contextRelationships = Context.Proxy.Relationships;

            if (Context.Proxy.Status != Proxy.LoadState.Full)
            {
                throw new UnexpectedMapException("The relationships were not loaded for the context of this map.");
            }

            var mapChildrenRelationships = contextRelationships.FindRelationships(MapManager.ConnectionTypes["To"], MapManager.RelationshipTypes["MapContainerRelationship"]);

            var modelChangedNodesEventArgs = new ModelChangedEventArgs<Node, Relationship>();
            var modelChangedRelationshipsEventArgs = new ModelChangedEventArgs<Node, Relationship>();

            foreach (Proxy.IRelationship relationship in mapChildrenRelationships)
            {
                Proxy.NodeSet allRelationshipNodes = relationship.Nodes;

                IEnumerable<Proxy.INode> connectedNodes = allRelationshipNodes.FindNodes(MapManager.ConnectionTypes["From"], true);

                if (connectedNodes == null)
                {
                    continue;
                }

                foreach (Proxy.INode modelNode in connectedNodes)
                {
                    var viewModelNode = new Node(MapManager);
                    viewModelNode.LoadNode(relationship, modelNode);
                    ThemeManager.DressNode(viewModelNode);

                    viewModelNode.PropertyChanged += ViewModelNode_PropertyChanged;
                    viewModelNode.LocationChanged += ViewModelNode_LocationChanged;

                    viewModelNode.VideoInfo = VideoController.CreateVideoInfoViewModel(viewModelNode);


                    var fromToRelationships =
                        modelNode.Relationships.FindRelationships(MapManager.RelationshipTypes["FromToRelationship"]);
                    var transclusionRelationships =
                        modelNode.Relationships.FindRelationships(
                            MapManager.RelationshipTypes["TransclusionFromToRelationship"]);

                    foreach (
                        KeyValuePair<Proxy.ConnectionType, Proxy.IRelationship> modelRelationshipPair in
                            fromToRelationships)
                    {
                        var viewModelRelationship = new Relationship();
                        viewModelRelationship.LoadRelationship(modelRelationshipPair.Value);

                        modelChangedRelationshipsEventArgs.Relationships.Add(
                            new KeyValuePair<Relationship, ModelOperationType>(viewModelRelationship,
                                ModelOperationType.New));
                    }

                    foreach (
                        KeyValuePair<Proxy.ConnectionType, Proxy.IRelationship> transclusionRelationshipPair in
                            transclusionRelationships)
                    {
                        var nodes =
                            transclusionRelationshipPair.Value.Nodes.FindNodes(
                                MapManager.ConnectionTypes["TransclusionMap"]);

                        if (nodes != null)
                        {
                            foreach (Proxy.INode node in nodes)
                            {
                                if (node.Id == Context.Proxy.Id)
                                {
                                    var viewModelRelationship = new Relationship();
                                    viewModelRelationship.LoadRelationship(transclusionRelationshipPair.Value);

                                    modelChangedRelationshipsEventArgs.Relationships.Add(
                                        new KeyValuePair<Relationship, ModelOperationType>(viewModelRelationship,
                                            ModelOperationType.New));
                                    break;
                                }
                            }
                        }
                    }

                    modelChangedNodesEventArgs.Nodes.Add(new KeyValuePair<Node, ModelOperationType>(viewModelNode,
                        ModelOperationType.New));


                    ActiveModel.Nodes[viewModelNode.Id] = viewModelNode;
                }
            }

            ModelChanged(this, modelChangedNodesEventArgs);
            ModelChanged(this, modelChangedRelationshipsEventArgs);
            if (MapLoadCompleted != null)
            {
                MapLoadCompleted(this, new EventArgs());
            }
        }

        private void ViewModelNode_LocationChanged(object sender, LocationChangedEventArgs e)
        {
            var node = sender as Node;

            if (node != null)
            {
                var chain = new TransactionFramework.TransactionChain();

                CommitLocation(node, e.Location, ref chain);

                MapManager.ExecuteTransaction(chain);
            }
        }

        private void CommitLocation(Node node, Point location, ref TransactionFramework.TransactionChain chain)
        {
            var relationships = node.Proxy.Relationships.FindRelationships(FromConnectionType, MapContainerRelationshipType);

            var isComplete = false;

            foreach (var relationship in relationships)
            {
                var potentialMapNodes = relationship.Nodes.FindNodes(ToConnectionType);

                if (potentialMapNodes.Any(mapNode => mapNode.Id == Context.Proxy.Id))
                {
                    isComplete = true;

                    node.Proxy.Metadata.Add(relationship, FromConnectionType, "XPosition", location.X.ToString(CultureInfo.InvariantCulture), ref chain);
                    node.Proxy.Metadata.Add(relationship, FromConnectionType, "YPosition", location.Y.ToString(CultureInfo.InvariantCulture), ref chain);
                }

                if (isComplete)
                {
                    break;
                }
            }
        }


        private void ViewModelNode_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {

        }

        #region ISuperGraphNodeOperations Implementation
        void ISuperGraphNodeOperations.GetNodeTransclusions(Node node)
        {
            //Query the service for all the node's details ie relationships
            MapManager.QueryMapByNodeCompleted.RegisterEvent(node.Proxy, NodeLoadCompleted, node);
            MapManager.QueryMapByNodeAsync(node.Proxy,1);
        }

        private void NodeLoadCompleted(object sender, Proxy.NodesEventArgs eventArgs)
        {
            Node node = eventArgs.State as Node;
            IEnumerable<Proxy.IRelationship> mapContainerRelationships = node.Proxy.Relationships.FindRelationships(MapManager.ConnectionTypes["From"], MapManager.RelationshipTypes["MapContainerRelationship"]);
            Dictionary<Guid, string> mapContainerNodes = GetMapContainerNodes(node.Proxy.Id, mapContainerRelationships);
            TransclusionLoadEventArgs transclusionEventArgs = new TransclusionLoadEventArgs() { MapContainerNodes = mapContainerNodes, Node = node };
            if (TransclusionsLoadCompleted != null)
            {
                TransclusionsLoadCompleted(this, transclusionEventArgs);
            }
        }

        //Helper method that looks through all the MapContainerRelationships and returns the map nodes
        private Dictionary<Guid, string> GetMapContainerNodes(Guid nodeId, IEnumerable<Proxy.IRelationship> mapContainerRelationships)
        {
            var output = new Dictionary<Guid, string>();
            foreach (var relationship in mapContainerRelationships)
            {
                var maps = relationship.Nodes.FindNodes(MapManager.ConnectionTypes["To"]);
                if (maps != null)
                {
                    foreach (Proxy.INode mapNode in maps)
                    {
                        //These nodes should all be of NodeType CompendiumMapNode, but being verified anyway
                        if (mapNode != null && mapNode.NodeType == MapManager.NodeTypes["CompendiumMapNode"] && mapNode.Id != nodeId)
                        {
                            //This check should not be here as map container relationships should be unique to maps, 
                            //duplicated relationship needs to be eliminated from it being created.
                            if (!output.ContainsKey(mapNode.Id))
                            {
                                output.Add(mapNode.Id, mapNode.Metadata.FindMetadata("Name").Value);
                            }
                        }
                    }
                }
            }
            return output;
        }
        #endregion

        #region ISuperGraphNodeFactory Implementation

        void ISuperGraphNodeFactory.AddLinkedNode(string nodeType, Point location, Node parent)
        {
            var mapNodeOperation = new AddNodeOperation(nodeType, MapManager, Context.Proxy, location);
            mapNodeOperation.ExecuteOperation();

            if (mapNodeOperation.Response.Nodes.Any())
            {
               
                var viewModelNode = new Node(MapManager);
                viewModelNode.LoadNode(null, mapNodeOperation.Response.Nodes.First());
                ThemeManager.DressNode(viewModelNode);

                viewModelNode.IsFocused = true;
                viewModelNode.VideoInfo = VideoController.CreateVideoInfoViewModel(viewModelNode);

                viewModelNode.PropertyChanged += ViewModelNode_PropertyChanged;
                viewModelNode.LocationChanged += ViewModelNode_LocationChanged;

                var modelChangedEventArgs = new ModelChangedEventArgs<Node, Relationship>();
                modelChangedEventArgs.Nodes.Add(new KeyValuePair<Node, ModelOperationType>(viewModelNode, ModelOperationType.New));
                ModelChanged(this, modelChangedEventArgs);


                var connectNodesOperation = new ConnectNodesOperation(MapManager);
                connectNodesOperation.From = viewModelNode.Proxy;
                connectNodesOperation.To = parent.Proxy;
                connectNodesOperation.ExecuteOperation();
                var relationshipModelCHangedEventArgs = new ModelChangedEventArgs<Node, Relationship>();
                foreach (Proxy.IRelationship modelRelationship in connectNodesOperation.Response.Relationships)
                {
                    var viewModelRelationship = new Relationship();
                    viewModelRelationship.LoadRelationship(modelRelationship);
                    relationshipModelCHangedEventArgs.Relationships.Add(new KeyValuePair<Relationship, ModelOperationType>(viewModelRelationship, ModelOperationType.New));
                }
                ModelChanged(this, relationshipModelCHangedEventArgs);

            }
        }

        void ISuperGraphNodeFactory.AddMap(Point location, Dictionary<string, string> metaData)
        {
            var mapNodeOperation = new AddMapNodeOperation(MapManager, Context.Proxy, location, metaData);
            mapNodeOperation.ExecuteOperation();
            AddNode(mapNodeOperation.Response);
        }

        void ISuperGraphNodeFactory.AddQuestion(Point location, Dictionary<string, string> metaData)
        {
            var questionNodeOperation = new AddQuestionNodeOperation(MapManager, Context.Proxy, location, metaData);
            questionNodeOperation.ExecuteOperation();
            AddNode(questionNodeOperation.Response);
        }

        void ISuperGraphNodeFactory.AddIdea(Point location, Dictionary<string, string> metaData)
        {
            var ideaNodeOperation = new AddIdeaNodeOperation(MapManager, Context.Proxy, location, metaData);
            ideaNodeOperation.ExecuteOperation();
            AddNode(ideaNodeOperation.Response);
        }

        void ISuperGraphNodeFactory.AddPro(Point location, Dictionary<string, string> metaData)
        {
            var proNodeOperation = new AddProNodeOperation(MapManager, Context.Proxy, location, metaData);
            proNodeOperation.ExecuteOperation();
            AddNode(proNodeOperation.Response);
        }

        void ISuperGraphNodeFactory.AddCon(Point location, Dictionary<string, string> metaData)
        {
            var conNodeOperation = new AddConNodeOperation(MapManager, Context.Proxy, location, metaData);
            conNodeOperation.ExecuteOperation();
            AddNode(conNodeOperation.Response);
        }

        void ISuperGraphNodeFactory.AddDecision(Point location, Dictionary<string, string> metaData)
        {
            var conNodeOperation = new AddDecisionNodeOperation(MapManager, Context.Proxy, location, metaData);
            conNodeOperation.ExecuteOperation();
            AddNode(conNodeOperation.Response);
        }

        void ISuperGraphNodeFactory.AddNote(Point location, Dictionary<string, string> metaData)
        {
            var conNodeOperation = new AddNoteNodeOperation(MapManager, Context.Proxy, location, metaData);
            conNodeOperation.ExecuteOperation();
            AddNode(conNodeOperation.Response);
        }

        private void AddNode(TransactionFramework.InProcessTransactionResponse response)
        {
            foreach (Proxy.INode modelNode in response.Nodes)
            {
                var viewModelNode = new Node(MapManager);
                viewModelNode.LoadNode(null, modelNode);
                ThemeManager.DressNode(viewModelNode);

                viewModelNode.IsFocused = true;
                viewModelNode.VideoInfo = VideoController.CreateVideoInfoViewModel(viewModelNode);

                viewModelNode.PropertyChanged += ViewModelNode_PropertyChanged;
                viewModelNode.LocationChanged += ViewModelNode_LocationChanged;

                var modelChangedEventArgs = new ModelChangedEventArgs<Node, Relationship>();
                modelChangedEventArgs.Nodes.Add(new KeyValuePair<Node, ModelOperationType>(viewModelNode, ModelOperationType.New));
                ModelChanged(this, modelChangedEventArgs);
            }
        }

        private void AddClonedNodes(TransactionFramework.InProcessTransactionResponse response)
        {
            foreach (Proxy.INode modelNode in response.Nodes)
            {
                var viewModelNode = new Node(MapManager);
                viewModelNode.LoadNode(null, modelNode);
                ThemeManager.DressNode(viewModelNode);

                viewModelNode.IsFocused = false;
                viewModelNode.VideoInfo = VideoController.CreateVideoInfoViewModel(viewModelNode);

                viewModelNode.PropertyChanged += ViewModelNode_PropertyChanged;
                viewModelNode.LocationChanged += ViewModelNode_LocationChanged;

                var modelChangedEventArgs = new ModelChangedEventArgs<Node, Relationship>();
                modelChangedEventArgs.Nodes.Add(new KeyValuePair<Node, ModelOperationType>(viewModelNode, ModelOperationType.Clone));
                ModelChanged(this, modelChangedEventArgs);
            }
        }

        private void CloneNodeCompleted(object sender, NodeOperationCompletedArgs e)
        {
            var response = e.Response;
            var modelChangedEventArgs = new ModelChangedEventArgs<Node, Relationship>();
            foreach (Proxy.INode modelNode in response.Nodes)
            {
                modelChangedEventArgs.Nodes.Add(new KeyValuePair<Node, ModelOperationType>(new Node(modelNode), ModelOperationType.Clone));
            }
            ModelChanged(this, modelChangedEventArgs);
        }

        void ISuperGraphNodeFactory.DeleteNode(Node viewModelNode)
        {
            var deleteNodeOperation = new DeleteNodeOperation(viewModelNode, MapManager);
            deleteNodeOperation.ExecuteOperation();
            var modelChangedEventArgs = new ModelChangedEventArgs<Node, Relationship>();
            modelChangedEventArgs.Nodes.Add(new KeyValuePair<Node, ModelOperationType>(viewModelNode, ModelOperationType.Deleted));
            ModelChanged(this, modelChangedEventArgs);
        }



        void ISuperGraphNodeFactory.UpdateNode(Node viewModelNode, ChangeNodeTypeEnum changedTo)
        {
            var updateNodeType = new UpdateNodeTypeOperation(MapManager);
            updateNodeType.Context = Context.Proxy;
            updateNodeType.Node = viewModelNode;
            updateNodeType.ChangedTo = changedTo;

            updateNodeType.ExecuteOperation();
            updateNodeType.OperationCompleted += UpdateNodeTypeOnOperationCompleted;
            ThemeManager.DressNode(updateNodeType.Node);
            var modelChangedEventArgs = new ModelChangedEventArgs<Node, Relationship>();
            modelChangedEventArgs.Nodes.Add(new KeyValuePair<Node, ModelOperationType>(updateNodeType.Node, ModelOperationType.Modified));
            ModelChanged(this, modelChangedEventArgs);
        }


        private void UpdateNodeTypeOnOperationCompleted(object sender, NodeOperationCompletedArgs e)
        {
            var modelChangedEventArgs = new ModelChangedEventArgs<Node, Relationship>();
            modelChangedEventArgs.Nodes.Add(new KeyValuePair<Node, ModelOperationType>(e.ViewModeNode, ModelOperationType.Modified));
        }

        void ISuperGraphNodeFactory.CloneNode(Node node)
        {
            var cloneNodeOperation = new CloneNodeOperation(MapManager, Context.Proxy, node);
            cloneNodeOperation.OperationCompleted += CloneNodeCompleted;
            cloneNodeOperation.ExecuteOperation();
            AddClonedNodes(cloneNodeOperation.Response);
        }

        void ISuperGraphNodeFactory.CloneNodes(IEnumerable<Node> nodes, IEnumerable<Relationship> relationships)
        {
            var cloneNodeOperation = new BatchCloneNodesOperation(MapManager, Context.Proxy, nodes, relationships);
            cloneNodeOperation.OperationCompleted += CloneNodeCompleted;
            cloneNodeOperation.ExecuteOperation();
            AddClonedNodes(cloneNodeOperation.Response);
            var modelChangedEventArgs = new ModelChangedEventArgs<Node, Relationship>();
            foreach (Proxy.IRelationship modelRelationship in cloneNodeOperation.Response.Relationships)
            {
                var viewModelRelationship = new Relationship();
                viewModelRelationship.LoadRelationship(modelRelationship);
                modelChangedEventArgs.Relationships.Add(new KeyValuePair<Relationship, ModelOperationType>(viewModelRelationship, ModelOperationType.New));
            }
            ModelChanged(this, modelChangedEventArgs);
        }

        

        

        #endregion

        #region ISuperGraphRelationshipFactory Implementation
        void ISuperGraphRelationshipFactory.ConnectNodes(Node from, Node to)
        {
            var connectNodesOperation = new ConnectNodesOperation(MapManager);
            connectNodesOperation.From = from.Proxy;
            connectNodesOperation.To = to.Proxy;
            connectNodesOperation.ExecuteOperation();
            var modelChangedEventArgs = new ModelChangedEventArgs<Node, Relationship>();
            foreach (Proxy.IRelationship modelRelationship in connectNodesOperation.Response.Relationships)
            {
                var viewModelRelationship = new Relationship();
                viewModelRelationship.LoadRelationship(modelRelationship);
                modelChangedEventArgs.Relationships.Add(new KeyValuePair<Relationship, ModelOperationType>(viewModelRelationship, ModelOperationType.New));
            }
            ModelChanged(this, modelChangedEventArgs);
        }

        void ISuperGraphRelationshipFactory.TranscludeNode(List<Node> nodes, INode map, Point newLocation)
        {
            foreach (var node in nodes)
            {
                var transcludeNodeOperation = new TranscludeNodesOperation(MapManager);
                transcludeNodeOperation.NewMap = Context.Proxy;
                transcludeNodeOperation.OriginalMap = map.Proxy;
                transcludeNodeOperation.ViewModelNode = node;
                transcludeNodeOperation.Location = newLocation;
                node.IsTranscluded = true;
                node.NumTranclusions++;

                var newNodeViewModel = new Node(node);
                newNodeViewModel.Location = newLocation;
                newNodeViewModel.CommitLocation();
                newNodeViewModel.PropertyChanged += ViewModelNode_PropertyChanged;
                var modelChangedEventArgs = new ModelChangedEventArgs<Node, Relationship>();
                modelChangedEventArgs.Nodes.Add(new KeyValuePair<Node, ModelOperationType>(newNodeViewModel, ModelOperationType.New));
                ModelChanged(this, modelChangedEventArgs);

                transcludeNodeOperation.ExecuteOperation();
            }
        }

        void ISuperGraphRelationshipFactory.DeleteTranscludedNode(List<Node> nodes, INode map, bool isQuiet)
        {
            foreach (var node in nodes)
            {
                var deleteTranscludedNodesOperation = new DeleteTranscludedNodeOperation(MapManager);
                deleteTranscludedNodesOperation.Context = map.Proxy;
                deleteTranscludedNodesOperation.Node = node.Proxy;
                deleteTranscludedNodesOperation.ViewModeNode = node;
                deleteTranscludedNodesOperation.ExecuteOperation();
                if (!isQuiet)
                {
                    var modelChangedEventArgs = new ModelChangedEventArgs<Node, Relationship>();
                    modelChangedEventArgs.Nodes.Add(new KeyValuePair<Node, ModelOperationType>(node, ModelOperationType.Deleted));

                    foreach (Proxy.IRelationship modelRelationship in deleteTranscludedNodesOperation.Response.Relationships)
                    {
                        var viewModelRelationship = new Relationship();
                        viewModelRelationship.LoadRelationship(modelRelationship);

                        modelChangedEventArgs.Relationships.Add(new KeyValuePair<Relationship, ModelOperationType>(viewModelRelationship, ModelOperationType.Deleted));
                    }
                    ModelChanged(this, modelChangedEventArgs);
                }
            }

            
        }



        void ISuperGraphRelationshipFactory.ConnectTranscludedNodes(Node from, Node to)
        {
            var connectTranscludedNodesOperation = new ConnectTranscludedNodesOperation(MapManager);
            connectTranscludedNodesOperation.Context = Context.Proxy;
            connectTranscludedNodesOperation.From = from.Proxy;
            connectTranscludedNodesOperation.To = to.Proxy;
            connectTranscludedNodesOperation.ExecuteOperation();
            foreach (Proxy.IRelationship modelRelationship in connectTranscludedNodesOperation.Response.Relationships)
            {
                var viewModelRelationship = new Relationship();
                viewModelRelationship.LoadRelationship(modelRelationship);

                var modelChangedEventArgs = new ModelChangedEventArgs<Node, Relationship>();
                modelChangedEventArgs.Relationships.Add(new KeyValuePair<Relationship, ModelOperationType>(viewModelRelationship, ModelOperationType.New));

                ModelChanged(this, modelChangedEventArgs);
            }
        }


        void ISuperGraphRelationshipFactory.DeleteRelationship(Relationship viewModelRelationship)
        {
            var deleteRelationshipOperation = new DeleteRelationshipOperation(MapManager);
            deleteRelationshipOperation.ViewModeRelationship = viewModelRelationship;
            var modelChangedEventArgs = new ModelChangedEventArgs<Node, Relationship>();
            modelChangedEventArgs.Relationships.Add(new KeyValuePair<Relationship, ModelOperationType>(viewModelRelationship, ModelOperationType.Deleted));
            ModelChanged(this, modelChangedEventArgs);
            deleteRelationshipOperation.ExecuteOperation();
        }


        void ISuperGraphRelationshipFactory.DeleteRelationships(IEnumerable<Relationship> relationships)
        {
            var enumerable = relationships as IList<Relationship> ?? relationships.ToList();
            if (enumerable.Any())
            {
                var deleteRelationshipsOperation = new DeleteRelationships(MapManager);
                var modelChangedEventArgs = new ModelChangedEventArgs<Node, Relationship>();
                foreach (var relationship in enumerable)
                {
                    deleteRelationshipsOperation.Relationships.Add(new KeyValuePair<Relationship, ModelOperationType>(relationship, ModelOperationType.Deleted));
                    modelChangedEventArgs.Relationships.Add(new KeyValuePair<Relationship, ModelOperationType>(relationship, ModelOperationType.Deleted));

                }
                ModelChanged(this, modelChangedEventArgs);
                deleteRelationshipsOperation.ExecuteOperation();
            }
        }

        #endregion

        #region ISuperGraphNodeBatchOperations Implementation

        void ISuperGraphNodeBatchOperations.CommitCollapseStates(IEnumerable<Node> pendingVisibilityNodes, IEnumerable<Node> pendingCollapseStateNodes, PermissionLevel group)
        {
            var operation = new BatchSaveCollapseStatesOperation(MapManager, group);
            operation.Map = Context.Proxy;
            operation.PendingVisibilityNodes = pendingVisibilityNodes;
            operation.PendingCollapseNodes = pendingCollapseStateNodes;
            operation.ExecuteOperation();
        }

        void ISuperGraphNodeBatchOperations.CommitLocations(IEnumerable<Node> nodesToBeCommitted)
        {
            var moveNodesOperation = new BatchMoveNodesOperation(MapManager);
            moveNodesOperation.Map = Context.Proxy;
            moveNodesOperation.Nodes = nodesToBeCommitted;
            moveNodesOperation.ExecuteOperation();
        }


        void ISuperGraphRelationshipFactory.TranscludeNodes(List<Node> nodes, INode map, Point location, Point? oldLocation = null)
        {
            if (nodes.Any())
            {
                var modelChangedEventArgs = new ModelChangedEventArgs<Node, Relationship>();
                var batchTransclusions = new BatchTranscludeNodesOperation(MapManager);
                foreach (var node in nodes)
                {
                    var newOperation = new TranscludeNodesOperation(MapManager);
                    newOperation.NewMap = Context.Proxy;
                    newOperation.OriginalMap = map.Proxy;
                    newOperation.ViewModelNode = node;
                    var oldRelativeLocation = oldLocation != null
                        ? node.Location.Subtract(oldLocation.Value)
                        : node.Location;
                    newOperation.Location = oldRelativeLocation.Add(location);
                    node.IsTranscluded = true;
                    node.NumTranclusions++;
                    batchTransclusions.AddTranscludeNodeOperation(newOperation);

                    var newNodeViewModel = new Node(node);
                    newNodeViewModel.Location = newOperation.Location;
                    newNodeViewModel.CommitLocation();

                    newNodeViewModel.PropertyChanged += ViewModelNode_PropertyChanged;

                    batchTransclusions.Nodes.Add(new KeyValuePair<Node, ModelOperationType>(newNodeViewModel, ModelOperationType.New));
                }
                batchTransclusions.ExecuteOperation();


                modelChangedEventArgs.Nodes = batchTransclusions.Nodes;
                foreach (Proxy.IRelationship modelRelationship in batchTransclusions.Response.Relationships)
                {
                    if (modelRelationship.RelationshipType != modelRelationship.MapManager.RelationshipTypes["MapContainerRelationship"])
                    {
                        var viewModelRelationship = new Relationship();
                        viewModelRelationship.LoadRelationship(modelRelationship);
                        modelChangedEventArgs.Relationships.Add(new KeyValuePair<Relationship, ModelOperationType>(viewModelRelationship, ModelOperationType.New));
                    }
                }
                ModelChanged(this, modelChangedEventArgs);
            }
        }


        void ISuperGraphRelationshipFactory.ConnectMultipleTranscludedNodes(List<Relationship> relationships, List<Node> fullNodes, INode map)
        {
            if (relationships.Any())
            {
                var modelChangedEventArgs = new ModelChangedEventArgs<Node, Relationship>();
                var batchTransclusionConnections = new BatchConnectTranscludedNodesOperation(MapManager);
                foreach (var viewModelRelationship in relationships)
                {
                    var from = fullNodes.FirstOrDefault(q => q.Id == viewModelRelationship.From);
                    var to = fullNodes.FirstOrDefault(q => q.Id == viewModelRelationship.To);

                    if (from != null && to != null)
                    {
                        var operation = new ConnectTranscludedNodesOperation(MapManager);
                        operation.Context = Context.Proxy;
                        operation.From = from.Proxy;
                        operation.To = to.Proxy;

                        batchTransclusionConnections.AddConnectTranscludedNodesOperation(operation);
                    }

                    if (viewModelRelationship.Proxy.RelationshipType != viewModelRelationship.Proxy.MapManager.RelationshipTypes["MapContainerRelationship"])
                    {
                        modelChangedEventArgs.Relationships.Add(new KeyValuePair<Relationship, ModelOperationType>(viewModelRelationship, ModelOperationType.New));
                    }
                }
                batchTransclusionConnections.ExecuteOperation();
                ModelChanged(this, modelChangedEventArgs);
            }
        }


        void ISuperGraphRelationshipFactory.DeleteNodes(List<Node> nodes, INode map, bool isSameMap)
        {
            if (nodes.Any())
            {
                var batchTransclusionDeletions = new BatchDeleteNodesOperation(MapManager);

                foreach (var node in nodes)
                {
                    batchTransclusionDeletions.Nodes.Add(new KeyValuePair<Node, ModelOperationType>(node, ModelOperationType.Deleted));

                    if (node.IsTranscluded && node.NumTranclusions > 1)
                    {
                        var deleteTranscludedNodesOperation = new DeleteTranscludedNodeOperation(MapManager);
                        deleteTranscludedNodesOperation.Context = map.Proxy;
                        deleteTranscludedNodesOperation.Node = node.Proxy;

                        batchTransclusionDeletions.AddDeleteNodeOperation(deleteTranscludedNodesOperation);
                    }
                    else
                    {
                        var deleteNodesOperation = new DeleteNodeOperation(node, MapManager);

                        batchTransclusionDeletions.AddDeleteNodeOperation(deleteNodesOperation);
                    }
                }

                batchTransclusionDeletions.ExecuteOperation();
                if (isSameMap)
                {
                    var modelChangedEventArgs = new ModelChangedEventArgs<Node, Relationship>();
                    var response = batchTransclusionDeletions.Response;
                    modelChangedEventArgs.Nodes = batchTransclusionDeletions.Nodes;
                    foreach (Proxy.IRelationship modelRelationship in response.Relationships)
                    {
                        var viewModelRelationship = new Relationship();
                        viewModelRelationship.LoadRelationship(modelRelationship);

                        modelChangedEventArgs.Relationships.Add(new KeyValuePair<Relationship, ModelOperationType>(viewModelRelationship, ModelOperationType.Deleted));
                    }
                    ModelChanged(this, modelChangedEventArgs);
                }
            }
        }

        #endregion
    }
}
