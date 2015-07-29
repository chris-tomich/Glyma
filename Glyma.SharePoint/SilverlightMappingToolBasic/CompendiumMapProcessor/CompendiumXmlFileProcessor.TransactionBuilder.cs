using System;
using System.ComponentModel;
using System.Net;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Linq;
using SilverlightMappingToolBasic.CompendiumMapProcessor.XmlModel.Relationships;
using TransactionalNodeService.Service;
using Proxy = TransactionalNodeService.Proxy;
using TransactionFramework = TransactionalNodeService.Soap.TransactionFramework;

namespace SilverlightMappingToolBasic.CompendiumMapProcessor
{
    public partial class CompendiumXmlFileProcessor
    {
        private class TransactionBuilder
        {
            private Dictionary<string, Proxy.INode> _proxyNodes;
            private int _executedTransaction;
            private int _totalTransactions;

            public event EventHandler<ProgressChangedEventArgs> ProgressChanged;
            public event EventHandler<FileProcessorCompletedEventArgs> ProgressCompleted;

            private IEnumerable<XmlModel.INode> Nodes
            {
                get;
                set;
            }

            private IEnumerable<CompendiumViewRelationship> RootViews
            {
                get;
                set;
            }

            private Dictionary<string, Proxy.INode> ProxyNodes
            {
                get
                {
                    if (_proxyNodes == null)
                    {
                        _proxyNodes = new Dictionary<string, Proxy.INode>();
                    }

                    return _proxyNodes;
                }
            }

            public Proxy.INode Map
            {
                get;
                set;
            }

            public Proxy.IMapManager MapManager
            {
                get;
                set;
            }

            public TransactionBuilder(IEnumerable<XmlModel.INode> nodes, IEnumerable<CompendiumViewRelationship> rootViews)
            {
                Nodes = nodes;
                RootViews = rootViews;
            }

            private void BuildNodeTransactions(ref Queue<TransactionFramework.TransactionChain> transactionQueue)
            {
                var transactionCounts = 0;
                var chain = new TransactionFramework.TransactionChain();

                foreach (XmlModel.INode node in Nodes)
                {
                    if (node.NodeType == MapManager.NodeTypes["DomainNode"])
                    {
                        continue;
                    }

                    if (transactionCounts >= 150)
                    {
                        transactionQueue.Enqueue(chain);
                        chain = new TransactionFramework.TransactionChain();
                        transactionCounts = 0;
                    }
                    else
                    {
                        transactionCounts++;
                    }

                    Proxy.INode newNode = MapManager.CreateNode(Map.DomainId, Map.RootMapId.Value, node.NodeType, string.Empty, ref chain);

                    ProxyNodes[node.Id] = newNode;

                    newNode.Metadata.Add(null, null, "Name", node.Name, ref chain);

                    if (node.NodeType == MapManager.NodeTypes["CompendiumReferenceNode"])
                    {
                        var link = string.Format("{0}/{1}/{2}", App.Params.SiteUrl, App.Params.FileUploadLibrary, node.Name);
                        newNode.Metadata.Add(null, null, "Link", link, ref chain);
                    }

                    //MapManager.ExecuteTransaction(chain);
                }

                transactionQueue.Enqueue(chain);
            }

            private void BuildRootViewRelationships(ref Queue<TransactionFramework.TransactionChain> transactionQueue)
            {
                TransactionFramework.TransactionChain chain = new TransactionFramework.TransactionChain();

                foreach (CompendiumViewRelationship viewRelationship in RootViews)
                {
                    Proxy.INode proxyNode = ProxyNodes[viewRelationship.FromNode.Id];

                    Proxy.IRelationship newRelationship = MapManager.CreateRelationship(Map.DomainId, Map.RootMapId.Value, MapManager.RelationshipTypes["MapContainerRelationship"], string.Empty, ref chain);
                    newRelationship.ConnectNode(MapManager.ConnectionTypes["To"], Map, ref chain);
                    newRelationship.ConnectNode(MapManager.ConnectionTypes["From"], proxyNode, ref chain);

                    proxyNode.Metadata.Add(newRelationship, MapManager.ConnectionTypes["From"], "XPosition", viewRelationship.XPosition.ToString(), ref chain);
                    proxyNode.Metadata.Add(newRelationship, MapManager.ConnectionTypes["From"], "YPosition", viewRelationship.YPosition.ToString(), ref chain);

                    //MapManager.ExecuteTransaction(chain);
                }

                transactionQueue.Enqueue(chain);
            }

