using System;
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

namespace SilverlightMappingToolBasic
{
    public class NodesEventArgs : EventArgs
    {
        public NodesEventArgs()
        {
            EventType = null;
            ContextNodeId = Guid.Empty;
            ContextNode = null;
            ContextRelationship = null;
            ContextRelationshipId = Guid.Empty;
            NodeIds = null;
            Nodes = null;
        }

        public NodesEventArgs(string eventType, IRelationshipProxy contextRelationship)
        {
            EventType = eventType;
            ContextRelationship = contextRelationship;
        }

        public NodesEventArgs(string eventType, Guid contextRelationshipId)
        {
            EventType = eventType;
            ContextRelationshipId = contextRelationshipId;
        }

        public NodesEventArgs(string eventType, INodeProxy contextNode, INodeProxy[] nodes)
            : this()
        {
            EventType = eventType;
            ContextNode = contextNode;
            Nodes = nodes;
        }

        public NodesEventArgs(string eventType, Guid contextNodeId, INodeProxy[] nodes)
            : this()
        {
            EventType = eventType;
            ContextNodeId = contextNodeId;
            Nodes = nodes;
        }

        public NodesEventArgs(string eventType, INodeProxy contextNode, Guid[] nodeIds)
            : this()
        {
            EventType = eventType;
            ContextNode = contextNode;
            NodeIds = nodeIds;
        }

        public NodesEventArgs(string eventType, Guid contextNodeId, Guid[] nodeIds)
            : this()
        {
            EventType = eventType;
            ContextNodeId = contextNodeId;
            NodeIds = nodeIds;
        }

        public string EventType
        {
            get;
            set;
        }

        public Guid ContextNodeId
        {
            get;
            set;
        }

        public INodeProxy ContextNode
        {
            get;
            set;
        }

        public Guid ContextRelationshipId
        {
            get;
            set;
        }

        public IRelationshipProxy ContextRelationship
        {
            get;
            set;
        }

        public Guid[] NodeIds
        {
            get;
            set;
        }

        public INodeProxy[] Nodes
        {
            get;
            set;
        }
    }

    public abstract class NodeNavigator : INodeNavigator
    {
        private static NavigationHistory _navHistory = new NavigationHistory();

        private Guid _currentFocalNode;
        private List<MetadataFilter> _metadataFilters;

        public NodeNavigator()
        {
            
        }

        #region INodeNavigator Members

        public Guid DomainId
        {
            get;
            protected set;
        }

        public Guid FocalNodeId
        {
            get
            {
                return _currentFocalNode;
            }
            set
            {
                _currentFocalNode = value;
            }
        }

        public INodeProxy FocalNode
        {
            get;
            protected set;
        }

        public INodeService NodeService
        {
            get;
            protected set;
        }

        public NavigationHistory History
        {
            get
            {
                return _navHistory;
            }
        }

        public ThemeManager ThemeManagementObject
        {
            get;
            protected set;
        }

        public List<MetadataFilter> MetadataFilters
        {
            get
            {
                if (_metadataFilters == null)
                {
                    _metadataFilters = new List<MetadataFilter>();
                }
                return _metadataFilters;
            }
            set
            {
                _metadataFilters = value;
            }
        }

        public abstract event EventHandler<NodesEventArgs> GetCurrentNodesCompleted;

        public abstract event EventHandler<NodesEventArgs> PasteNodeCloneCompleted;

        public abstract event EventHandler<AddedNodeEventArgs> AddNodeCompleted;

        public abstract event EventHandler<NodesEventArgs> ConnectNodesCompleted;

        public abstract event EventHandler<NodesEventArgs> UpdateNodeMetadataCompleted;

        public abstract event EventHandler<NodesEventArgs> DeleteNodeCompleted;

        public abstract event EventHandler<NodesEventArgs> DeleteRelationshipCompleted;

        public event EventHandler NavigationCompleted;

        public virtual void SetCurrentNode(INodeProxy node)
        {
            //put the last focal node into the history before setting the new one
            SetHistory(node.Name, node);
            if (NavigationCompleted != null)
            {
                NavigationCompleted.Invoke(this, new EventArgs());
            }

            //Set the new current focal node.
            FocalNode = node;
            _currentFocalNode = FocalNode.Id;
        }

