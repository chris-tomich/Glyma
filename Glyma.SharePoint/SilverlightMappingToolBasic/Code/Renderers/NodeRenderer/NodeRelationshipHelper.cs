using System;
using System.Collections;
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

using SilverlightMappingToolBasic.Controls;

namespace SilverlightMappingToolBasic
{
    public class NodeRelationshipHelper
    {
        private NodeControl _toNode = null;
        private NodeControl _fromNode = null;
        private ArrowControl _relationshipArrow = null;

        public event EventHandler NodesConnected;

        public NodeControl FromNode
        {
            get
            {
                return _fromNode;
            }
            set
            {
                _relationshipArrow = null;
                _fromNode = value;
            }
        }

        public NodeControl ToNode
        {
            get
            {
                return _toNode;
            }
            set
            {
                if (_fromNode != value && _fromNode != null)
                {
                    _relationshipArrow = null;
                    _toNode = value;

                    //Note: the NodeControl's DataContext will be a INodeProxy
                    if (NodesConnected != null)
                    {
                        NodesConnected.Invoke(this, new EventArgs());
                    }
                }
            }
        }

        public ArrowControl Relationship
        {
            get
            {
                return _relationshipArrow;
            }
            set
            {
                _toNode = null;
                _fromNode = null;
                _relationshipArrow = value;
            }
        }

        public bool IsEditting
        {
            get;
            set;
        }

        public RelationshipSide EdittingSide
        {
            get;
            set;
        }

        public Point GetCenterOfFromNode()
        {
            Point result = new Point(0,0);
            if (_fromNode != null)
            {
                double startPositionX = FromNode.NodeSkinWidth / 2;
                double startPositionY = FromNode.NodeSkinHeight / 2;

                startPositionX += (double)FromNode.Parent.GetValue(Canvas.LeftProperty);
                startPositionY += (double)FromNode.Parent.GetValue(Canvas.TopProperty);

                result = new Point(startPositionX, startPositionY);
            }
            return result;
        }

    }

    public enum RelationshipSide
    {
        From,
        To
    }
}