            private void BuildRelationships(ref Queue<TransactionFramework.TransactionChain> transactionQueue)
            {
                int transactionCounts = 0;
                TransactionFramework.TransactionChain chain = new TransactionFramework.TransactionChain();

                foreach (XmlModel.INode node in Nodes)
                {
                    foreach (XmlModel.IDescriptor descriptor in node.Descriptors)
                    {
                        if (descriptor.DescriptorType == null || descriptor.DescriptorType.Name == "From")
                        {
                            continue;
                        }

                        XmlModel.IRelationship relationship = descriptor.Relationship;
                        XmlModel.INode connectedNode = null;

                        foreach (XmlModel.IDescriptor linkedDescriptor in relationship.Descriptors)
                        {
                            if (linkedDescriptor != descriptor)
                            {
                                connectedNode = linkedDescriptor.Node;
                                break;
                            }
                        }

                        if (connectedNode == null)
                        {
                            // This means that there was no connected nodes or there may have been a circular reference.
                            continue;
                        }

                        Proxy.INode proxyNode;

                        if (ProxyNodes.ContainsKey(node.Id))
                        {
                            proxyNode = ProxyNodes[node.Id];
                        }
                        else
                        {
                            continue;
                        }

                        Proxy.INode proxyConnectedNode;

                        if (ProxyNodes.ContainsKey(connectedNode.Id))
                        {
                            proxyConnectedNode = ProxyNodes[connectedNode.Id];
                        }
                        else
                        {
                            continue;
                        }

                        if (proxyNode.NodeType == MapManager.NodeTypes["DomainNode"] || proxyConnectedNode.NodeType == MapManager.NodeTypes["DomainNode"])
                        {
                            continue;
                        }

                        if (transactionCounts >= 150)
                        {
                            transactionQueue.Enqueue(chain);
                            chain = new TransactionFramework.TransactionChain();
                            transactionCounts = 0;
                        }
                        else
                        {
                            transactionCounts++;
                        }

                        if (relationship is CompendiumLinkRelationship)
                        {
                            CompendiumLinkRelationship linkRelationship = relationship as CompendiumLinkRelationship;

                            if (!linkRelationship.IsTransclusion)
                            {
                                // In this situation the relationship is between two nodes in a map.
                                Proxy.IRelationship newRelationship = MapManager.CreateRelationship(Map.DomainId, Map.RootMapId.Value, MapManager.RelationshipTypes["FromToRelationship"], string.Empty, ref chain);
                                newRelationship.ConnectNode(MapManager.ConnectionTypes["To"], proxyNode, ref chain);
                                newRelationship.ConnectNode(MapManager.ConnectionTypes["From"], proxyConnectedNode, ref chain);
                            }
                            else
                            {
                                Proxy.INode proxyMap = ProxyNodes[linkRelationship.Map.Id];

                                // In this situation the relationship is a transclusion so we'll need to handle it slightly differently.
                                //Proxy.IRelationship newTranscludedNodeRelationship = MapManager.CreateRelationship(Map.DomainId, MapManager.RelationshipTypes["MapContainerRelationship"], string.Empty, ref chain);
                                //newTranscludedNodeRelationship.ConnectNode(MapManager.ConnectionTypes["To"], proxyMap, ref chain);
                                //newTranscludedNodeRelationship.ConnectNode(MapManager.ConnectionTypes["From"], transclusionNode, ref chain);

                                //transclusionNode.Metadata.Add(newTranscludedNodeRelationship, MapManager.ConnectionTypes["From"], "XPosition", transclusionRelationship.XPosition.ToString(), ref chain);
                                //transclusionNode.Metadata.Add(newTranscludedNodeRelationship, MapManager.ConnectionTypes["From"], "YPosition", transclusionRelationship.YPosition.ToString(), ref chain);

                                Proxy.IRelationship newTransclusionRelationship = MapManager.CreateRelationship(Map.DomainId, Map.RootMapId.Value, MapManager.RelationshipTypes["TransclusionFromToRelationship"], string.Empty, ref chain);

                                newTransclusionRelationship.ConnectNode(MapManager.ConnectionTypes["TransclusionMap"], proxyMap, ref chain);
                                newTransclusionRelationship.ConnectNode(MapManager.ConnectionTypes["To"], proxyNode, ref chain);
                                newTransclusionRelationship.ConnectNode(MapManager.ConnectionTypes["From"], proxyConnectedNode, ref chain);
                            }
                        }
                        else if (relationship is CompendiumViewRelationship)
                        {
                            CompendiumViewRelationship viewRelationship = relationship as CompendiumViewRelationship;

                            if (!viewRelationship.IsRootView)
                            {
                                // In this situation the relationship is between a node and a map node.
                                Proxy.IRelationship newRelationship = MapManager.CreateRelationship(Map.DomainId, Map.RootMapId.Value, MapManager.RelationshipTypes["MapContainerRelationship"], string.Empty, ref chain);
                                newRelationship.ConnectNode(MapManager.ConnectionTypes["To"], proxyNode, ref chain);
                                newRelationship.ConnectNode(MapManager.ConnectionTypes["From"], proxyConnectedNode, ref chain);

                                proxyConnectedNode.Metadata.Add(newRelationship, MapManager.ConnectionTypes["From"], "XPosition", viewRelationship.XPosition.ToString(), ref chain);
                                proxyConnectedNode.Metadata.Add(newRelationship, MapManager.ConnectionTypes["From"], "YPosition", viewRelationship.YPosition.ToString(), ref chain);
                            }
                        }
                        else
                        {
                            // In this situation the relationship is of an unknown type so lets just skip it.
                            continue;
                        }
                    }
                    //chains.Enqueue(chain);

                    //MapManager.ExecuteTransaction(chain);
                    ///
                }
                if (chain != null && chain.NumOfTransactions > 0)
                {
                    transactionQueue.Enqueue(chain);
                }
            }

