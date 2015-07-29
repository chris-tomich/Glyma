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
using Service = TransactionalNodeService.Service;
using Proxy = TransactionalNodeService.Proxy;
using System.Collections.Generic;
using System.Linq;
using ServerObjects = TransactionalNodeService.Proxy.ServerObjects;
using TransactionFramework = TransactionalNodeService.Soap.TransactionFramework;
using TransactionalNodeService.GraphBuilder;
using TransactionalNodeService.Proxy.Exceptions;
using TransactionalNodeService.Storage;
using System.ComponentModel;
using System.ServiceModel;

namespace TransactionalNodeService.Soap
{
    public class SoapMapManager : Proxy.IMapManager
    {
        private InMemoryGraph _graph = null;
        private SoapTypeManager _typeManager = null;
        private SoapToServerObjectConverter _soConverter = null;
        private Service.TransactionalMappingToolServiceClient _serviceProxy = null;

        private Proxy.CreateDomainEventRegister _createDomainCompleted = null;
        private Proxy.CreateRootMapEventRegister _createRootMapCompleted = null;
        private Proxy.DeleteEventRegister _deleteDomainCompleted = null;
        private Proxy.DeleteEventRegister _deleteRootMapCompleted = null;
        private Proxy.QueryMapByNodeEventRegister _preLoadNodesCompleted = null;
        private Proxy.QueryMapByIdEventRegister _preLoadNodesRawCompleted = null;
        private Proxy.QueryDomainsEventRegister _acquireDomainsCompleted = null;
        private Proxy.QueryMapByDomainEventRegister _acquireNodesByDomainCompleted = null;

        public event EventHandler<Proxy.InitialiseMapManagerEventArgs> InitialiseMapManagerCompleted;
        public event EventHandler<Proxy.MapManagerActivityEventArgs> MapManagerActivityStatusUpdated;
        public event EventHandler<Proxy.MetadataChangedEventArgs> AssignMetadataCompleted;

        public SoapMapManager(ISoapEndPointFactory serviceProxyFactory)
        {
            ServiceProxyFactory = serviceProxyFactory;

            ServiceProxy.QueryMapCompleted += OnQueryMapCompleted;
            ServiceProxy.QueryMapPagedCompleted += OnQueryMapPagedCompleted;
            ServiceProxy.CreateDomainCompleted += OnCreateDomainCompleted;
            ServiceProxy.CreateRootMapCompleted += OnCreateRootMapCompleted;
            ServiceProxy.DeleteDomainCompleted += OnDeleteDomainCompleted;
            ServiceProxy.DeleteRootMapCompleted += OnDeleteRootMapCompleted;

            IsMapTypesLoaded = false;
        }

        public bool IsMapTypesLoaded
        {
            get;
            protected set;
        }

        protected ISoapEndPointFactory ServiceProxyFactory
        {
            get;
            set;
        }

        protected Service.TransactionalMappingToolServiceClient ServiceProxy
        {
            get
            {
                if (_serviceProxy == null || _serviceProxy.State == CommunicationState.Faulted)
                {
                    _serviceProxy = ServiceProxyFactory.Create();
                }
                
                return _serviceProxy;
            }
        }

        private SoapTypeManager TypeManager
        {
            get
            {
                if (_typeManager == null)
                {
                    _typeManager = new SoapTypeManager();
                }

                return _typeManager;
            }
        }

        public SoapToServerObjectConverter ServerObjectConverter
        {
            get
            {
                if (_soConverter == null)
                {
                    _soConverter = new SoapToServerObjectConverter();
                }

                return _soConverter;
            }
        }

        public Proxy.INodeFactory NodeFactory
        {
            get
            {
                return Graph.NodeFactory;
            }
        }

        public Proxy.IRelationshipFactory RelationshipFactory
        {
            get
            {
                return Graph.RelationshipFactory;
            }
        }

        public Proxy.ITypeManager<Service.NT, Proxy.NodeType> NodeTypes
        {
            get
            {
                return TypeManager.NodeTypes;
            }
        }

