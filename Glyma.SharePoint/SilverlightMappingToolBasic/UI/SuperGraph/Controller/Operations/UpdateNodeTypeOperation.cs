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
using TransactionalNodeService.Service;
using Proxy = TransactionalNodeService.Proxy;
using TransactionFramework = TransactionalNodeService.Soap.TransactionFramework;
using System.Collections.Generic;

namespace SilverlightMappingToolBasic.UI.SuperGraph.Controller.Operations
{
    public class UpdateNodeTypeOperation : CommonOperationBase, ISuperGraphOperation
    {
        public UpdateNodeTypeOperation(Proxy.IMapManager mapManager)
            : base(mapManager)
        {
        }

        public ThemeManager ThemeManager
        {
            get;
            set;
        }

        public Proxy.INode Context
        {
            get;
            set;
        }

        public ViewModel.Node Node
        {
            get;
            set;
        }

        public ViewModel.ChangeNodeTypeEnum ChangedTo
        {
            get;
            set;
        }

        public event EventHandler<NodeOperationCompletedArgs> OperationCompleted;

        public void ExecuteOperation()
        {
            Proxy.NodeType nodeTypeToChangeTo;

            switch (ChangedTo)
            {
                case ViewModel.ChangeNodeTypeEnum.Question:
                    nodeTypeToChangeTo = MapManager.NodeTypes["CompendiumQuestionNode"];
                    break;

                case ViewModel.ChangeNodeTypeEnum.Idea:
                    nodeTypeToChangeTo = MapManager.NodeTypes["CompendiumIdeaNode"];
                    break;

                case ViewModel.ChangeNodeTypeEnum.Pro:
                    nodeTypeToChangeTo = MapManager.NodeTypes["CompendiumProNode"];
                    break;

                case ViewModel.ChangeNodeTypeEnum.Con:
                    nodeTypeToChangeTo = MapManager.NodeTypes["CompendiumConNode"];
                    break;

                case ViewModel.ChangeNodeTypeEnum.Map:
                    nodeTypeToChangeTo = MapManager.NodeTypes["CompendiumMapNode"];
                    break;
                case ViewModel.ChangeNodeTypeEnum.Note:
                    nodeTypeToChangeTo = MapManager.NodeTypes["CompendiumNoteNode"];
                    break;
                case ViewModel.ChangeNodeTypeEnum.Decision:
                    nodeTypeToChangeTo = MapManager.NodeTypes["CompendiumDecisionNode"];
                    break;

                default:
                    return;
            }

            TransactionFramework.TransactionChain chain = new TransactionFramework.TransactionChain();
            chain.TransactionCompleted += ChainOnTransactionCompleted;
            if (Node.Proxy.NodeType == MapManager.NodeTypes["CompendiumMapNode"])
            {
                IEnumerable<Proxy.IRelationship> relationships = Node.Proxy.Relationships.FindRelationships(MapManager.ConnectionTypes["To"], MapManager.RelationshipTypes["MapContainerRelationship"]);

                foreach (Proxy.IRelationship relationship in relationships)
                {
                    relationship.ConnectNode(MapManager.ConnectionTypes["To"], Context, ref chain);

                    IEnumerable<Proxy.INode> childNodes = relationship.Nodes.FindNodes(MapManager.ConnectionTypes["From"]);

                    foreach (Proxy.INode childNode in childNodes)
                    {
                        IEnumerable<KeyValuePair<Proxy.ConnectionType, Proxy.IRelationship>> transclusionRelationshipConnections = childNode.Relationships.FindRelationships(MapManager.RelationshipTypes["TransclusionFromToRelationship"]);

                        foreach (KeyValuePair<Proxy.ConnectionType, Proxy.IRelationship> transclusionRelationshipConnection in transclusionRelationshipConnections)
                        {
                            Proxy.IRelationship transclusionRelationship = transclusionRelationshipConnection.Value;

                            IEnumerable<Proxy.INode> transclusionMapNodes = transclusionRelationship.Nodes.FindNodes(MapManager.ConnectionTypes["TransclusionMap"]);

                            foreach (Proxy.INode transclusionMapNode in transclusionMapNodes)
                            {
                                if (transclusionMapNode.Id == Node.Proxy.Id)
                                {
                                    transclusionRelationship.ConnectNode(MapManager.ConnectionTypes["TransclusionMap"], Context, ref chain);

                                    break;
                                }
                            }
                        }
                    }
                }
            }

            Node.Proxy.Update(nodeTypeToChangeTo, ref chain);

            MapManager.ExecuteTransaction(chain);
        }

        private void ChainOnTransactionCompleted(object sender, CompleteTransactionCompletedEventArgs e)
        {
            if (OperationCompleted != null)
            {
                OperationCompleted(sender, new NodeOperationCompletedArgs { ViewModeNode = Node });
            }
        }
    }
}