            private Queue<TransactionFramework.TransactionChain> TransactionQueue
            {
                get;
                set;
            }

            private void TransactionChainOnTransactionCompleted(object sender, CompleteTransactionCompletedEventArgs completeTransactionCompletedEventArgs)
            {
                _executedTransaction++;
                if (_executedTransaction < _totalTransactions && _totalTransactions != 0)
                {
                    if (ProgressChanged != null)
                    {
                        ProgressChanged(this, new ProgressChangedEventArgs(_executedTransaction * 100 / _totalTransactions, null));
                    }
                }
                else
                {
                    if (ProgressCompleted != null)
                    {
                        ProgressCompleted(this, new FileProcessorCompletedEventArgs{ NeedRefresh = true });
                    }
                }
            }

            public void InitialiseTransactions()
            {
                Queue<TransactionFramework.TransactionChain> transactionQueue = new Queue<TransactionFramework.TransactionChain>();

                BuildNodeTransactions(ref transactionQueue);
                BuildRootViewRelationships(ref transactionQueue);
                BuildRelationships(ref transactionQueue);

                TransactionQueue = transactionQueue;
                _totalTransactions = TransactionQueue.Count;
                _executedTransaction = 0;
            }

            public void ExecuteTransactions()
            {
                ExecuteTransactions(TransactionQueue.Count);
            }

            /// <summary>
            /// Executes the number of transactions specified.
            /// False if all transactions have been executed. True if there are still transactions to complete.
            /// </summary>
            /// <param name="numberOfTransactions"></param>
            /// <returns>False if all transactions have been executed. True if there are still transactions to complete.</returns>
            public bool ExecuteTransactions(int numberOfTransactions)
            {
                for (int i = 0; i < numberOfTransactions; i++)
                {
                    if (TransactionQueue.Count > 0)
                    {
                        TransactionFramework.TransactionChain transactionChain = TransactionQueue.Dequeue();
                        transactionChain.TransactionCompleted += TransactionChainOnTransactionCompleted;
                        MapManager.ExecuteTransaction(transactionChain);
                    }
                    else
                    {
                        
                        return false;
                    }
                }

                return (TransactionQueue.Count > 0);
            }

            
        }
    }
}
