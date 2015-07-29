using System;
using System.Net;
using System.Windows;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using SilverlightMappingToolBasic.Controls;
using SilverlightMappingToolBasic.MappingService;

namespace SilverlightMappingToolBasic.MapDepth
{
    public class MapDepthNodeRenderer : NodeRenderer, INodeRenderer
    {
        protected MapDepthNodeRenderer() : base()
        {
        }

        public MapDepthNodeRenderer(NavigatorView parentNavigatorView, INodeProxy nodeProxy, ThemeManager themeManager, string skinName)
            : base(parentNavigatorView, nodeProxy, themeManager, skinName)
        {
        }

        private bool GetNodeLocation(MetadataContext xPosKey, MetadataContext yPosKey, out double x, out double y)
        {
            x = 0.0;
            y = 0.0;

            if (Node.Metadata != null && Node.HasMetadata(xPosKey) && Node.HasMetadata(yPosKey))
            {
                double.TryParse(Node.GetNodeMetadata(xPosKey).MetadataValue, out x);
                double.TryParse(Node.GetNodeMetadata(yPosKey).MetadataValue, out y);

                return true;
            }

            return false;
        }

        private void RenderNodeAsTransclusion(MetadataContext xPosKey, MetadataContext yPosKey, out IDescriptorTypeProxy descriptorType, out double x, out double y)
        {
            x = 0.0;
            y = 0.0;

            descriptorType = IoC.IoCContainer.GetInjectionInstance().GetInstance<TypeManager>().GetDescriptorType("TransclusionMap");
            xPosKey.DescriptorTypeUid = descriptorType.Id;
            yPosKey.DescriptorTypeUid = descriptorType.Id;

            foreach (IRelationshipProxy relationship in GetTransclusionRelationship(Node))
            {
                xPosKey.RelationshipUid = relationship.Id;
                yPosKey.RelationshipUid = relationship.Id;

                if (GetNodeLocation(xPosKey, yPosKey, out x, out y))
                {
                    break;
                }
            }
        }

        private void RenderNodeAsChild(MetadataContext xPosKey, MetadataContext yPosKey, out IDescriptorTypeProxy descriptorType, out double x, out double y)
        {
            x = 0.0;
            y = 0.0;

            IRelationshipProxy mapRelationship = GetMapRelationship(Node);
            descriptorType = IoC.IoCContainer.GetInjectionInstance().GetInstance<TypeManager>().GetDescriptorType("From");
            xPosKey.DescriptorTypeUid = descriptorType.Id;
            yPosKey.DescriptorTypeUid = descriptorType.Id;

            if (mapRelationship != null)
            {
                xPosKey.RelationshipUid = mapRelationship.Id;
                yPosKey.RelationshipUid = mapRelationship.Id;
            }

            GetNodeLocation(xPosKey, yPosKey, out x, out y);
        }

        public override NodeControl RenderChildren(RenderingContextInfo info)
        {
            if (_nodeControl == null)
            {
                double x = 0.0;
                double y = 0.0;

                MetadataContext xPosKey = new MetadataContext()
                {
                    MetadataName = "XPosition",
                    NodeUid = Node.Id
                };
                MetadataContext yPosKey = new MetadataContext()
                {
                    MetadataName = "YPosition",
                    NodeUid = Node.Id
                };

                IDescriptorTypeProxy descriptorType;

                //The is being drawn as a transclusion representation (i.e. not in it's original parent map)
                if (Node.ParentMapNodeUid != Guid.Empty && Node.ParentMapNodeUid != ParentNavigatorView.ContextNode.Id)
                {
                    RenderNodeAsTransclusion(xPosKey, yPosKey, out descriptorType, out x, out y);
                }

                //The node is being drawn in it's parent map (i.e. not a transclusion representation)
                else
                {
                    RenderNodeAsChild(xPosKey, yPosKey, out descriptorType, out x, out y);
                }

                Location = new Point(x, y);

                _nodeControl = Skin.RenderSkinElements(Node, SkinName, Skin.SkinProperties);
            }

            return _nodeControl;
        }

