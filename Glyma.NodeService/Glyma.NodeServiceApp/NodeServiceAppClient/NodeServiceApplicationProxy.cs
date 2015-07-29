using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.Text;
using TransactionalNodeService.Common;
using TransactionalNodeService.Common.Model;
using TransactionalNodeService.Common.NodeServiceOperations.Bulk;

namespace Glyma.NodeServiceApp
{
    [System.Runtime.InteropServices.Guid("693E86B3-574D-4EAD-8D94-15CAC086B304")]
    public class NodeServiceApplicationProxy : SPIisWebServiceApplicationProxy
    {
        private ChannelFactory<IGlymaNodeServiceApp> _channelFactory;
        private object _channelFactoryLock = new object();
        private string _endpointConfigName;

        [Persisted]
        private SPServiceLoadBalancer _loadBalancer;

        public NodeServiceApplicationProxy()
        {
        }

        public NodeServiceApplicationProxy(string name, NodeServiceProxy nodeServiceProxy, Uri serviceApplicationUri)
            : base(name, nodeServiceProxy, serviceApplicationUri)
        {
            _loadBalancer = new SPRoundRobinServiceLoadBalancer(serviceApplicationUri);
        }

        private ChannelFactory<T> CreateChannelFactory<T>(string endpointConfigName)
        {
            // Open the client.config.
#if SP2010
            string clientConfigPath = SPUtility.GetGenericSetupPath(@"WebClients\GlymaNodeServiceApp");
#endif
#if SP2013
            string clientConfigPath = SPUtility.GetVersionedGenericSetupPath(@"WebClients\GlymaNodeServiceApp", SPUtility.ContextCompatibilityLevel);
#endif
            Configuration clientConfig = OpenClientConfiguration(clientConfigPath);
            ConfigurationChannelFactory<T> factory = new ConfigurationChannelFactory<T>(endpointConfigName, clientConfig, null);

            // Configure the channel factory.
            factory.ConfigureCredentials(SPServiceAuthenticationMode.Claims);

            return factory;
        }

        internal delegate void CodeToRunOnApplicationProxy(NodeServiceApplicationProxy appProxy);

        internal static void Invoke(SPServiceContext serviceContext, CodeToRunOnApplicationProxy codeBlock)
        {
            if (serviceContext == null)
            {
                throw new ArgumentNullException("serviceContext");
            }

            // Get service app proxy from the context.
            NodeServiceApplicationProxy proxy = (NodeServiceApplicationProxy)serviceContext.GetDefaultProxy(typeof(NodeServiceApplicationProxy));

            if (proxy == null)
            {
                throw new InvalidOperationException("Unable to obtain object reference to node service proxy.");
            }

            // Run the code block on the proxy.
            using (new SPServiceContextScope(serviceContext))
            {
                codeBlock(proxy);
            }
        }

        private string GetEndpointConfigName(Uri address)
        {
            string configName;

            // Get the the config name for the provided address.
            if (address.Scheme == Uri.UriSchemeHttp)
            {
                configName = "http";
            }
            else if (address.Scheme == Uri.UriSchemeHttps)
            {
                configName = "https";
            }
            else
            {
                throw new NotSupportedException("Unsupported endpoint address.");
            }

            return configName;
        }

        private IGlymaNodeServiceApp GetChannel(Uri address)
        {
            string endpointConfig = GetEndpointConfigName(address);

            // if there's a cached channel, use that
            if ((_channelFactory == null) || (endpointConfig != _endpointConfigName))
            {
                lock (_channelFactoryLock)
                {
                    // create a channel factory using the endpoint name
                    _channelFactory = CreateChannelFactory<IGlymaNodeServiceApp>(endpointConfig);
                    // cache the created channel
                    _endpointConfigName = endpointConfig;
                }
            }

            IGlymaNodeServiceApp channel;

            // create a channel that acts as the logged on user when authenticating with the service
            channel = _channelFactory.CreateChannelActingAsLoggedOnUser<IGlymaNodeServiceApp>(new EndpointAddress(address));

            return channel;
        }

        private delegate void CodeToRunOnChannel(IGlymaNodeServiceApp contract);

