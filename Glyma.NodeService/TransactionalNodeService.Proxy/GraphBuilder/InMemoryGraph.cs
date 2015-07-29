﻿using System;
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
using TransactionalNodeService.Soap;
using TransactionalNodeService.Proxy;
using ServerObjects = TransactionalNodeService.Proxy.ServerObjects;

namespace TransactionalNodeService.GraphBuilder
{
    public class InMemoryGraph
    {
        private SoapNodeManager _nodeManager = null;
        private SoapRelationshipManager _relationshipManager = null;

        public InMemoryGraph(SoapMapManager mapManager)
        {
            MapManager = mapManager;
        }

        private SoapMapManager MapManager
        {
            get;
            set;
        }

        private SoapNodeManager NodeManager
        {
            get
            {
                if (_nodeManager == null)
                {
                    _nodeManager = new SoapNodeManager(MapManager);
                }

                return _nodeManager;
            }
        }

        private SoapRelationshipManager RelationshipManager
        {
            get
            {
                if (_relationshipManager == null)
                {
                    _relationshipManager = new SoapRelationshipManager(MapManager);
                }

                return _relationshipManager;
            }
        }

        public Proxy.INodeFactory NodeFactory
        {
            get
            {
                return NodeManager;
            }
        }

        public Proxy.IRelationshipFactory RelationshipFactory
        {
            get
            {
                return RelationshipManager;
            }
        }

        public void NodesAndRelationshipBuildEventArgs(Proxy.NodesEventArgs eventArgs, IDictionary<Guid, ServerObjects.Node> nodes, IDictionary<Guid, ServerObjects.Relationship> relationships)
        {
            foreach (ServerObjects.Node serviceNode in nodes.Values)
            {
                Proxy.INode proxyNode = NodeManager.FindNode(serviceNode);

                eventArgs.Nodes.Add(proxyNode.Id, proxyNode);
            }

            foreach (ServerObjects.Relationship serviceRelationship in relationships.Values)
            {
                RelationshipManager.CreateRelationship(serviceRelationship);
            }

            foreach (ServerObjects.Node serviceNode in nodes.Values)
            {
                Proxy.INode proxyNode = NodeManager.FindNode(serviceNode);

                SoapNode soapNode = proxyNode as SoapNode;

                /// Not all the nodes that are stored in the NodeManager are SoapNodes, some are FacadeNodes. In this scenario we want to check if they have an inner SoapNode and use that instead.
                if (soapNode == null)
                {
                    if (proxyNode is FacadeNode)
                    {
                        FacadeNode facadeNode = proxyNode as FacadeNode;
                        soapNode = facadeNode.BaseNode as SoapNode;
                    }
                }

                if (soapNode != null)
                {
                    soapNode.LoadNode(RelationshipManager);
                }
            }

            foreach (ServerObjects.Relationship serviceRelationship in relationships.Values)
            {
                Proxy.IRelationship proxyRelationship = RelationshipManager.FindRelationship(serviceRelationship);

                SoapRelationship soapRelationship = proxyRelationship as SoapRelationship;

                /// Not all the relationships that are stored in the RelationshipManager are SoapRelationships, some are FacadeRelationships. In this scenario we want to check if they have an inner SoapRelationship and use that instead.
                if (soapRelationship == null)
                {
                    if (proxyRelationship is FacadeRelationship)
                    {
                        FacadeRelationship facadeRelationship = proxyRelationship as FacadeRelationship;
                        soapRelationship = facadeRelationship.BaseRelationship as SoapRelationship;
                    }
                }

                if (soapRelationship != null)
                {
                    soapRelationship.LoadRelationship(NodeManager);
                }
            }
        }

        public Proxy.INode FindNode(ServerObjects.Node serviceNode)
        {
            return NodeManager.FindNode(serviceNode);
        }
    }
}
