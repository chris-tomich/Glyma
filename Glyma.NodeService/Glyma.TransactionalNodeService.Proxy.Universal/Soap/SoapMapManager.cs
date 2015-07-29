using System;
using System.Collections.Generic;
using System.ComponentModel;
using TransactionalNodeService.Proxy.Universal.HeaderHandler;
using TransactionalNodeService.Proxy.Universal.EventArgs;
using TransactionalNodeService.Proxy.Universal.EventRegisters;
using TransactionalNodeService.Proxy.Universal.GraphBuilder;
using TransactionalNodeService.Proxy.Universal.Service;
using TransactionalNodeService.Proxy.Universal.Storage;
using TransactionalNodeService.Proxy.Universal.TypeManagers;
using TransactionalNodeService.Proxy.Universal.Types;

namespace TransactionalNodeService.Proxy.Universal.Soap
{
    public class SoapMapManager : IMapManager
    {
        private InMemoryGraph _graph = null;
        private SoapTypeManager _typeManager = null;
        private SoapToServerObjectConverter _soConverter = null;

        private CreateDomainEventRegister _createDomainCompleted = null;
        private CreateRootMapEventRegister _createRootMapCompleted = null;
        private DeleteEventRegister _deleteDomainCompleted = null;
        private DeleteEventRegister _deleteRootMapCompleted = null;
        private QueryMapByNodeEventRegister _preLoadNodesCompleted = null;
        private QueryMapByIdEventRegister _preLoadNodesRawCompleted = null;
        private QueryDomainsEventRegister _acquireDomainsCompleted = null;
        private QueryMapByDomainEventRegister _acquireNodesByDomainCompleted = null;

        public event EventHandler<InitialiseMapManagerEventArgs> InitialiseMapManagerCompleted;
        public event EventHandler<MapManagerActivityEventArgs> MapManagerActivityStatusUpdated;
        public event EventHandler<MetadataChangedEventArgs> AssignMetadataCompleted;

        public SoapMapManager(TransactionalMappingToolServiceClient serviceProxy, bool forceWindowsLogin)
        {
            ServiceProxy = serviceProxy;
            ServiceProxy.QueryMapCompleted += OnQueryMapCompleted;
            ServiceProxy.QueryMapPagedCompleted += OnQueryMapPagedCompleted;
            ServiceProxy.CreateDomainCompleted += OnCreateDomainCompleted;
            ServiceProxy.CreateRootMapCompleted += OnCreateRootMapCompleted;
            ServiceProxy.DeleteDomainCompleted += OnDeleteDomainCompleted;
            ServiceProxy.DeleteRootMapCompleted += OnDeleteRootMapCompleted;
            IsMapTypesLoaded = false;

            if (forceWindowsLogin)
            {
                ForceWindowsAuthHeaderBehaviour forceWindowsAuthBehaviour = new ForceWindowsAuthHeaderBehaviour();
                ServiceProxy.Endpoint.Behaviors.Add(forceWindowsAuthBehaviour);
            }
        }

        public bool IsMapTypesLoaded
        {
            get;
            protected set;
        }