        private void ExecuteOnChannel(string operationName, CodeToRunOnChannel codeBlock)
        {
            SPServiceLoadBalancerContext loadBalancerContext = _loadBalancer.BeginOperation();

            try
            {
                // get a channel to the service app endpoint
                IChannel channel = (IChannel)GetChannel(loadBalancerContext.EndpointAddress);
                try
                {
                    // execute the code block
                    codeBlock((IGlymaNodeServiceApp)channel);
                    channel.Close();
                }
                catch (TimeoutException)
                {
                    loadBalancerContext.Status = SPServiceLoadBalancerStatus.Failed;
                    throw;
                }
                catch (EndpointNotFoundException)
                {
                    loadBalancerContext.Status = SPServiceLoadBalancerStatus.Failed;
                    throw;
                }
                finally
                {
                    if (channel.State != CommunicationState.Closed)
                        channel.Abort();
                }
            }
            finally
            {
                loadBalancerContext.EndOperation();
            }
        }

        public override string TypeName
        {
            get
            {
                return "Glyma Node Service Application Proxy";
            }
        }

        // Provisioning the app proxy requires creating a new load balancer.
        public override void Provision()
        {
            _loadBalancer.Provision();
            base.Provision();
            this.Update();
        }

        // Unprovisioning the app proxy requires deleting the load balancer.
        public override void Unprovision(bool deleteData)
        {
            _loadBalancer.Unprovision();
            base.Unprovision(deleteData);
            this.Update();
        }

        #region Service Application Methods
        public GlymaVersion GetApplicationVersion()
        {
            GlymaVersion result = null;

            // execute the call against the service app
            ExecuteOnChannel("GetApplicationVersion",
                delegate(IGlymaNodeServiceApp channel)
                {
                    result = channel.GetApplicationVersion();
                });

            return result;
        }

        public MapResponse CreateDomain(GlymaSessionConfiguration configuration, string name)
        {
            MapResponse result = null;

            // execute the call against the service app
            ExecuteOnChannel("CreateDomain",
                delegate(IGlymaNodeServiceApp channel)
                {
                    result = channel.CreateDomain(configuration, name);
                });

            return result;
        }

        public QueryResponse CreateRootMap(GlymaSessionConfiguration configuration, Guid domainId, string name, NodeType nodeType, string originalId)
        {
            QueryResponse result = null;

            // execute the call against the service app
            ExecuteOnChannel("CreateRootMap",
                delegate(IGlymaNodeServiceApp channel)
                {
                    result = channel.CreateRootMap(configuration, domainId, name, nodeType, originalId);
                });

            return result;
        }

        public int DeleteDomain(GlymaSessionConfiguration configuration, Guid domainId)
        {
            int result = 0;

            // execute the call against the service app
            ExecuteOnChannel("DeleteDomain",
                delegate(IGlymaNodeServiceApp channel)
                {
                    result = channel.DeleteDomain(configuration, domainId);
                });

            return result;
        }

        public int DeleteRootMap(GlymaSessionConfiguration configuration, Guid domainId, Guid rootMapId)
        {
            int result = 0;

            // execute the call against the service app
            ExecuteOnChannel("DeleteRootMap",
                delegate(IGlymaNodeServiceApp channel)
                {
                    result = channel.DeleteRootMap(configuration, domainId, rootMapId);
                });

            return result;
        }

        public Guid BeginTransaction(GlymaSessionConfiguration configuration)
        {
            Guid result = Guid.Empty;

            // execute the call against the service app
            ExecuteOnChannel("BeginTransaction",
                delegate(IGlymaNodeServiceApp channel)
                {
                    result = channel.BeginTransaction(configuration);
                });

            return result;
        }

        public MapResponse CompleteTransaction(GlymaSessionConfiguration configuration, Guid sessionId)
        {
            MapResponse result = null;

            // execute the call against the service app
            ExecuteOnChannel("CompleteTransaction",
                delegate(IGlymaNodeServiceApp channel)
                {
                    result = channel.CompleteTransaction(configuration, sessionId);
                });

            return result;
        }