        public Proxy.ITypeManager<Service.RT, Proxy.RelationshipType> RelationshipTypes
        {
            get
            {
                return TypeManager.RelationshipTypes;
            }
        }

        public Proxy.ITypeManager<Service.DT, Proxy.ConnectionType> ConnectionTypes
        {
            get
            {
                return TypeManager.ConnectionTypes;
            }
        }

        public Proxy.ITypeManager<Service.MetadataType, Proxy.MetadataType> MetadataTypes
        {
            get
            {
                return TypeManager.MetadataTypes;
            }
        }

        public InMemoryGraph Graph
        {
            get
            {
                if (_graph == null)
                {
                    _graph = new InMemoryGraph(this);
                }

                return _graph;
            }
        }

        public Proxy.CreateDomainEventRegister CreateDomainCompleted
        {
            get
            {
                if (_createDomainCompleted == null)
                {
                    _createDomainCompleted = new Proxy.CreateDomainEventRegister();
                }

                return _createDomainCompleted;
            }
        }

        public Proxy.CreateRootMapEventRegister CreateRootMapCompleted
        {
            get
            {
                if (_createRootMapCompleted == null)
                {
                    _createRootMapCompleted = new Proxy.CreateRootMapEventRegister();
                }

                return _createRootMapCompleted;
            }
        }

        public Proxy.DeleteEventRegister DeleteDomainCompleted
        {
            get
            {
                if (_deleteDomainCompleted == null)
                {
                    _deleteDomainCompleted = new Proxy.DeleteEventRegister();
                }

                return _deleteDomainCompleted;
            }
        }

        public Proxy.DeleteEventRegister DeleteRootMapCompleted
        {
            get
            {
                if (_deleteRootMapCompleted == null)
                {
                    _deleteRootMapCompleted = new Proxy.DeleteEventRegister();
                }

                return _deleteRootMapCompleted;
            }
        }

        public Proxy.QueryMapByNodeEventRegister QueryMapByNodeCompleted
        {
            get
            {
                if (_preLoadNodesCompleted == null)
                {
                    _preLoadNodesCompleted = new Proxy.QueryMapByNodeEventRegister();
                }

                return _preLoadNodesCompleted;
            }
        }

        public Proxy.QueryMapByIdEventRegister QueryMapByIdCompleted
        {
            get
            {
                if (_preLoadNodesRawCompleted == null)
                {
                    _preLoadNodesRawCompleted = new Proxy.QueryMapByIdEventRegister();
                }

                return _preLoadNodesRawCompleted;
            }
        }

        public Proxy.QueryDomainsEventRegister QueryDomainsCompleted
        {
            get
            {
                if (_acquireDomainsCompleted == null)
                {
                    _acquireDomainsCompleted = new Proxy.QueryDomainsEventRegister();
                }

                return _acquireDomainsCompleted;
            }
        }

        public Proxy.QueryMapByDomainEventRegister QueryMapByDomainCompleted
        {
            get
            {
                if (_acquireNodesByDomainCompleted == null)
                {
                    _acquireNodesByDomainCompleted = new Proxy.QueryMapByDomainEventRegister();
                }

                return _acquireNodesByDomainCompleted;
            }
        }

        public void InitialiseMapManagerAsync()
        {
            ServiceProxy.GetApplicationVersionCompleted += GetGlymaVersionCompleted;
            ServiceProxy.GetApplicationVersionAsync();
        }