        protected TransactionalMappingToolServiceClient ServiceProxy
        {
            get;
            set;
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

        public INodeFactory NodeFactory
        {
            get
            {
                return Graph.NodeFactory;
            }
        }

        public IRelationshipFactory RelationshipFactory
        {
            get
            {
                return Graph.RelationshipFactory;
            }
        }

        public ITypeManager<Service.NT, NodeType> NodeTypes
        {
            get
            {
                return TypeManager.NodeTypes;
            }
        }

        public ITypeManager<Service.RT, RelationshipType> RelationshipTypes
        {
            get
            {
                return TypeManager.RelationshipTypes;
            }
        }

        public ITypeManager<Service.DT, ConnectionType> ConnectionTypes
        {
            get
            {
                return TypeManager.ConnectionTypes;
            }
        }

        public ITypeManager<Service.MetadataType, Types.MetadataType> MetadataTypes
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

        public CreateDomainEventRegister CreateDomainCompleted
        {
            get
            {
                if (_createDomainCompleted == null)
                {
                    _createDomainCompleted = new CreateDomainEventRegister();
                }

                return _createDomainCompleted;
            }
        }

        public CreateRootMapEventRegister CreateRootMapCompleted
        {
            get
            {
                if (_createRootMapCompleted == null)
                {
                    _createRootMapCompleted = new CreateRootMapEventRegister();
                }

                return _createRootMapCompleted;
            }
        }

        public DeleteEventRegister DeleteDomainCompleted
        {
            get
            {
                if (_deleteDomainCompleted == null)
                {
                    _deleteDomainCompleted = new DeleteEventRegister();
                }

                return _deleteDomainCompleted;
            }
        }

        public DeleteEventRegister DeleteRootMapCompleted
        {
            get
            {
                if (_deleteRootMapCompleted == null)
                {
                    _deleteRootMapCompleted = new DeleteEventRegister();
                }

                return _deleteRootMapCompleted;
            }
        }

        public QueryMapByNodeEventRegister QueryMapByNodeCompleted
        {
            get
            {
                if (_preLoadNodesCompleted == null)
                {
                    _preLoadNodesCompleted = new QueryMapByNodeEventRegister();
                }

                return _preLoadNodesCompleted;
            }
        }

        public QueryMapByIdEventRegister QueryMapByIdCompleted
        {
            get
            {
                if (_preLoadNodesRawCompleted == null)
                {
                    _preLoadNodesRawCompleted = new QueryMapByIdEventRegister();
                }

                return _preLoadNodesRawCompleted;
            }
        }

        public QueryDomainsEventRegister QueryDomainsCompleted
        {
            get
            {
                if (_acquireDomainsCompleted == null)
                {
                    _acquireDomainsCompleted = new QueryDomainsEventRegister();
                }

                return _acquireDomainsCompleted;
            }
        }

        public QueryMapByDomainEventRegister QueryMapByDomainCompleted
        {
            get
            {
                if (_acquireNodesByDomainCompleted == null)
                {
                    _acquireNodesByDomainCompleted = new QueryMapByDomainEventRegister();
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
            if (eventArgs.Error == null)
            {
                ServiceProxy.GetApplicationVersionCompleted -= GetGlymaVersionCompleted;

                Service.GlymaVersion serverVersion = eventArgs.Result;

                if (GlymaClientVersion.RequiresRefresh(serverVersion))
                {
                    InitialiseMapManagerEventArgs initialisationEventArgs = new InitialiseMapManagerEventArgs(false);

                    initialisationEventArgs.ErrorMessage = "The version of Glyma your browser is using is out-of-date. Please clear your browser's cache and refresh your browser to download the latest version of Glyma.";

                    InitialiseMapManagerCompleted(this, initialisationEventArgs);
                }
                else
                {
                    ServiceProxy.GetAllMapTypesCompleted += GetAllMapTypesCompleted;
                    ServiceProxy.GetAllMapTypesAsync(CallingUrl.Url);
                }
            }
            else
            {
                InitialiseMapManagerEventArgs initialisationEventArgs = new InitialiseMapManagerEventArgs(false);

                string errorMsg = eventArgs.Error.ToString();
                if (eventArgs.Error.InnerException != null)
                {
                    //strip off the TargetInvocationException that wraps the real exception
                    errorMsg = eventArgs.Error.InnerException.ToString();
                }
                initialisationEventArgs.ErrorMessage = errorMsg;
                InitialiseMapManagerCompleted(this, initialisationEventArgs);
            }
        }

        private void GetAllMapTypesCompleted(object sender, Service.GetAllMapTypesCompletedEventArgs eventArgs)
        {
            if (eventArgs.Error == null)
            {
                ServiceProxy.GetAllMapTypesCompleted -= GetAllMapTypesCompleted;

                TypeManager.AddNodeTypes(eventArgs.Result.NodeTypes.Values);
                TypeManager.AddRelationshipTypes(eventArgs.Result.RelationshipTypes.Values);
                TypeManager.AddConnectionTypes(eventArgs.Result.DescriptorTypes.Values);
                TypeManager.AddMetadataTypes(eventArgs.Result.MetadataTypes.Values);

                IsMapTypesLoaded = true;

                InitialiseMapManagerEventArgs initialisationEventArgs = new InitialiseMapManagerEventArgs(true);

                InitialiseMapManagerCompleted(this, initialisationEventArgs);
            }
            else
            {
                InitialiseMapManagerEventArgs initialisationEventArgs = new InitialiseMapManagerEventArgs(false);

                string errorMsg = eventArgs.Error.ToString();
                if (eventArgs.Error.InnerException != null) 
                {
                    //strip off the TargetInvocationException that wraps the real exception
                    errorMsg = eventArgs.Error.InnerException.ToString();
                }
                initialisationEventArgs.ErrorMessage = errorMsg;
                InitialiseMapManagerCompleted(this, initialisationEventArgs);
            }
        }

        public void QueryDomainsAsync()
        {
            ServiceProxy.QueryDomainsCompleted += new EventHandler<Service.QueryDomainsCompletedEventArgs>(OnQueryDomainsCompleted);
            ServiceProxy.QueryDomainsAsync(CallingUrl.Url);
        }

        private void OnQueryDomainsCompleted(object sender, Service.QueryDomainsCompletedEventArgs e)
        {
            NodesEventArgs eventArgs = new NodesEventArgs();

            foreach (QueryResponse soapServiceQueryResponse in e.Result.Values)
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

            ServiceProxy.QueryMapAsync(CallingUrl.Url, domainId, nodeId, depth, null, null, responseStateInfo);
        }

        public void QueryMapByNodeAsync(INode node)
        {
            QueryMapByNodeAsync(node, 1);
        }

        public void QueryMapByNodeAsync(INode node, int depth)
        {
            QueryMapResponseStateInfo responseStateInfo = new QueryMapResponseStateInfo();
            responseStateInfo.ResponseType = QueryMapResponseStateType.ByNode;

            ServiceProxy.QueryMapAsync(CallingUrl.Url, node.DomainId, node.Id, depth, null, null, responseStateInfo);
        }

        private void OnQueryMapCompleted(object sender, Service.QueryMapCompletedEventArgs e)
        {
            if (e != null && e.Result != null)
            {
                e.Result.DecompressResponse();
            }

            NodesEventArgs eventArgs = new NodesEventArgs();

            QueryResponse soapServiceQueryResponse = e.Result;
            QueryMapResponseStateInfo callerInfo = e.UserState as QueryMapResponseStateInfo;

            if (soapServiceQueryResponse.FinalObjectIndex <= 0)
            {
                ServerObjects.QueryResponse soQueryResponse = ServerObjectConverter.ToQueryResponse(soapServiceQueryResponse);

                Graph.NodesAndRelationshipBuildEventArgs(eventArgs, soQueryResponse.Nodes, soQueryResponse.Relationships);

                if (e.Result.NodeContext != null)
                {
                    INode context = eventArgs.Nodes[e.Result.NodeContext.N];

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
                QueryResponse aggregatedResponse = new QueryResponse();

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
                ServiceProxy.QueryMapPagedAsync(CallingUrl.Url, aggregatedResponse.Domain.DomainUid, aggregatedResponse.NodeContext.N, 1, null, null, soapServiceQueryResponse.LastObjectIndex + 1, info);
            }
        }

        private void OnQueryMapPagedCompleted(object sender, Service.QueryMapPagedCompletedEventArgs e)
        {
            if (e != null && e.Result != null)
            {
                e.Result.DecompressResponse();
            }

            //ServiceProxy.QueryMapPagedCompleted -= OnQueryMapByIdPagedCompleted;
            QueryResponse queryResponse = e.Result;

            QueryMapResponseStateInfo callerInfo = e.UserState as QueryMapResponseStateInfo;

            QueryResponse aggregatedResponse = callerInfo.AggregatedResponse;//e.UserState as Service.QueryResponse;

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

                NodesEventArgs eventArgs = new NodesEventArgs();

                Graph.NodesAndRelationshipBuildEventArgs(eventArgs, soAggregatedResponse.Nodes, soAggregatedResponse.Relationships);

                INode context = eventArgs.Nodes[e.Result.NodeContext.N];

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
                ServiceProxy.QueryMapPagedAsync(CallingUrl.Url, aggregatedResponse.Domain.DomainUid, aggregatedResponse.NodeContext.N, 1, null, null, queryResponse.LastObjectIndex + 1, info);
            }
        }

        public void QueryMapByDomainAsync(Guid domainId)
        {
            ServiceProxy.QueryMapByDomainCompleted += new EventHandler<Service.QueryMapByDomainCompletedEventArgs>(OnQueryMapByDomainCompleted);
            ServiceProxy.QueryMapByDomainAsync(CallingUrl.Url, domainId, 1, null, null);
        }

        private void OnQueryMapByDomainCompleted(object sender, Service.QueryMapByDomainCompletedEventArgs e)
        {
            if (e != null && e.Result != null)
            {
                e.Result.DecompressResponse();
            }

            if (QueryMapByDomainCompleted != null)
            {
                NodesEventArgs eventArgs = new NodesEventArgs();
                ServerObjects.QueryResponse soQueryResponse = ServerObjectConverter.ToQueryResponse(e.Result);

                Graph.NodesAndRelationshipBuildEventArgs(eventArgs, soQueryResponse.Nodes, soQueryResponse.Relationships);

                INode nodeContext = Graph.FindNode(soQueryResponse.NodeContext);

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

        private void OnTransactionFailed(object sender, System.EventArgs e)
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
                    MapManagerActivityEventArgs status = new MapManagerActivityEventArgs();
                    status.TransactionsLeft = ExecutionThreads.Count + 1;
                    status.Status = ActivityStatusEnum.Busy;

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
                MapManagerActivityEventArgs status = new MapManagerActivityEventArgs();
                status.TransactionsLeft = ExecutionThreads.Count + 1;
                status.Status = ActivityStatusEnum.Busy;

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
                    MapManagerActivityEventArgs status = new MapManagerActivityEventArgs();
                    status.TransactionsLeft = ExecutionThreads.Count + 1;
                    status.Status = ActivityStatusEnum.Busy;

                    MapManagerActivityStatusUpdated.Invoke(this, status);
                }

                executor.ExecuteTransaction(Guid.Empty, ServiceProxy);
            }
            else
            {
                _isInProcess = false;

                if (MapManagerActivityStatusUpdated != null)
                {
                    MapManagerActivityEventArgs status = new MapManagerActivityEventArgs();
                    status.TransactionsLeft = 0;
                    status.Status = ActivityStatusEnum.Idle;

                    MapManagerActivityStatusUpdated.Invoke(this, status);
                }
            }
        }

        private void OnTransactionChainFacadesCompleted(object sender, System.EventArgs e)
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
            ServiceProxy.CreateDomainAsync(CallingUrl.Url, domainName);
        }

        void OnCreateDomainCompleted(object sender, Service.CreateDomainCompletedEventArgs e)
        {
            DomainEventArgs domainEventArgs = new DomainEventArgs();

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

        public void CreateRootMap(Guid domainId, string mapName, NodeType nodeType, string originalId)
        {
            ServiceProxy.CreateRootMapAsync(CallingUrl.Url, domainId, mapName, NodeTypes.ConvertProxyToService(nodeType), originalId);
        }

        void OnCreateRootMapCompleted(object sender, Service.CreateRootMapCompletedEventArgs e)
        {
            if (CreateRootMapCompleted != null)
            {
                NodesEventArgs eventArgs = new NodesEventArgs();
                ServerObjects.QueryResponse soQueryResponse = ServerObjectConverter.ToQueryResponse(e.Result);

                Graph.NodesAndRelationshipBuildEventArgs(eventArgs, soQueryResponse.Nodes, soQueryResponse.Relationships);

                CreateRootMapCompleted.FireEvent(this, eventArgs);
            }
        }

        public void DeleteDomain(Guid domainId)
        {
            ServiceProxy.DeleteDomainAsync(CallingUrl.Url, domainId, domainId);
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

                EventRegisterEventArgs eventArgs = new EventRegisterEventArgs();

                DeleteDomainCompleted.FireEvent(domainId, this, eventArgs);
            }
        }

        public void DeleteRootMap(Guid domainId, Guid rootMapId)
        {
            ServiceProxy.DeleteRootMapAsync(CallingUrl.Url, domainId, rootMapId, rootMapId);
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

                EventRegisterEventArgs eventArgs = new EventRegisterEventArgs();

                DeleteRootMapCompleted.FireEvent(rootMapId, this, eventArgs);
            }
        }

