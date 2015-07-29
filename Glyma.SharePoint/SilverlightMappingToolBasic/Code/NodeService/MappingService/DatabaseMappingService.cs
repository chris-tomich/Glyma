using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using SilverlightMappingToolBasic.MappingService;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Windows.Browser;

namespace SilverlightMappingToolBasic.MappingService
{
    public class DatabaseMappingService : INodeService
    {
        private Dictionary<Guid, INodeProxy> _cachedNodes;
        private MappingToolServiceClient _client = null;
        private string _mappingToolSvcUrl = null;

        public DatabaseMappingService()
        {
            _cachedNodes = new Dictionary<Guid, INodeProxy>();
        }

        public DatabaseMappingService(string mappingToolSvcUrl) : this()
        {
            _mappingToolSvcUrl = mappingToolSvcUrl;
        }

        #region Node Service Event Handlers

        private void OnPasteNodeCloneCompleted(object sender, PasteNodeCloneCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                SoapNode soapNode = e.Result;
                INodeProxy nodeProxy = new NodeProxy(soapNode);

                if (PasteNodeCloneCompleted != null)
                {
                    Point location = new Point(0, 0);
                    if (e.UserState != null)
                    {
                        location = ((MapLocationState)e.UserState).Location;
                    }
                    AddedNodeEventArgs anea = new AddedNodeEventArgs(nodeProxy, location);
                    PasteNodeCloneCompleted.Invoke(sender, anea);
                }
            }
        }