        private void GetGlymaVersionCompleted(object sender, Service.GetApplicationVersionCompletedEventArgs eventArgs)
        {
            ServiceProxy.GetApplicationVersionCompleted -= GetGlymaVersionCompleted;

            Service.GlymaVersion serverVersion = eventArgs.Result;

            if (GlymaClientVersion.RequiresRefresh(serverVersion))
            {
                Proxy.InitialiseMapManagerEventArgs initialisationEventArgs = new Proxy.InitialiseMapManagerEventArgs(false);

                initialisationEventArgs.ErrorMessage = "The version of Glyma your browser is using is out-of-date. Please clear your browser's cache and refresh your browser to download the latest version of Glyma.";

                InitialiseMapManagerCompleted(this, initialisationEventArgs);
            }
            else
            {
                ServiceProxy.GetAllMapTypesCompleted += GetAllMapTypesCompleted;
                ServiceProxy.GetAllMapTypesAsync(System.Windows.Browser.HtmlPage.Document.DocumentUri.ToString());
            }
        }

        private void GetAllMapTypesCompleted(object sender, Service.GetAllMapTypesCompletedEventArgs eventArgs)
        {
            ServiceProxy.GetAllMapTypesCompleted -= GetAllMapTypesCompleted;

            TypeManager.AddNodeTypes(eventArgs.Result.NodeTypes.Values);
            TypeManager.AddRelationshipTypes(eventArgs.Result.RelationshipTypes.Values);
            TypeManager.AddConnectionTypes(eventArgs.Result.DescriptorTypes.Values);
            TypeManager.AddMetadataTypes(eventArgs.Result.MetadataTypes.Values);

            IsMapTypesLoaded = true;

            Proxy.InitialiseMapManagerEventArgs initialisationEventArgs = new Proxy.InitialiseMapManagerEventArgs(true);

            InitialiseMapManagerCompleted(this, initialisationEventArgs);
        }

        public void QueryDomainsAsync()
        {
            ServiceProxy.QueryDomainsCompleted += new EventHandler<Service.QueryDomainsCompletedEventArgs>(OnQueryDomainsCompleted);
            ServiceProxy.QueryDomainsAsync(System.Windows.Browser.HtmlPage.Document.DocumentUri.ToString());
        }

        private void OnQueryDomainsCompleted(object sender, Service.QueryDomainsCompletedEventArgs e)
        {
            Proxy.NodesEventArgs eventArgs = new Proxy.NodesEventArgs();

            foreach (Service.QueryResponse soapServiceQueryResponse in e.Result.Values)
            {
                ServerObjects.QueryResponse soQueryResponse = ServerObjectConverter.ToQueryResponse(soapServiceQueryResponse);

                Graph.NodesAndRelationshipBuildEventArgs(eventArgs, soQueryResponse.Nodes, soQueryResponse.Relationships);
            }

            QueryDomainsCompleted.FireEvent(this, eventArgs);
        }

        public void QueryMapByIdAsync(Guid domainId, Guid nodeId)
        {
            QueryMapByIdAsync(domainId, nodeId, 1);
        }

        public void QueryMapByIdAsync(Guid domainId, Guid nodeId, int depth)
        {
            QueryMapResponseStateInfo responseStateInfo = new QueryMapResponseStateInfo();
            responseStateInfo.ResponseType = QueryMapResponseStateType.ById;

            ServiceProxy.QueryMapAsync(System.Windows.Browser.HtmlPage.Document.DocumentUri.ToString(), domainId, nodeId, depth, null, null, responseStateInfo);
        }

        public void QueryMapByNodeAsync(Proxy.INode node)
        {
            QueryMapByNodeAsync(node, 1);
        }

        public void QueryMapByNodeAsync(Proxy.INode node, int depth)
        {
            QueryMapResponseStateInfo responseStateInfo = new QueryMapResponseStateInfo();
            responseStateInfo.ResponseType = QueryMapResponseStateType.ByNode;

            ServiceProxy.QueryMapAsync(System.Windows.Browser.HtmlPage.Document.DocumentUri.ToString(), node.DomainId, node.Id, depth, null, null, responseStateInfo);
        }