        public TypeResponse GetAllMapTypes(GlymaSessionConfiguration configuration)
        {
            TypeResponse result = null;

            // execute the call against the service app
            ExecuteOnChannel("GetAllMapTypes",
                delegate(IGlymaNodeServiceApp channel)
                {
                    result = channel.GetAllMapTypes(configuration);
                });

            return result;
        }

        public Dictionary<Guid, QueryResponse> QueryDomains(GlymaSessionConfiguration configuration)
        {
            Dictionary<Guid, QueryResponse> result = null;

            // execute the call against the service app
            ExecuteOnChannel("QueryDomains",
                delegate(IGlymaNodeServiceApp channel)
                {
                    result = channel.QueryDomains(configuration);
                });

            return result;
        }

        public QueryResponse QueryMap(GlymaSessionConfiguration configuration, Guid domainId, Guid nodeId, int maxDepth, EdgeConditions edgeConditions, FilterConditions filterConditions)
        {
            QueryResponse result = null;

            // execute the call against the service app
            ExecuteOnChannel("QueryMap",
                delegate(IGlymaNodeServiceApp channel)
                {
                    result = channel.QueryMap(configuration, domainId, nodeId, maxDepth, edgeConditions, filterConditions);
                });

            return result;
        }

        public QueryResponse QueryMapPaged(GlymaSessionConfiguration configuration, Guid domainId, Guid nodeId, int maxDepth, EdgeConditions edgeConditions, FilterConditions filterConditions, int objectIndex)
        {
            QueryResponse result = null;

            // execute the call against the service app
            ExecuteOnChannel("QueryMapPaged",
                delegate(IGlymaNodeServiceApp channel)
                {
                    result = channel.QueryMapPaged(configuration, domainId, nodeId, maxDepth, edgeConditions, filterConditions, objectIndex);
                });

            return result;
        }

        public QueryResponse QueryMapByDomain(GlymaSessionConfiguration configuration, Guid domainId, int maxDepth, EdgeConditions edgeConditions, FilterConditions filterConditions)
        {
            QueryResponse result = null;

            // execute the call against the service app
            ExecuteOnChannel("QueryMapByDomain",
                delegate(IGlymaNodeServiceApp channel)
                {
                    result = channel.QueryMapByDomain(configuration, domainId, maxDepth, edgeConditions, filterConditions);
                });

            return result;
        }

        public SearchMapResult SearchMap(GlymaSessionConfiguration configuration, Guid domainId, Guid rootMapUid, SearchConditions conditions, int pageNumber, int pageSize)
        {
            if (pageNumber == 0)
            {
                pageNumber = 1;
            }

            if (pageSize == 0)
            {
                pageSize = 5;
            }

            SearchMapResult result = null;

            // execute the call against the service app
            ExecuteOnChannel("SearchMap",
                delegate(IGlymaNodeServiceApp channel)
                {
                    result = channel.SearchMap(configuration, domainId, rootMapUid, conditions, pageNumber, pageSize);
                });

            return result;
        }

        public MapParameter AddNode(GlymaSessionConfiguration configuration, Guid sessionId, MapParameter domainId, MapParameter rootMapId, NodeType nodeType, string originalId)
        {
            MapParameter result = null;

            // execute the call against the service app
            ExecuteOnChannel("AddNode",
                delegate(IGlymaNodeServiceApp channel)
                {
                    result = channel.AddNode(configuration, sessionId, domainId, rootMapId, nodeType, originalId);
                });

            return result;
        }

        public MapParameter AddBulkNode(GlymaSessionConfiguration configuration, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter rootMapId, NodeType nodeType, string originalId)
        {
            MapParameter result = null;

            // execute the call against the service app
            ExecuteOnChannel("AddBulkNode",
                delegate(IGlymaNodeServiceApp channel)
                {
                    result = channel.AddBulkNode(configuration, sessionId, responseParameter, domainId, rootMapId, nodeType, originalId);
                });

            return result;
        }