        public INode CreateNode(Guid domainId, Guid rootMapId, NodeType nodeType, string originalId, ref TransactionFramework.TransactionChain chain)
        {
            TransactionFramework.AddNodeTransactionLink createNodeTransaction = new TransactionFramework.AddNodeTransactionLink();
            createNodeTransaction.MapManager = this;
            createNodeTransaction.DomainId = domainId;
            createNodeTransaction.RootMapId = rootMapId;
            createNodeTransaction.NodeType = nodeType;
            createNodeTransaction.OriginalId = originalId;

            chain.AddTransaction(createNodeTransaction);

            INode node = createNodeTransaction.CreateInProcessObjects();

            return node;
        }

        public IRelationship CreateRelationship(Guid domainId, Guid rootMapId, RelationshipType relationshipType, string originalId, ref TransactionFramework.TransactionChain chain)
        {
            TransactionFramework.AddRelationshipTransactionLink createRelationshipTransaction = new TransactionFramework.AddRelationshipTransactionLink();
            createRelationshipTransaction.MapManager = this;
            createRelationshipTransaction.DomainId = domainId;
            createRelationshipTransaction.RootMapId = rootMapId;
            createRelationshipTransaction.RelationshipType = relationshipType;
            createRelationshipTransaction.OriginalId = originalId;

            chain.AddTransaction(createRelationshipTransaction);

            IRelationship relationship = createRelationshipTransaction.CreateInProcessObjects();

            return relationship;
        }
    }
}