        private void OnQueryMapCompleted(object sender, Service.QueryMapCompletedEventArgs e)
        {
            if (e != null && e.Result != null)
            {
                e.Result.DecompressResponse();
            }

            Proxy.NodesEventArgs eventArgs = new Proxy.NodesEventArgs();

            Service.QueryResponse soapServiceQueryResponse = e.Result;
            QueryMapResponseStateInfo callerInfo = e.UserState as QueryMapResponseStateInfo;

            if (soapServiceQueryResponse.FinalObjectIndex <= 0)
            {
                ServerObjects.QueryResponse soQueryResponse = ServerObjectConverter.ToQueryResponse(soapServiceQueryResponse);

                Graph.NodesAndRelationshipBuildEventArgs(eventArgs, soQueryResponse.Nodes, soQueryResponse.Relationships);

                if (e.Result.NodeContext != null)
                {
                    Proxy.INode context = eventArgs.Nodes[e.Result.NodeContext.N];

                    if (callerInfo.ResponseType == QueryMapResponseStateType.ById)
                    {
                        QueryMapByIdCompleted.FireEvent(context.Id, this, eventArgs);
                    }
                    else if (callerInfo.ResponseType == QueryMapResponseStateType.ByNode)
                    {
                        QueryMapByNodeCompleted.FireEvent(context, this, eventArgs);
                    }
                }
                else
                {
                    /// TODO: Need to find a nicer way of handling this exception rather than just ignoring it altogether.
                    //throw new NodeNotFoundException("The given node wasn't found");
                }
            }
            else
            {
                Service.QueryResponse aggregatedResponse = new Service.QueryResponse();

                aggregatedResponse.Domain = soapServiceQueryResponse.Domain;
                aggregatedResponse.NodeContext = soapServiceQueryResponse.NodeContext;

                aggregatedResponse.Nodes = new Dictionary<Guid, Service.NO>();
                aggregatedResponse.Relationships = new Dictionary<Guid, Service.RE>();

                foreach (KeyValuePair<Guid, Service.NO> nodePair in soapServiceQueryResponse.Nodes)
                {
                    aggregatedResponse.Nodes[nodePair.Key] = nodePair.Value;
                }

                foreach (KeyValuePair<Guid, Service.RE> relationshipPair in soapServiceQueryResponse.Relationships)
                {
                    aggregatedResponse.Relationships[relationshipPair.Key] = relationshipPair.Value;
                }

                //ServiceProxy.QueryMapPagedCompleted += OnQueryMapByIdPagedCompleted;
                QueryMapResponseStateInfo info = new QueryMapResponseStateInfo();
                info.AggregatedResponse = aggregatedResponse;
                info.ResponseType = callerInfo.ResponseType;
                ServiceProxy.QueryMapPagedAsync(System.Windows.Browser.HtmlPage.Document.DocumentUri.ToString(), aggregatedResponse.Domain.DomainUid, aggregatedResponse.NodeContext.N, 1, null, null, soapServiceQueryResponse.LastObjectIndex + 1, info);
            }
        }