        public MapParameter DeleteNode(GlymaSessionConfiguration configuration, Guid sessionId, MapParameter domainId, MapParameter nodeId)
        {
            MapParameter result = null;

            // execute the call against the service app
            ExecuteOnChannel("DeleteNode",
                delegate(IGlymaNodeServiceApp channel)
                {
                    result = channel.DeleteNode(configuration, sessionId, domainId, nodeId);
                });

            return result;
        }

        public MapParameter DeleteBulkNode(GlymaSessionConfiguration configuration, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter nodeId)
        {
            MapParameter result = null;

            // execute the call against the service app
            ExecuteOnChannel("DeleteBulkNode",
                delegate(IGlymaNodeServiceApp channel)
                {
                    result = channel.DeleteBulkNode(configuration, sessionId, responseParameter, domainId, nodeId);
                });

            return result;
        }

        public MapParameter UpdateNode(GlymaSessionConfiguration configuration, Guid sessionId, MapParameter domainId, MapParameter nodeId, NodeType nodeType)
        {
            MapParameter result = null;

            // execute the call against the service app
            ExecuteOnChannel("UpdateNode",
                delegate(IGlymaNodeServiceApp channel)
                {
                    result = channel.UpdateNode(configuration, sessionId, domainId, nodeId, nodeType);
                });

            return result;
        }

        public MapParameter UpdateBulkNode(GlymaSessionConfiguration configuration, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter nodeId, NodeType nodeType)
        {
            MapParameter result = null;

            // execute the call against the service app
            ExecuteOnChannel("UpdateBulkNode",
                delegate(IGlymaNodeServiceApp channel)
                {
                    result = channel.UpdateBulkNode(configuration, sessionId, responseParameter, domainId, nodeId, nodeType);
                });

            return result;
        }

        public MapParameter AddRelationship(GlymaSessionConfiguration configuration, Guid sessionId, MapParameter domainId, MapParameter rootMapId, Dictionary<DescriptorType, MapParameter> nodes, RelationshipType relationshipType, string originalId)
        {
            MapParameter result = null;

            // execute the call against the service app
            ExecuteOnChannel("AddRelationship",
                delegate(IGlymaNodeServiceApp channel)
                {
                    result = channel.AddRelationship(configuration, sessionId, domainId, rootMapId, nodes, relationshipType, originalId);
                });

            return result;
        }

        public MapParameter AddBulkRelationship(GlymaSessionConfiguration configuration, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter rootMapId, Dictionary<DescriptorType, MapParameter> nodes, RelationshipType relationshipType, string originalId)
        {
            MapParameter result = null;

            // execute the call against the service app
            ExecuteOnChannel("AddBulkRelationship",
                delegate(IGlymaNodeServiceApp channel)
                {
                    result = channel.AddBulkRelationship(configuration, sessionId, responseParameter, domainId, rootMapId, nodes, relationshipType, originalId);
                });

            return result;
        }

        public MapParameter DeleteRelationship(GlymaSessionConfiguration configuration, Guid sessionId, MapParameter domainId, MapParameter relationshipId)
        {
            MapParameter result = null;

            // execute the call against the service app
            ExecuteOnChannel("DeleteRelationship",
                delegate(IGlymaNodeServiceApp channel)
                {
                    result = channel.DeleteRelationship(configuration, sessionId, domainId, relationshipId);
                });

            return result;
        }

        public MapParameter DeleteBulkRelationship(GlymaSessionConfiguration configuration, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter relationshipId)
        {
            MapParameter result = null;

            // execute the call against the service app
            ExecuteOnChannel("DeleteBulkRelationship",
                delegate(IGlymaNodeServiceApp channel)
                {
                    result = channel.DeleteBulkRelationship(configuration, sessionId, responseParameter, domainId, relationshipId);
                });

            return result;
        }

        public MapParameter UpdateRelationship(GlymaSessionConfiguration configuration, Guid sessionId, MapParameter domainId, MapParameter relationshipId, Dictionary<DescriptorType, MapParameter> nodes, RelationshipType relationshipType)
        {
            MapParameter result = null;

            // execute the call against the service app
            ExecuteOnChannel("UpdateRelationship",
                delegate(IGlymaNodeServiceApp channel)
                {
                    result = channel.UpdateRelationship(configuration, sessionId, domainId, relationshipId, nodes, relationshipType);
                });

            return result;
        }