        private void OnAddNodeAsyncCompleted(object sender, AddNodeCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                SoapNode soapNode = e.Result;
                INodeProxy nodeProxy = new NodeProxy(soapNode);
                
                if (AddNodeCompleted != null)
                {
                    Point location = new Point(0, 0);
                    if (e.UserState != null)
                    {
                        location = ((MapLocationState)e.UserState).Location;
                    }
                    AddedNodeEventArgs anea = new AddedNodeEventArgs(nodeProxy, location);
                    AddNodeCompleted.Invoke(sender, anea);
                }
            }
        }

        private void OnConnectNodesCompleted(object sender, ConnectNodesCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                List<INodeProxy> nodes = new List<INodeProxy>();

                ConnectedNodesResult connectResult = e.Result;

                foreach (SoapNode soapNode in connectResult.Nodes.Values)
                {
                    if (_cachedNodes.ContainsKey(soapNode.Id))
                    {
                        _cachedNodes.Remove(soapNode.Id);
                    }

                    NodeProxy node = new NodeProxy(soapNode);
                    _cachedNodes.Add(soapNode.Id, node);
                    nodes.Add(node);
                }

                foreach (INodeProxy nodeProxy in nodes)
                {
                    foreach (IDescriptorProxy descriptorProxy in nodeProxy.Descriptors)
                    {
                        CompleteRelationship(descriptorProxy.Relationship);
                    }
                }

                ConnectedNodesEventArgs connectedNodesEventArgs = new ConnectedNodesEventArgs();
                connectedNodesEventArgs.Nodes = nodes.ToArray();
                connectedNodesEventArgs.Relationship = new RelationshipProxy(e.Result.Relationship);

                CompleteRelationship(connectedNodesEventArgs.Relationship);

                //When a node is connected via a MapContainerRelationship the UserState will be the location of the new node
                //on the map, it can't be stored in the db until the relationship exists since it's the contectual relationship
                //that determines where it is located in it's view in this map (it may be elsewhere in transclusions)
                if (e.UserState != null)
                {
                    INodeProxy nodeProxy = connectedNodesEventArgs.Nodes[1];
                    Point location = (Point)e.UserState;
                    if (location != null)
                    {
                        TypeManager typeManager = IoC.IoCContainer.GetInjectionInstance().GetInstance<TypeManager>();
                        IDescriptorTypeProxy descriptorType = null;
                        if (e.Result.Relationship.RelationshipType.Name == "TransclusionRelationship")
                        {
                            descriptorType = typeManager.GetDescriptorType("TransclusionMap");
                        }
                        else
                        {
                            descriptorType = typeManager.GetDescriptorType("From");
                        }
                        
                        MetadataContext xPositionKey = new MetadataContext()
                        {
                            NodeUid = nodeProxy.Id,
                            RelationshipUid = e.Result.Relationship.Id,
                            DescriptorTypeUid = descriptorType.Id,
                            MetadataName = "XPosition"
                        };
                        MetadataContext yPositionKey = new MetadataContext()
                        {
                            NodeUid = nodeProxy.Id,
                            RelationshipUid = e.Result.Relationship.Id,
                            DescriptorTypeUid = descriptorType.Id,
                            MetadataName = "YPosition"
                        };

                        if (nodeProxy.Metadata != null && nodeProxy.GetNodeMetadata(xPositionKey) != null)
                        {
                            nodeProxy.GetNodeMetadata(xPositionKey).MetadataValue = location.X.ToString();
                        }
                        else
                        {
                            MetadataTypeProxy metaDataTypeProxy = typeManager.GetMetadataType("double") as MetadataTypeProxy;
                            if (metaDataTypeProxy != null)
                            {
                                SoapMetadata soapMetadata = new SoapMetadata();
                                soapMetadata.MetadataName = "XPosition";
                                soapMetadata.MetadataType = metaDataTypeProxy.BaseSoapNodeType;
                                soapMetadata.MetadataValue = location.X.ToString();
                                nodeProxy.Metadata.Add(xPositionKey, soapMetadata);
                            }
                        }

                        if (nodeProxy.Metadata != null && nodeProxy.GetNodeMetadata(yPositionKey) != null)
                        {
                            nodeProxy.GetNodeMetadata(yPositionKey).MetadataValue = location.Y.ToString();
                        }
                        else
                        {
                            MetadataTypeProxy metaDataTypeProxy = typeManager.GetMetadataType("double") as MetadataTypeProxy;
                            if (metaDataTypeProxy != null)
                            {
                                SoapMetadata soapMetadata = new SoapMetadata();
                                soapMetadata.MetadataName = "YPosition";
                                soapMetadata.MetadataType = metaDataTypeProxy.BaseSoapNodeType;
                                soapMetadata.MetadataValue = location.Y.ToString();
                                nodeProxy.Metadata.Add(yPositionKey, soapMetadata);
                            }
                        }
                    }
                }

                if (ConnectNodesCompleted != null)
                {
                    ConnectNodesCompleted.Invoke(this, connectedNodesEventArgs);
                }
            }
        }

        private void CompleteRelationship(IRelationshipProxy relationship)
        {
            if (relationship != null)
            {
                foreach (DescriptorProxy descriptor in relationship.Descriptors)
                {
                    if (descriptor.Node == null && descriptor.NodeId != Guid.Empty)
                    {
                        if (_cachedNodes.ContainsKey(descriptor.NodeId))
                        {
                            descriptor.Node = _cachedNodes[descriptor.NodeId];
                            foreach (DescriptorProxy innerDesc in descriptor.Node.Descriptors)
                            {
                                CompleteRelationship(innerDesc.Relationship);
                            }
                        }
                    }
                }
            }
        }

        private void OnCreateNewDomainCompleted(object sender, CreateNewDomainCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                SoapNode domainNode = e.Result;

                ReturnedNodesEventArgs eventArgs = new ReturnedNodesEventArgs();
                eventArgs.SearchedNode = e.Result.Domain;
                eventArgs.Nodes = new INodeProxy[] { new NodeProxy(e.Result) };

                if (CreateNewDomainCompleted != null)
                {
                    CreateNewDomainCompleted.Invoke(this, eventArgs);
                }
            }
        }

        void OnCreateNewMapCompleted(object sender, CreateNewMapCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                ReturnedNodeIdEventArgs eventArgs = new ReturnedNodeIdEventArgs(e.Result);

                if (CreateNewMapCompleted != null)
                {
                    CreateNewMapCompleted.Invoke(this, eventArgs);
                }
            }
        }

        private void OnGetAllSoapTypesAsyncCompleted(object sender, GetAllSoapTypesCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                List<INodeTypeProxy> nodeTypeProxies = new List<INodeTypeProxy>();
                List<IDescriptorTypeProxy> descriptorTypeProxies = new List<IDescriptorTypeProxy>();
                List<IRelationshipTypeProxy> relationshipTypeProxies = new List<IRelationshipTypeProxy>();
                List<IMetadataTypeProxy> metadataTypeProxies = new List<IMetadataTypeProxy>();

                foreach (SoapTypeElement soapElement in e.Result)
                {
                    if (soapElement is SoapNodeType)
                    {
                        INodeTypeProxy nodeTypeProxy = new NodeTypeProxy(soapElement as SoapNodeType);
                        nodeTypeProxies.Add(nodeTypeProxy);
                    }
                    else if (soapElement is SoapDescriptorType)
                    {
                        IDescriptorTypeProxy descriptorTypeProxy = new DescriptorTypeProxy(soapElement as SoapDescriptorType);
                        descriptorTypeProxies.Add(descriptorTypeProxy);
                    }
                    else if (soapElement is SoapRelationshipType)
                    {
                        IRelationshipTypeProxy relationshipTypeProxy = new RelationshipTypeProxy(soapElement as SoapRelationshipType);
                        relationshipTypeProxies.Add(relationshipTypeProxy);
                    }
                    else if (soapElement is SoapMetadataType)
                    {
                        IMetadataTypeProxy metadataTypeProxy = new MetadataTypeProxy(soapElement as SoapMetadataType);
                        metadataTypeProxies.Add(metadataTypeProxy);
                    }
                }

                if (GetAllSoapTypesCompleted != null)
                {
                    ReturnedTypesEventArgs eventArgs = new ReturnedTypesEventArgs();
                    eventArgs.NodeTypes = nodeTypeProxies.ToArray();
                    eventArgs.DescriptorTypes = descriptorTypeProxies.ToArray();
                    eventArgs.RelationshipTypes = relationshipTypeProxies.ToArray();
                    eventArgs.MetadataTypes = metadataTypeProxies.ToArray();

                    GetAllSoapTypesCompleted.Invoke(this, eventArgs);
                }
            }
        }

        private void OnGetDomainNodeIdsCompleted(object sender, GetDomainNodeIdsCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                if (GetDomainNodeIdsCompleted != null)
                {
                    GetDomainNodeIdsCompleted.Invoke(this, new ReturnedNodeIdsEventArgs(e.Result));
                }
            }
        }

        private void OnGetDomainNodeIdCompleted(object sender, GetDomainNodeIdCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                if (GetDomainNodeIdCompleted != null)
                {
                    GetDomainNodeIdCompleted.Invoke(this, new ReturnedNodeIdEventArgs(e.Result));
                }
            }
        }

        private void OnGetDomainMapNodesCompleted(object sender, GetDomainMapNodesCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                if (GetDomainMapNodesCompleted != null)
                {
                    ReturnedMapNodesEventArgs eventArgs = new ReturnedMapNodesEventArgs(e);
                    GetDomainMapNodesCompleted.Invoke(this, eventArgs);
                }
            }
        }

        private void OnGetContainerMapNodeIdCompleted(object sender, GetContainerMapNodeIdCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                if (GetContainerMapNodeIdCompleted != null)
                {
                    ReturnedNodeIdEventArgs eventArgs = new ReturnedNodeIdEventArgs(e.Result);
                    GetContainerMapNodeIdCompleted.Invoke(this, eventArgs);
                }
            }
        }

        private void OnGetMapsNodesCompleted(object sender, GetMapsNodesCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                List<INodeProxy> nodes = BuildNodeList(e.Result);

                ReturnedNodesEventArgs eventArgs = new ReturnedNodesEventArgs(e.Result.SearchedNode, nodes.ToArray());

                if (GetMapsNodesCompleted != null)
                {
                    GetMapsNodesCompleted.Invoke(this, eventArgs);
                }
            }
        }

        private void OnGetNodesCompleted(object sender, GetNodesCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                List<INodeProxy> nodes = new List<INodeProxy>();

                foreach (SoapNode soapNode in e.Result.Values)
                {
                    NodeProxy node = new NodeProxy(soapNode);

                    nodes.Add(node);
                    _cachedNodes.Add(node.Id, node);
                }

                ReturnedNodesEventArgs eventArgs = new ReturnedNodesEventArgs(Guid.Empty, nodes.ToArray());

                if (GetNodeByIdCompleted != null)
                {
                    GetNodeByIdCompleted.Invoke(this, eventArgs);
                }
            }
        }

        //private void OnGetNodesWithConditionsCompleted(object sender, GetNodesWithConditionsCompletedEventArgs e)
        //{
        //    if (e.Error == null)
        //    {
        //        List<INodeProxy> nodes = BuildNodeList(e.Result);

        //        ReturnedNodesEventArgs eventArgs = new ReturnedNodesEventArgs(e.Result.SearchedNode, nodes.ToArray());

        //        if (GetNodesWithConditionsCompleted != null)
        //        {
        //            GetNodesWithConditionsCompleted.Invoke(this, eventArgs);
        //        }
        //    }
        //}

        private void OnGetRelatedNodesByIdAsyncCompleted(object sender, GetRelatedNodesCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                List<INodeProxy> nodes = BuildNodeList(e.Result);

                ReturnedNodesEventArgs eventArgs = new ReturnedNodesEventArgs(e.Result.SearchedNode, nodes.ToArray());

                if (GetRelatedNodesByIdCompleted != null)
                {
                    GetRelatedNodesByIdCompleted.Invoke(this, eventArgs);
                }
            }
        }

        private void OnUpdateNodeMetadataCompleted(object sender, UpdateNodeMetadataCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                if (_cachedNodes.ContainsKey(e.Result.Id))
                {
                    _cachedNodes.Remove(e.Result.Id);
                }

                NodeProxy node = new NodeProxy(e.Result);
                _cachedNodes.Add(e.Result.Id, node);

                ReturnedNodesEventArgs returnedNodesEventArgs = new ReturnedNodesEventArgs();
                returnedNodesEventArgs.Nodes = new INodeProxy[] { node };

                if (UpdateNodeMetadataCompleted != null)
                {
                    UpdateNodeMetadataCompleted.Invoke(this, returnedNodesEventArgs);
                }
            }
        }

        private void OnRenameNodeMetadataCompleted(object sender, RenameNodeMetadataCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                ResultEventArgs resultEventArgs = new ResultEventArgs(e.Result);

                if (RenameNodeMetadataCompleted != null)
                {
                    RenameNodeMetadataCompleted.Invoke(this, resultEventArgs);
                }
            }
        }

        private void OnDeleteMetadataCompleted(object sender, DeleteMetadataCompletedEventArgs e)
        {
            bool successful = false;
            if (e.Error == null)
            {
                successful = e.Result.DeleteSuccessful;

                if (DeleteMetadataCompleted != null)
                {
                    if (successful)
                    {
                        DeleteMetadataCompleted.Invoke(this, new DeleteEventArgs(successful, e.Result.DeletedId));
                    }
                    else
                    {
                        DeleteMetadataCompleted.Invoke(this, new DeleteEventArgs(successful, Guid.Empty));
                    }
                }
            }
        }

        private void OnDeleteNodeCompleted(object sender, DeleteNodeCompletedEventArgs e)
        {
            bool successful = false;
            if (e.Error == null)
            {
                successful = e.Result.DeleteSuccessful;

                if (DeleteNodeCompleted != null)
                {
                    if (successful)
                    {
                        DeleteNodeCompleted.Invoke(this, new DeleteEventArgs(successful, e.Result.DeletedId));
                    }
                    else
                    {
                        DeleteNodeCompleted.Invoke(this, new DeleteEventArgs(successful, Guid.Empty));
                    }
                }
            }
        }

        private void OnDeleteNodeTransclusionCompleted(object sender, DeleteNodeTransclusionCompletedEventArgs e)
        {
            bool successful = false;
            if (e.Error == null)
            {
                successful = e.Result.DeleteSuccessful;

                if (DeleteNodeCompleted != null)
                {
                    DeleteNodeCompleted.Invoke(this, new DeleteEventArgs(successful, e.Result.DeletedId));
                }
            }
        }

        private void OnDeleteRelationshipCompleted(object sender, DeleteRelationshipCompletedEventArgs e)
        {
            bool successful = false;
            if (e.Error == null)
            {
                successful = e.Result.DeleteSuccessful;

                if (DeleteRelationshipCompleted != null)
                {
                    DeleteRelationshipCompleted.Invoke(this, new DeleteEventArgs(successful, e.Result.DeletedId));
                }
            }
        }

        #endregion

        /// <summary>
        /// Builds out the nodes and descriptors on the client side. Adds to a _cachedNodes list and keeps it updated.
        /// </summary>
        /// <param name="relatedNodesResult"></param>
        /// <returns></returns>
        private List<INodeProxy> BuildNodeList(RelatedNodesSearchResult relatedNodesResult)
        {
            List<INodeProxy> nodes = new List<INodeProxy>();

            if (relatedNodesResult.Nodes.Count > 0)
            {
                foreach (SoapNode soapNode in relatedNodesResult.Nodes.Values)
                {
                    if (_cachedNodes.ContainsKey(soapNode.Id))
                    {
                        _cachedNodes.Remove(soapNode.Id);
                    }
                    NodeProxy node = new NodeProxy(soapNode);
                    _cachedNodes.Add(soapNode.Id, node);
                }

                foreach (SoapNode soapNode in relatedNodesResult.Nodes.Values)
                {
                    foreach (SoapRelationship relationship in soapNode.Relationships.Values)
                    {
                        RelationshipProxy relationshipProxy = new RelationshipProxy(relationship);

                        foreach (DescriptorProxy descriptor in relationshipProxy.Descriptors)
                        {
                            if (_cachedNodes.ContainsKey(descriptor.NodeId))
                            {
                                INodeProxy connectedNode = _cachedNodes[descriptor.NodeId];
                                descriptor.Node = connectedNode;
                                if (!connectedNode.Descriptors.Contains(descriptor))
                                {
                                    connectedNode.Descriptors.Add(descriptor);
                                }
                            }
                        }
                    }
                }

                foreach (SoapNode soapNode in relatedNodesResult.Nodes.Values)
                {
                    nodes.Add(_cachedNodes[soapNode.Id]);
                }

                foreach (INodeProxy nodeProxy in nodes)
                {
                    foreach (IDescriptorProxy descriptorProxy in nodeProxy.Descriptors)
                    {
                        CompleteRelationship(descriptorProxy.Relationship);
                    }
                }
            }

            return nodes;
        }

        #region INodeService Members
        public event EventHandler<AddedNodeEventArgs> PasteNodeCloneCompleted;
        public event EventHandler<AddedNodeEventArgs> AddNodeCompleted;
        public event EventHandler<ConnectedNodesEventArgs> ConnectNodesCompleted;
        public event EventHandler<ReturnedTypesEventArgs> GetAllSoapTypesCompleted;
        public event EventHandler<ReturnedNodeIdsEventArgs> GetDomainNodeIdsCompleted;
        public event EventHandler<ReturnedNodeIdEventArgs> GetDomainNodeIdCompleted;
        public event EventHandler<ReturnedMapNodesEventArgs> GetDomainMapNodesCompleted;
        public event EventHandler<ReturnedNodesEventArgs> GetNodeByIdCompleted;
        public event EventHandler<ReturnedNodesEventArgs> GetMapsNodesCompleted;
        public event EventHandler<ReturnedNodesEventArgs> GetRelatedNodesByIdCompleted;
        public event EventHandler<ReturnedNodesEventArgs> UpdateNodeMetadataCompleted;
        public event EventHandler<DeleteEventArgs> DeleteNodeCompleted;
        public event EventHandler<DeleteEventArgs> DeleteRelationshipCompleted;
        public event EventHandler<ReturnedNodesEventArgs> CreateNewDomainCompleted;
        public event EventHandler<ReturnedNodeIdEventArgs> CreateNewMapCompleted;
        public event EventHandler<ReturnedNodeIdEventArgs> GetContainerMapNodeIdCompleted;
        public event EventHandler<DeleteEventArgs> DeleteMetadataCompleted;
        public event EventHandler<ResultEventArgs> RenameNodeMetadataCompleted;

        public void CreateNewDomain(string domainName)
        {
            Client.CreateNewDomainAsync(domainName);
        }

        public void CreateNewMap(Guid domainId, string mapName)
        {
            Client.CreateNewMapAsync(domainId, mapName);
        }

        public void GetContainerMapNodeId(Guid domain, Guid nodeId)
        {
            Client.GetContainerMapNodeIdAsync(domain, nodeId);
        }

        public void AddNode(Guid domainId, INodeTypeProxy nodeType, string originalId, Point location)
        {
            MapLocationState addParam = new MapLocationState() { Location = location};
            Client.AddNodeAsync(domainId, new SoapNodeType() { Id = nodeType.Id, Name = nodeType.Name }, originalId, addParam);
        }

        public void PasteNodeClone(Guid domainId, Guid copiedNodeId, Point location)
        {
            MapLocationState cloneParam = new MapLocationState() { Location = location };
            Client.PasteNodeCloneAsync(domainId, copiedNodeId, cloneParam);
        }

        public void DeleteMetadata(MetadataContext context)
        {
            Client.DeleteMetadataAsync(context);
        }

        public void DeleteNode(Guid domainId, Guid nodeIdToDelete)
        {
            Client.DeleteNodeAsync(domainId, nodeIdToDelete);
        }

        public void DeleteRelationship(Guid domainId, Guid relationshipId)
        {
            Client.DeleteRelationshipAsync(domainId, relationshipId);
        }

        public void DeleteNodePromoteTransclusion(Guid domainId, Guid currentMapId, INodeProxy originalNode)
        {
            Client.DeleteNodePromoteTransclusionAsync(domainId, currentMapId, originalNode.Id);
            ////Find a transclusion version of the node
            //Guid newMasterMapId = Guid.Empty;
            //Guid relationshipToPromote = Guid.Empty;
            //foreach (IDescriptorProxy descriptor in originalNode.Descriptors.GetByDescriptorTypeName("To"))
            //{
            //    if (descriptor.Relationship.RelationshipType.Name == "TransclusionRelationship")
            //    {
            //        foreach (IDescriptorProxy transMapDescriptor in descriptor.Relationship.Descriptors.GetByDescriptorTypeName("TransclusionMap"))
            //        {
            //            if (currentMapId != transMapDescriptor.NodeId)
            //            {
            //                newMasterMapId = transMapDescriptor.NodeId;
            //                relationshipToPromote = descriptor.Relationship.Id;
            //                break;
            //            }
            //        }
            //    }
            //    if (newMasterMapId != Guid.Empty)
            //    {
            //        break;
            //    }
            //}

            //if (newMasterMapId != Guid.Empty)
            //{
            //    //Find original Map Container Relationship
            //    Guid originalMapNodeContainer = Guid.Empty;
            //    foreach (IDescriptorProxy descriptor in originalNode.Descriptors.GetByDescriptorTypeName("From"))
            //    {
            //        if (descriptor.Relationship.RelationshipType.Name == "MapContainerRelationship")
            //        {
            //            originalMapNodeContainer = descriptor.Relationship.Id;
            //            break;
            //        }
            //    }

            //    if (originalMapNodeContainer != Guid.Empty)
            //    {
            //        //Delete the original Map Container Relationship and remove it...no metadata from it is required.
            //        DeleteRelationship(domainId, originalMapNodeContainer);
                    

            //        //Get the X, Y Position metadata from the Transclusion Relationship being changed to a Map Container Relationship
            //        TypeManager typeManager = IoC.IoCContainer.GetInjectionInstance().GetInstance<TypeManager>();
            //        IDescriptorTypeProxy transclusionMapDescriptorTypeProxy = typeManager.GetDescriptorType("TransclusionMap");
            //        MetadataContext xPositionContext = new MetadataContext()
            //        {
            //            MetadataName = "XPosition",
            //            NodeUid = originalNode.Id,
            //            RelationshipUid = relationshipToPromote,
            //            DescriptorTypeUid = transclusionMapDescriptorTypeProxy.Id
            //        };
            //        MetadataContext yPositionContext = new MetadataContext()
            //        {
            //            MetadataName = "YPosition",
            //            NodeUid = originalNode.Id,
            //            RelationshipUid = relationshipToPromote,
            //            DescriptorTypeUid = transclusionMapDescriptorTypeProxy.Id
            //        };
            //        SoapMetadata xPositionMetadata = originalNode.GetMetadataItem(xPositionContext);
            //        SoapMetadata yPositionMetadata = originalNode.GetMetadataItem(yPositionContext);
                    
            //        IDescriptorTypeProxy fromDescriptorTypeProxy = typeManager.GetDescriptorType("From");
            //        IDescriptorTypeProxy toDescriptorTypeProxy = typeManager.GetDescriptorType("To");
            //        IRelationshipTypeProxy mapContainerRelationshipType = typeManager.GetRelationshipType("MapContainerRelationship");
            //        IRelationshipTypeProxy fromToRelationshipType = typeManager.GetRelationshipType("FromToRelationship");
            //        Dictionary<IDescriptorTypeProxy, Guid> nodes = new Dictionary<IDescriptorTypeProxy, Guid>();
            //        nodes.Load(toDescriptorTypeProxy, newMasterMapId);
            //        nodes.Load(fromDescriptorTypeProxy, originalNode.Id);

            //        //Connect new MapContainerRelationship using the new Map Id
            //        ConnectNodesAsync(originalNode.Domain, nodes, mapContainerRelationshipType,
            //            new Point(Double.Parse(xPositionMetadata.MetadataValue), Double.Parse(yPositionMetadata.MetadataValue)), null);

            //        //Delete the old transclusion map relationship
            //        DeleteRelationship(domainId, relationshipToPromote);

            //        //Convert the Transclusion Map Relationships that are between the node (that was previously transcluded) and other nodes on that map.
            //        //First collect all the transclusion relationships on the map that is now the container of the node.
            //        List<IRelationshipProxy> relationshipsToConvert = new List<IRelationshipProxy>();
            //        foreach (IDescriptorProxy descriptor in originalNode.Descriptors)
            //        {
            //            if (descriptor.Relationship.RelationshipType.Name == "TransclusionRelationship" && descriptor.Relationship.Descriptors.Count == 3)
            //            {
            //                foreach (IDescriptorProxy transclusionMapDesc in descriptor.Relationship.Descriptors.GetByDescriptorTypeName("TransclusionMap"))
            //                {
            //                    if (transclusionMapDesc.NodeId == newMasterMapId)
            //                    {
            //                        relationshipsToConvert.Load(descriptor.Relationship);
            //                    }
            //                }
            //            }
            //        }
            //        //Then create the new FromToRelationships
            //        foreach (IRelationshipProxy transclusionRelationship in relationshipsToConvert)
            //        {
            //            IDescriptorProxy fromDescritorProxy = null;
            //            IDescriptorProxy toDescriptorProxy = null;
            //            foreach (IDescriptorProxy fromDescriptor in transclusionRelationship.Descriptors.GetByDescriptorTypeName("From")) 
            //            {
            //                fromDescritorProxy = fromDescriptor;
            //                break; //there will only be one anyway
            //            }
            //            foreach (IDescriptorProxy toDescriptor in transclusionRelationship.Descriptors.GetByDescriptorTypeName("To"))
            //            {
            //                toDescriptorProxy = toDescriptor;
            //                break;  //there will only be one anyway
            //            }
            //            nodes.Clear(); //clear the temporary relationship building nodes dictionary before use
            //            nodes.Load(toDescriptorTypeProxy, toDescriptorProxy.NodeId);
            //            nodes.Load(fromDescriptorTypeProxy, fromDescritorProxy.NodeId);

            //            ConnectNodesAsync(originalNode.Domain, nodes, fromToRelationshipType, null);
            //            DeleteRelationship(domainId, transclusionRelationship.Id);
            //        }
            //    }

            //}
        }

        public void DeleteNodeTransclusion(Guid domainId, Guid currentMapId, INodeProxy transcludedNode)
        {
            Client.DeleteNodeTransclusionAsync(domainId, currentMapId, transcludedNode.Id);
        }

        public void ConnectNodesAsync(Guid domainId, Dictionary<IDescriptorTypeProxy, Guid> nodes, IRelationshipTypeProxy relationshipType, string originalId)
        {
            Dictionary<SoapDescriptorType, Guid> soapNodes = new Dictionary<SoapDescriptorType, Guid>();

            foreach (KeyValuePair<IDescriptorTypeProxy, Guid> nodeDetails in nodes)
            {
                SoapDescriptorType soapDescriptorType = new SoapDescriptorType() { Id = nodeDetails.Key.Id, Name = nodeDetails.Key.Name };
                soapNodes.Add(soapDescriptorType, nodeDetails.Value);
            }

            SoapRelationshipType soapRelationship = new SoapRelationshipType() { Id = relationshipType.Id, Name = relationshipType.Name };

            Client.ConnectNodesAsync(domainId, soapNodes, soapRelationship, originalId);
        }

        public void ConnectNodesAsync(Guid domainId, Dictionary<IDescriptorTypeProxy, Guid> nodes, IRelationshipTypeProxy relationshipType, Point location, string originalId)
        {
            Dictionary<SoapDescriptorType, Guid> soapNodes = new Dictionary<SoapDescriptorType, Guid>();

            foreach (KeyValuePair<IDescriptorTypeProxy, Guid> nodeDetails in nodes)
            {
                SoapDescriptorType soapDescriptorType = new SoapDescriptorType() { Id = nodeDetails.Key.Id, Name = nodeDetails.Key.Name };
                soapNodes.Add(soapDescriptorType, nodeDetails.Value);
            }

            SoapRelationshipType soapRelationship = new SoapRelationshipType() { Id = relationshipType.Id, Name = relationshipType.Name };

            Client.ConnectNodesAsync(domainId, soapNodes, soapRelationship, originalId, location);
        }

        public void GetAllSoapTypesAsync()
        {
            Client.GetAllSoapTypesAsync();
        }

        public void GetDomainNodeIdsAsync()
        {
            Client.GetDomainNodeIdsAsync();
        }

        public void GetDomainNodeIdAsync(Guid domainId)
        {
            Client.GetDomainNodeIdAsync(domainId);
        }

        public void GetDomainMapNodes(Guid domainId)
        {
            Client.GetDomainMapNodesAsync(domainId);
        }

        public void GetMapsNodes(Guid domainId, Guid mapNodeId)
        {
            Client.GetMapsNodesAsync(domainId, mapNodeId);
        }

        public void GetNodeByIdAsync(Guid nodeId, Guid mapId)
        {
            if (_cachedNodes.ContainsKey(nodeId))
            {
                ReturnedNodesEventArgs returnedNodesEventArgs = new ReturnedNodesEventArgs();
                List<INodeProxy> returnedNodes = new List<INodeProxy>();
                returnedNodes.Add(_cachedNodes[nodeId]);
                returnedNodesEventArgs.Nodes = returnedNodes.ToArray();

                if (GetNodeByIdCompleted != null)
                {
                    GetNodeByIdCompleted.Invoke(this, returnedNodesEventArgs);
                }
            }
            else
            {
                Dictionary<Guid, Guid> nodes = new Dictionary<Guid, Guid>();
                nodes.Add(nodeId, mapId);

                Client.GetNodesAsync(nodes);
            }
        }

        //public void GetNodesWithConditions(Guid domainId, Guid nodeId, List<Condition> conditions)
        //{
        //    Client.GetNodesWithConditionsAsync(domainId, nodeId, -1, conditions);
        //}

        public void GetRelatedNodesByIdAsync(Guid domainId, Guid nodeId, int depth)
        {
            Client.GetRelatedNodesAsync(domainId, nodeId, depth);
        }

        public void RenameNodeMetadataAsync(Guid domainId, Guid nodeId, Guid relationshipId, IDescriptorTypeProxy descriptorTypeProxy, string originalMetadataName, string newMetadataName)
        {
            Guid soapRelationshipId = Guid.Empty;

            if (relationshipId != Guid.Empty)
            {
                soapRelationshipId = relationshipId;
            }

            SoapDescriptorType soapDescriptorType = null;

            if (descriptorTypeProxy != null)
            {
                soapDescriptorType = new SoapDescriptorType() { Id = descriptorTypeProxy.Id, Name = descriptorTypeProxy.Name };
            }
            Client.RenameNodeMetadataAsync(domainId, nodeId, soapRelationshipId, soapDescriptorType, originalMetadataName, newMetadataName);
        }

        public void UpdateNodeMetadataAsync(Guid domainId, Guid nodeId, Guid relationshipId, IDescriptorTypeProxy descriptorTypeProxy, string metadataName, string metadataValue, IMetadataTypeProxy metadataType)
        {
            Guid soapRelationshipId = Guid.Empty;

            if (relationshipId != Guid.Empty)
            {
                soapRelationshipId = relationshipId;
            }

            SoapDescriptorType soapDescriptorType = null;

            if (descriptorTypeProxy != null)
            {
                soapDescriptorType = new SoapDescriptorType() { Id = descriptorTypeProxy.Id, Name = descriptorTypeProxy.Name };
            }

            SoapMetadataType soapMetadataType = null;

            if (metadataType != null)
            {
                soapMetadataType = new SoapMetadataType() { Id = metadataType.Id, Name = metadataType.Name };
            }

            Client.UpdateNodeMetadataAsync(domainId, nodeId, soapRelationshipId, soapDescriptorType, metadataName, metadataValue, soapMetadataType);
        }

        private object _clientLock = new object();
        private MappingToolServiceClient Client
        {
            get
            {
                if (_client == null)
                {
                    lock (_clientLock)
                    {
                        if (_client == null)
                        {
                            if (string.IsNullOrEmpty(_mappingToolSvcUrl))
                            {
                                _mappingToolSvcUrl = HtmlPage.Window.Invoke("getMappingToolSvcUrl") as string;
                            }

                            BasicHttpBinding binding = new BasicHttpBinding();
                            binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
                            binding.MaxReceivedMessageSize = 2147483647;

                            EndpointAddress address = new EndpointAddress(new Uri(_mappingToolSvcUrl));
                            _client = new MappingToolServiceClient(binding, address);

                            _client.PasteNodeCloneCompleted += new EventHandler<PasteNodeCloneCompletedEventArgs>(OnPasteNodeCloneCompleted);
                            _client.AddNodeCompleted += new EventHandler<AddNodeCompletedEventArgs>(OnAddNodeAsyncCompleted);
                            _client.DeleteMetadataCompleted += new EventHandler<DeleteMetadataCompletedEventArgs>(OnDeleteMetadataCompleted);
                            _client.DeleteNodeCompleted += new EventHandler<DeleteNodeCompletedEventArgs>(OnDeleteNodeCompleted);
                            _client.DeleteNodeTransclusionCompleted += new EventHandler<DeleteNodeTransclusionCompletedEventArgs>(OnDeleteNodeTransclusionCompleted);
                            _client.DeleteRelationshipCompleted += new EventHandler<DeleteRelationshipCompletedEventArgs>(OnDeleteRelationshipCompleted);
                            _client.ConnectNodesCompleted += new EventHandler<ConnectNodesCompletedEventArgs>(OnConnectNodesCompleted);
                            _client.GetAllSoapTypesCompleted += new EventHandler<GetAllSoapTypesCompletedEventArgs>(OnGetAllSoapTypesAsyncCompleted);
                            _client.GetDomainNodeIdsCompleted += new EventHandler<GetDomainNodeIdsCompletedEventArgs>(OnGetDomainNodeIdsCompleted);
                            _client.GetDomainNodeIdCompleted += new EventHandler<GetDomainNodeIdCompletedEventArgs>(OnGetDomainNodeIdCompleted);
                            _client.GetDomainMapNodesCompleted += new EventHandler<GetDomainMapNodesCompletedEventArgs>(OnGetDomainMapNodesCompleted);
                            _client.GetMapsNodesCompleted += new EventHandler<GetMapsNodesCompletedEventArgs>(OnGetMapsNodesCompleted);
                            _client.GetNodesCompleted += new EventHandler<GetNodesCompletedEventArgs>(OnGetNodesCompleted);
                            _client.GetContainerMapNodeIdCompleted += new EventHandler<GetContainerMapNodeIdCompletedEventArgs>(OnGetContainerMapNodeIdCompleted);
                            //_client.GetNodesWithConditionsCompleted += new EventHandler<GetNodesWithConditionsCompletedEventArgs>(OnGetNodesWithConditionsCompleted);
                            _client.GetRelatedNodesCompleted += new EventHandler<GetRelatedNodesCompletedEventArgs>(OnGetRelatedNodesByIdAsyncCompleted);
                            _client.RenameNodeMetadataCompleted += new EventHandler<RenameNodeMetadataCompletedEventArgs>(OnRenameNodeMetadataCompleted);
                            _client.UpdateNodeMetadataCompleted += new EventHandler<UpdateNodeMetadataCompletedEventArgs>(OnUpdateNodeMetadataCompleted);
                            _client.CreateNewDomainCompleted += new EventHandler<CreateNewDomainCompletedEventArgs>(OnCreateNewDomainCompleted);
                            _client.CreateNewMapCompleted += new EventHandler<CreateNewMapCompletedEventArgs>(OnCreateNewMapCompleted);
                        }
                    }
                }
                return _client;
            }
        }
        
        #endregion
    }
}