        private void OnQueryMapPagedCompleted(object sender, Service.QueryMapPagedCompletedEventArgs e)
        {
            if (e != null && e.Result != null)
            {
                e.Result.DecompressResponse();
            }

            //ServiceProxy.QueryMapPagedCompleted -= OnQueryMapByIdPagedCompleted;
            Service.QueryResponse queryResponse = e.Result;

            QueryMapResponseStateInfo callerInfo = e.UserState as QueryMapResponseStateInfo;

            Service.QueryResponse aggregatedResponse = callerInfo.AggregatedResponse;//e.UserState as Service.QueryResponse;

            if (aggregatedResponse == null)
            {
                aggregatedResponse.Domain = queryResponse.Domain;
                aggregatedResponse.NodeContext = queryResponse.NodeContext;

                aggregatedResponse.Nodes = new Dictionary<Guid, Service.NO>();
                aggregatedResponse.Relationships = new Dictionary<Guid, Service.RE>();
            }

            foreach (KeyValuePair<Guid, Service.NO> nodePair in queryResponse.Nodes)
            {
                aggregatedResponse.Nodes[nodePair.Key] = nodePair.Value;
            }

            foreach (KeyValuePair<Guid, Service.RE> relationshipPair in queryResponse.Relationships)
            {
                aggregatedResponse.Relationships[relationshipPair.Key] = relationshipPair.Value;
            }

            if (queryResponse.LastObjectIndex >= queryResponse.FinalObjectIndex)
            {
                ServerObjects.QueryResponse soAggregatedResponse = ServerObjectConverter.ToQueryResponse(aggregatedResponse);

                Proxy.NodesEventArgs eventArgs = new Proxy.NodesEventArgs();

                Graph.NodesAndRelationshipBuildEventArgs(eventArgs, soAggregatedResponse.Nodes, soAggregatedResponse.Relationships);

                Proxy.INode context = eventArgs.Nodes[e.Result.NodeContext.N];

                if (callerInfo.ResponseType == QueryMapResponseStateType.ById)
                {
                    QueryMapByIdCompleted.FireEvent(context.Id, this, eventArgs);
                }
                else if (callerInfo.ResponseType == QueryMapResponseStateType.ByNode)
                {
                    QueryMapByNodeCompleted.FireEvent(context, this, eventArgs);
                }
            }
            else
            {
                QueryMapResponseStateInfo info = new QueryMapResponseStateInfo();
                info.AggregatedResponse = aggregatedResponse;
                info.ResponseType = callerInfo.ResponseType;
                ServiceProxy.QueryMapPagedAsync(System.Windows.Browser.HtmlPage.Document.DocumentUri.ToString(), aggregatedResponse.Domain.DomainUid, aggregatedResponse.NodeContext.N, 1, null, null, queryResponse.LastObjectIndex + 1, info);
            }
        }

        public void QueryMapByDomainAsync(Guid domainId)
        {
            ServiceProxy.QueryMapByDomainCompleted += new EventHandler<Service.QueryMapByDomainCompletedEventArgs>(OnQueryMapByDomainCompleted);
            ServiceProxy.QueryMapByDomainAsync(System.Windows.Browser.HtmlPage.Document.DocumentUri.ToString(), domainId, 1, null, null);
        }

        private void OnQueryMapByDomainCompleted(object sender, Service.QueryMapByDomainCompletedEventArgs e)
        {
            if (e != null && e.Result != null)
            {
                e.Result.DecompressResponse();
            }

            if (QueryMapByDomainCompleted != null)
            {
                Proxy.NodesEventArgs eventArgs = new Proxy.NodesEventArgs();
                ServerObjects.QueryResponse soQueryResponse = ServerObjectConverter.ToQueryResponse(e.Result);

                Graph.NodesAndRelationshipBuildEventArgs(eventArgs, soQueryResponse.Nodes, soQueryResponse.Relationships);

                Proxy.INode nodeContext = Graph.FindNode(soQueryResponse.NodeContext);

                QueryMapByDomainCompleted.FireEvent(nodeContext.DomainId, this, eventArgs);
            }
        }

        private bool _isInProcess = false;
        private object _executionThreadsLock = new object();
        private TransactionStore _executionThreads = null;
        private List<TransactionFramework.ISoapTransactionLinkExecutor> _incompleteExecutionThreads = null;

        private TransactionStore ExecutionThreads
        {
            get
            {
                if (_executionThreads == null)
                {
                    _executionThreads = new TransactionStore("UnfinishedTransactions");
                }

                return _executionThreads;
            }
        }

        private List<TransactionFramework.ISoapTransactionLinkExecutor> IncompleteExecutionThreads
        {
            get
            {
                if (_incompleteExecutionThreads == null)
                {
                    _incompleteExecutionThreads = new List<TransactionFramework.ISoapTransactionLinkExecutor>();
                }

                return _incompleteExecutionThreads;
            }
        }