        /// <summary>
        /// This method is only ever called on the intial drawing of the map before any nodes are retrieved.
        /// and when using the breadcrumb bar to go back through the history
        /// </summary>
        /// <param name="nodeId">The NodeUid for the current map node</param>
        public virtual void SetCurrentNode(Guid nodeId)
        {
            if (!_navHistory.HasHistory)
            {
                SetHistory(null, new NodeProxy() { Id = nodeId });
            }
            if (NavigationCompleted != null)
            {
                NavigationCompleted.Invoke(this, new EventArgs());
            }
            _currentFocalNode = nodeId;
            if (_navHistory.HasHistory)
            {
                FocalNode = _navHistory.Stack.Peek().Node;
            }
        }

        private void SetHistory(string name, INodeProxy node)
        {
            if (node != null)
            {
                if (name == null)
                {
                    name = "Home";
                }
                NodeHistoryElement item = new NodeHistoryElement() { Name = name, Node = node };
                _navHistory.AddToHistory(item);
            }
        }

        public virtual List<INodeProxy> FiltersResults(INodeProxy[] nodes)
        {
            lock (MetadataFilters)
            {
                List<INodeProxy> result = new List<INodeProxy>();
                if (MetadataFilters != null && nodes != null)
                {
                    if (MetadataFilters.Count == 0)
                    {
                        //if there are no filters it's quicker to return the full list.
                        result.AddRange(nodes);
                        return result;
                    }

                    foreach (INodeProxy node in nodes)
                    {
                        bool includeNode = true;
                        foreach (MetadataFilter filter in MetadataFilters)
                        {
                            if (!filter.HasMatch(node))
                            {
                                includeNode = false;
                                break;
                            }
                        }
                        if (includeNode)
                        {
                            result.Add(node);
                        }
                    }
                }
                return result;
            }
        }

        public abstract void GetCurrentNodesAsync();

        public virtual void PasteNodeClone(Guid copiedNodeId, Point location)
        {
            NodeService.PasteNodeClone(DomainId, copiedNodeId, location);
        }

        public virtual void AddNode(INodeTypeProxy nodeType, string originalId, Point location)
        {
            NodeService.AddNode(DomainId, nodeType, originalId, location);
        }

        public virtual void DeleteNodes(INodeProxy[] nodesToDelete)
        {
            foreach (INodeProxy nodeProxy in nodesToDelete)
            {
                if (this.FocalNodeId != nodeProxy.ParentMapNodeUid)
                {
                    NodeService.DeleteNodeTransclusion(this.DomainId, this.FocalNodeId, nodeProxy);
                }
                else
                {
                    if (nodeProxy.TransclusionCount == 1)
                    {
                        NodeService.DeleteNode(this.DomainId, nodeProxy.Id);
                    }
                    else
                    {
                        NodeService.DeleteNodePromoteTransclusion(this.DomainId, this.FocalNodeId, nodeProxy);
                    }
                }
            }
        }

        public virtual void DeleteRelationships(IRelationshipProxy[] relationshipsToDelete)
        {
            foreach (IRelationshipProxy relationship in relationshipsToDelete)
            {
                NodeService.DeleteRelationship(DomainId, relationship.Id);
            }
        }

        public virtual void ConnectNodesAsync(Dictionary<IDescriptorTypeProxy, Guid> nodes, IRelationshipTypeProxy relationshipType, string originalId)
        {
            NodeService.ConnectNodesAsync(DomainId, nodes, relationshipType, originalId);
        }

        public virtual void ConnectNodesAsync(Dictionary<IDescriptorTypeProxy, Guid> nodes, IRelationshipTypeProxy relationshipType, Point location, string originalId)
        {
            NodeService.ConnectNodesAsync(DomainId, nodes, relationshipType, location, originalId);
        }

        public virtual void UpdateNodeMetadataAsync(INodeProxy node, Guid relationshipId, IDescriptorTypeProxy descriptorType, string metadataName, string metadataValue, IMetadataTypeProxy metadataType)
        {
            NodeService.UpdateNodeMetadataAsync(DomainId, node.Id, relationshipId, descriptorType, metadataName, metadataValue, metadataType);
        }

        public virtual void GoBack()
        {
            if (_navHistory.HasHistory)
            {
                Guid previousNodeId = _navHistory.GoBack();
                SetCurrentNode(previousNodeId);
                GetCurrentNodesAsync();
            }
        }

        public virtual void GoBackTo(Guid nodeId)
        {
            if (_navHistory.HasHistory)
            {
                _navHistory.GoBackTo(nodeId);
                SetCurrentNode(nodeId);
                GetCurrentNodesAsync();
            }
        }
        #endregion

    }
}