        public MapParameter UpdateBulkRelationship(GlymaSessionConfiguration configuration, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter relationshipId, Dictionary<DescriptorType, MapParameter> nodes, RelationshipType relationshipType)
        {
            MapParameter result = null;

            // execute the call against the service app
            ExecuteOnChannel("UpdateBulkRelationship",
                delegate(IGlymaNodeServiceApp channel)
                {
                    result = channel.UpdateBulkRelationship(configuration, sessionId, responseParameter, domainId, relationshipId, nodes, relationshipType);
                });

            return result;
        }

        public MapParameter AddMetadata(GlymaSessionConfiguration configuration, Guid sessionId, MapParameter domainId, MapParameter rootMapId, MapParameter node, MapParameter relationship, DescriptorType descriptorType, MetadataType metadataType, string name, string value)
        {
            MapParameter result = null;

            // execute the call against the service app
            ExecuteOnChannel("AddMetadata",
                delegate(IGlymaNodeServiceApp channel)
                {
                    result = channel.AddMetadata(configuration, sessionId, domainId, rootMapId, node, relationship, descriptorType, metadataType, name, value);
                });

            return result;
        }

        public MapParameter AddBulkMetadata(GlymaSessionConfiguration configuration, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter rootMapId, MapParameter node, MapParameter relationship, DescriptorType descriptorType, MetadataType metadataType, string name, string value)
        {
            MapParameter result = null;

            // execute the call against the service app
            ExecuteOnChannel("AddBulkMetadata",
                delegate(IGlymaNodeServiceApp channel)
                {
                    result = channel.AddBulkMetadata(configuration, sessionId, responseParameter, domainId, rootMapId, node, relationship, descriptorType, metadataType, name, value);
                });

            return result;
        }

        public MapParameter DeleteMetadata(GlymaSessionConfiguration configuration, Guid sessionId, MapParameter domainId, MapParameter metadata)
        {
            MapParameter result = null;

            // execute the call against the service app
            ExecuteOnChannel("DeleteMetadata",
                delegate(IGlymaNodeServiceApp channel)
                {
                    result = channel.DeleteMetadata(configuration, sessionId, domainId, metadata);
                });

            return result;
        }

        public MapParameter DeleteBulkMetadata(GlymaSessionConfiguration configuration, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter metadata)
        {
            MapParameter result = null;

            // execute the call against the service app
            ExecuteOnChannel("DeleteBulkMetadata",
                delegate(IGlymaNodeServiceApp channel)
                {
                    result = channel.DeleteBulkMetadata(configuration, sessionId, responseParameter, domainId, metadata);
                });

            return result;
        }

        public MapParameter UpdateMetadata(GlymaSessionConfiguration configuration, Guid sessionId, MapParameter domainId, MapParameter metadata, string name, string value)
        {
            MapParameter result = null;

            // execute the call against the service app
            ExecuteOnChannel("UpdateMetadata",
                delegate(IGlymaNodeServiceApp channel)
                {
                    result = channel.UpdateMetadata(configuration, sessionId, domainId, metadata, name, value);
                });

            return result;
        }

        public MapParameter UpdateBulkMetadata(GlymaSessionConfiguration configuration, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter metadata, string name, string value)
        {
            MapParameter result = null;

            // execute the call against the service app
            ExecuteOnChannel("UpdateBulkMetadata",
                delegate(IGlymaNodeServiceApp channel)
                {
                    result = channel.UpdateBulkMetadata(configuration, sessionId, responseParameter, domainId, metadata, name, value);
                });

            return result;
        }

        public bool IsSessionCompleted(GlymaSessionConfiguration configuration, Guid sessionId)
        {
            bool result = false;

            // execute the call against the service app
            ExecuteOnChannel("IsSessionCompleted",
                delegate(IGlymaNodeServiceApp channel)
                {
                    result = channel.IsSessionCompleted(configuration, sessionId);
                });

            return result;
        }
        #endregion
    }
}