        private void ToggleChildNodes(bool isSelected, INodeProxy nodeProxy)
        {
            foreach (Guid childNodeId in GetAllChildNodes(nodeProxy))
            {
                if (ParentNavigatorView.NodeRenderers.ContainsKey(childNodeId))
                {
                    NodeRenderer nodeRenderer = ParentNavigatorView.NodeRenderers[childNodeId];
                    if (nodeProxy.IsTransclusion && !IsChildInCurrentView(nodeProxy, nodeRenderer.Node.Id))
                    {
                        continue;
                    }

                    nodeRenderer.IsSelected = isSelected;

                    INodeProxy childNode = nodeRenderer.Node;
                    foreach (IDescriptorProxy descriptor in childNode.Descriptors.GetByDescriptorTypeName("From"))
                    {
                        if (descriptor.Relationship.RelationshipType.Name != "MapContainerRelationship"
                            && IsChildRelationship(descriptor.Relationship, nodeProxy.Id)
                            && ParentNavigatorView.RelationshipRenderers.ContainsKey(descriptor.Relationship.Id))
                        {
                            IRelationshipRenderer relationshipRenderer = ParentNavigatorView.RelationshipRenderers[descriptor.Relationship.Id];

                            relationshipRenderer.IsSelected = isSelected;
                        }
                    }

                    if (isSelected)
                    {
                        SelectChildNodes(childNode);
                    }
                    else
                    {
                        UnselectChildNodes(childNode);
                    }
                }
            }
        }

        public override void SelectChildNodes(INodeProxy nodeProxy)
        {
            ToggleChildNodes(true, nodeProxy);
        }

        public override void UnselectChildNodes(INodeProxy nodeProxy)
        {
            ToggleChildNodes(false, nodeProxy);
        }