        public void ExecuteTransactions(Queue<TransactionFramework.TransactionChain> chains)
        {
            TransactionFramework.TransactionChain firstChain = null;

            lock (_executionThreadsLock)
            {
                while (chains.Count > 0)
                {
                    TransactionFramework.TransactionChain chain = chains.Dequeue();

                    /// TODO: Need to figure why this is happening for the re-alignment code.
                    /// TODO: Probably should add a warning in the debugger to indicate there is a potential performance problem if there are too many chains with empty transactions.
                    if (chain.NumOfTransactions <= 0)
                    {
                        continue;
                    }

                    if (firstChain == null)
                    {
                        firstChain = chain;

                        ExecutionThreads.AddTransactionChain(firstChain);

                        if (_isInProcess)
                        {
                            firstChain = null;
                        }
                        else
                        {
                            ExecutionThreads.MoveToNextChain();
                            _isInProcess = true;
                        }
                    }
                    else
                    {
                        ExecutionThreads.AddTransactionChain(chain);
                    }
                }
            }

            if (firstChain != null)
            {
                firstChain.TransactionFailed += OnTransactionFailed;
                firstChain.TransactionCompleted += OnTransactionCompleted;
                firstChain.ExecuteTransaction(Guid.Empty, ServiceProxy);
            }
        }

        private void OnTransactionFailed(object sender, EventArgs e)
        {
            ForceTransactionReExecution();
        }

        public void ForceTransactionReExecution()
        {
            TransactionFramework.ISoapTransactionLinkExecutor executor = null;

            if (_executionThreadsLock == null || ExecutionThreads == null || ExecutionThreads.CurrentChain == null || ExecutionThreads.CurrentChain.End == null)
            {
                return;
            }

            TransactionFramework.TransactionChain transactionChain = ExecutionThreads.CurrentChain;
            transactionChain.Reset();

            /// If the last transaction is in one of the post execution states and the user has requested to force the commit,
            /// then it means we should attempt to do the current transaction again as something appears to have happened to
            /// cause the previous attempt to hang.
            if (transactionChain.End.TransactionStatus == TransactionFramework.ServerStatus.TransactionCompleted || transactionChain.End.TransactionStatus == TransactionFramework.ServerStatus.TransactionReceived || transactionChain.End.TransactionStatus == TransactionFramework.ServerStatus.TransactionFailed)
            {
                executor = ExecutionThreads.CurrentChain;

                /// Debug lines
                /// TODO: Remove the following
                if (transactionChain != null)
                {
                    System.Diagnostics.Debug.WriteLine(transactionChain.ChainId);
                }
                ///

                if (MapManagerActivityStatusUpdated != null)
                {
                    Proxy.MapManagerActivityEventArgs status = new Proxy.MapManagerActivityEventArgs();
                    status.TransactionsLeft = ExecutionThreads.Count + 1;
                    status.Status = Proxy.ActivityStatusEnum.Busy;

                    MapManagerActivityStatusUpdated.Invoke(this, status);
                }

                executor.ExecuteTransaction(Guid.Empty, ServiceProxy);
            }
        }

        public void ExecuteTransaction(TransactionFramework.TransactionChain transactionChain)
        {
            /// TODO: Need to figure why this is happening for the re-alignment code.
            /// TODO: Probably should add a warning in the debugger to indicate there is a potential performance problem if there are too many chains with empty transactions.
            if (transactionChain.NumOfTransactions <= 0)
            {
                return;
            }

            TransactionFramework.ISoapTransactionLinkExecutor executor = transactionChain;

            if (executor == null)
            {
                throw new NotSupportedException("This is not a supported type of link. This link does not implement ISoapTransactionLinkExecutor.");
            }

            lock (_executionThreadsLock)
            {
                ExecutionThreads.AddTransactionChain(transactionChain);

                if (_isInProcess)
                {
                    executor = null;
                }
                else
                {
                    ExecutionThreads.MoveToNextChain();
                    _isInProcess = true;
                }
            }

            if (MapManagerActivityStatusUpdated != null)
            {
                Proxy.MapManagerActivityEventArgs status = new Proxy.MapManagerActivityEventArgs();
                status.TransactionsLeft = ExecutionThreads.Count + 1;
                status.Status = Proxy.ActivityStatusEnum.Busy;

                MapManagerActivityStatusUpdated.Invoke(this, status);
            }

            if (executor != null)
            {
                /// TODO: Remove the following
                TransactionFramework.TransactionChain chain = executor as TransactionFramework.TransactionChain;

                if (chain != null)
                {
                    System.Diagnostics.Debug.WriteLine(chain.ChainId);
                }
                ///

                executor.TransactionFailed += OnTransactionFailed;
                executor.TransactionCompleted += OnTransactionCompleted;
                executor.ExecuteTransaction(Guid.Empty, ServiceProxy);
            }
        }