        private bool IsChildInCurrentView(INodeProxy nodeProxy, Guid childNodeId)
        {
            foreach (IDescriptorProxy descriptor in nodeProxy.Descriptors.GetByDescriptorTypeName("To"))
            {
                if (descriptor.Relationship.RelationshipType.Name == "TransclusionRelationship")
                {
                    foreach (IDescriptorProxy fromNodeDesc in descriptor.Relationship.Descriptors.GetByDescriptorTypeName("From"))
                    {
                        if (fromNodeDesc.NodeId == childNodeId)
                        {
                            foreach (IDescriptorProxy transMap in descriptor.Relationship.Descriptors.GetByDescriptorTypeName("TransclusionMap"))
                            {
                                if (transMap.NodeId == this.ParentNavigatorView.ContextNode.Id)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
                else if (descriptor.Relationship.RelationshipType.Name == "FromToRelationship" 
                           || descriptor.Relationship.RelationshipType.Name == "MapRelationship")
                {
                    foreach (IDescriptorProxy fromNodeDesc in descriptor.Relationship.Descriptors.GetByDescriptorTypeName("From"))
                    {
                        if (fromNodeDesc.NodeId == childNodeId)
                        {
                            if (fromNodeDesc.Node.ParentMapNodeUid == this.ParentNavigatorView.ContextNode.Id)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        private void ToggleParentNodes(bool isSelected, INodeProxy nodeProxy)
        {
            foreach (Guid parentNodeId in nodeProxy.ParentNodes)
            {
                if (ParentNavigatorView.ContextNode.Id == parentNodeId)
                {
                    continue; //a map node shouldn't really ever get in this situation where it is transcluded within itself
                }
                if (ParentNavigatorView.NodeRenderers.ContainsKey(parentNodeId))
                {
                    NodeRenderer nodeRenderer = ParentNavigatorView.NodeRenderers[parentNodeId];
                    if (!IsParentInCurrentView(nodeProxy, nodeRenderer.Node))
                    {
                        continue;
                    }

                    nodeRenderer.IsSelected = isSelected;

                    INodeProxy parentNode = nodeRenderer.Node;
                    foreach (IDescriptorProxy descriptor in parentNode.Descriptors.GetByDescriptorTypeName("To"))
                    {
                        if (descriptor.Relationship.RelationshipType.Name != "MapContainerRelationship"
                            && IsParentRelationship(descriptor.Relationship, nodeProxy.Id)
                            && ParentNavigatorView.RelationshipRenderers.ContainsKey(descriptor.Relationship.Id))
                        {
                            IRelationshipRenderer relationshipRenderer = ParentNavigatorView.RelationshipRenderers[descriptor.Relationship.Id];

                            relationshipRenderer.IsSelected = isSelected;
                        }
                    }

                    if (isSelected)
                    {
                        SelectParentNodes(parentNode);
                    }
                    else
                    {
                        UnselectParentNodes(parentNode);
                    }
                }
            }
        }

        public override void SelectParentNodes(INodeProxy nodeProxy)
        {
            ToggleParentNodes(true, nodeProxy);
        }

        public override void UnselectParentNodes(INodeProxy nodeProxy)
        {
            ToggleParentNodes(false, nodeProxy);
        }

        private bool IsParentInCurrentView(INodeProxy nodeProxy, INodeProxy relatedNode)
        {
            foreach (IDescriptorProxy descriptor in nodeProxy.Descriptors.GetByDescriptorTypeName("From"))
            {
                if (descriptor.Relationship.RelationshipType.Name == "TransclusionRelationship")
                {
                    foreach (IDescriptorProxy toNodeDesc in descriptor.Relationship.Descriptors.GetByDescriptorTypeName("To"))
                    {
                        if (toNodeDesc.NodeId == relatedNode.Id)
                        {
                            foreach (IDescriptorProxy transMap in descriptor.Relationship.Descriptors.GetByDescriptorTypeName("TransclusionMap"))
                            {
                                if (transMap.NodeId == this.ParentNavigatorView.ContextNode.Id)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
                else if (descriptor.Relationship.RelationshipType.Name == "FromToRelationship" ||
                         descriptor.Relationship.RelationshipType.Name == "MapRelationship")
                {
                    foreach (IDescriptorProxy fromDesc in descriptor.Relationship.Descriptors.GetByDescriptorTypeName("From"))
                    {
                        if (fromDesc.NodeId == nodeProxy.Id)
                        {
                            foreach (IDescriptorProxy toDesc in descriptor.Relationship.Descriptors.GetByDescriptorTypeName("To"))
                            {
                                if (toDesc.NodeId == relatedNode.Id)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        private IEnumerable<Guid> GetAllChildNodes(INodeProxy nodeProxy)
        {
            List<Guid> nodes = new List<Guid>();
            if (nodeProxy.Descriptors != null)
            {
                foreach (DescriptorProxy descriptorProxy in nodeProxy.Descriptors.GetByDescriptorTypeName("To"))
                {
                    foreach (DescriptorProxy dp in descriptorProxy.Relationship.Descriptors.GetByDescriptorTypeName("From"))
                    {
                        if (descriptorProxy.Relationship.RelationshipType.Name != "TransclusionRelationship")
                        {
                            if (!nodes.Contains(dp.NodeId))
                            {
                                nodes.Add(dp.NodeId);
                            }
                        }
                        else
                        {
                            if (IsChildInCurrentView(nodeProxy, dp.NodeId))
                            {
                                if (!nodes.Contains(dp.NodeId))
                                {
                                    nodes.Add(dp.NodeId);
                                }
                            }
                        }
                    }
                }
            }
            return nodes.ToArray();
        }

        private bool IsRelated(IRelationshipProxy relationship, Guid nodeId, string descriptorType)
        {
            bool result = false;
            foreach (IDescriptorProxy descriptor in relationship.Descriptors.GetByDescriptorTypeName(descriptorType))
            {
                if (descriptor.NodeId == nodeId)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        private bool IsParentRelationship(IRelationshipProxy relationship, Guid childNodeId)
        {
            return IsRelated(relationship, childNodeId, "From");
        }

        private bool IsChildRelationship(IRelationshipProxy relationship, Guid parentNodeId)
        {
            return IsRelated(relationship, parentNodeId, "To");
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
                        if (alternateDescriptor.NodeId == ParentNavigatorView.ContextNode.Id)
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
                if (descriptor.Relationship.RelationshipType.Name == "TransclusionRelationship")
                {
                    foreach (IDescriptorProxy alternateDescriptor in descriptor.Relationship.Descriptors.GetByDescriptorTypeName("TransclusionMap"))
                    {
                        //return a transclusion relationship that is in this map being rendered.
                        if (alternateDescriptor.NodeId == ParentNavigatorView.ContextNode.Id)
                        {
                            yield return descriptor.Relationship;
                        }
                    }
                }
            }
        }
    }
    
}