        private void OnTransactionCompleted(object sender, Service.CompleteTransactionCompletedEventArgs e)
        {
            TransactionFramework.ISoapTransactionLinkExecutor executor = null;

            lock (_executionThreadsLock)
            {
                if (e.Error != null)
                {
                    /// An error occurred so let's retry the transaction.
                    executor = ExecutionThreads.CurrentChain;
                }
                while (executor == null && ExecutionThreads.Count > 0)
                {
                    _isInProcess = true;
                    executor = ExecutionThreads.MoveToNextChain();
                    executor.UpdateDependencies();

                    if (executor.Dependencies.IsCompleted)
                    {
                        executor.TransactionFailed += OnTransactionFailed;
                        executor.TransactionCompleted += OnTransactionCompleted;
                    }
                    else
                    {
                        IncompleteExecutionThreads.Add(executor);
                        executor.Dependencies.FacadesCompleted += OnTransactionChainFacadesCompleted;
                        executor = null;
                    }
                }
            }

            if (executor != null)
            {
                /// TODO: Remove the following
                TransactionFramework.TransactionChain chain = executor as TransactionFramework.TransactionChain;

                if (chain != null)
                {
                    System.Diagnostics.Debug.WriteLine(chain.ChainId);
                }
                ///

                if (MapManagerActivityStatusUpdated != null)
                {
                    Proxy.MapManagerActivityEventArgs status = new Proxy.MapManagerActivityEventArgs();
                    status.TransactionsLeft = ExecutionThreads.Count + 1;
                    status.Status = Proxy.ActivityStatusEnum.Busy;

                    MapManagerActivityStatusUpdated.Invoke(this, status);
                }

                executor.ExecuteTransaction(Guid.Empty, ServiceProxy);
            }
            else
            {
                _isInProcess = false;

                if (MapManagerActivityStatusUpdated != null)
                {
                    Proxy.MapManagerActivityEventArgs status = new Proxy.MapManagerActivityEventArgs();
                    status.TransactionsLeft = 0;
                    status.Status = Proxy.ActivityStatusEnum.Idle;

                    MapManagerActivityStatusUpdated.Invoke(this, status);
                }
            }
        }

        private void OnTransactionChainFacadesCompleted(object sender, EventArgs e)
        {
            TransactionFramework.DependencyCollection dependencies = sender as TransactionFramework.DependencyCollection;

            if (dependencies == null)
            {
                return;
            }

            TransactionFramework.TransactionChain chain = dependencies.State as TransactionFramework.TransactionChain;

            if (chain == null)
            {
                return;
            }

            IncompleteExecutionThreads.Remove(chain);
            ExecuteTransaction(chain);
        }

        public void CreateDomain(string domainName)
        {
            ServiceProxy.CreateDomainAsync(System.Windows.Browser.HtmlPage.Document.DocumentUri.ToString(), domainName);
        }

        void OnCreateDomainCompleted(object sender, Service.CreateDomainCompletedEventArgs e)
        {
            Proxy.DomainEventArgs domainEventArgs = new Proxy.DomainEventArgs();

            foreach (Service.CH mapChange in e.Result.C)
            {
                if (mapChange.N != null)
                {
                    domainEventArgs.Domain = mapChange.N.D;
                    break;
                }
            }

            CreateDomainCompleted.FireEvent(Guid.Empty, this, domainEventArgs);
        }

        public void CreateRootMap(Guid domainId, string mapName, Proxy.NodeType nodeType, string originalId)
        {
            ServiceProxy.CreateRootMapAsync(System.Windows.Browser.HtmlPage.Document.DocumentUri.ToString(), domainId, mapName, NodeTypes.ConvertProxyToService(nodeType), originalId);
        }

        void OnCreateRootMapCompleted(object sender, Service.CreateRootMapCompletedEventArgs e)
        {
            if (CreateRootMapCompleted != null)
            {
                Proxy.NodesEventArgs eventArgs = new Proxy.NodesEventArgs();
                ServerObjects.QueryResponse soQueryResponse = ServerObjectConverter.ToQueryResponse(e.Result);

                Graph.NodesAndRelationshipBuildEventArgs(eventArgs, soQueryResponse.Nodes, soQueryResponse.Relationships);

                CreateRootMapCompleted.FireEvent(this, eventArgs);
            }
        }

        public void DeleteDomain(Guid domainId)
        {
            ServiceProxy.DeleteDomainAsync(System.Windows.Browser.HtmlPage.Document.DocumentUri.ToString(), domainId, domainId);
        }

        private void OnDeleteDomainCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (DeleteDomainCompleted != null)
            {
                if (e.UserState == null || !(e.UserState is Guid))
                {
                    return;
                }

                Guid domainId = (Guid)e.UserState;

                Proxy.EventRegisterEventArgs eventArgs = new Proxy.EventRegisterEventArgs();

                DeleteDomainCompleted.FireEvent(domainId, this, eventArgs);
            }
        }

        public void DeleteRootMap(Guid domainId, Guid rootMapId)
        {
            ServiceProxy.DeleteRootMapAsync(System.Windows.Browser.HtmlPage.Document.DocumentUri.ToString(), domainId, rootMapId, rootMapId);
        }

        private void OnDeleteRootMapCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (DeleteRootMapCompleted != null)
            {
                if (e.UserState == null || !(e.UserState is Guid))
                {
                    return;
                }

                Guid rootMapId = (Guid)e.UserState;

                Proxy.EventRegisterEventArgs eventArgs = new Proxy.EventRegisterEventArgs();

                DeleteRootMapCompleted.FireEvent(rootMapId, this, eventArgs);
            }
        }

        public Proxy.INode CreateNode(Guid domainId, Guid rootMapId, Proxy.NodeType nodeType, string originalId, ref TransactionFramework.TransactionChain chain)
        {
            TransactionFramework.AddNodeTransactionLink createNodeTransaction = new TransactionFramework.AddNodeTransactionLink();
            createNodeTransaction.MapManager = this;
            createNodeTransaction.DomainId = domainId;
            createNodeTransaction.RootMapId = rootMapId;
            createNodeTransaction.NodeType = nodeType;
            createNodeTransaction.OriginalId = originalId;

            chain.AddTransaction(createNodeTransaction);

            Proxy.INode node = createNodeTransaction.CreateInProcessObjects();

            return node;
        }

        public Proxy.IRelationship CreateRelationship(Guid domainId, Guid rootMapId, Proxy.RelationshipType relationshipType, string originalId, ref TransactionFramework.TransactionChain chain)
        {
            TransactionFramework.AddRelationshipTransactionLink createRelationshipTransaction = new TransactionFramework.AddRelationshipTransactionLink();
            createRelationshipTransaction.MapManager = this;
            createRelationshipTransaction.DomainId = domainId;
            createRelationshipTransaction.RootMapId = rootMapId;
            createRelationshipTransaction.RelationshipType = relationshipType;
            createRelationshipTransaction.OriginalId = originalId;

            chain.AddTransaction(createRelationshipTransaction);

            Proxy.IRelationship relationship = createRelationshipTransaction.CreateInProcessObjects();

            return relationship;
        }
    }
}
