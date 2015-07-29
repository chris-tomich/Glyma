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
using System.Collections.Generic;
using IoC;
using SilverlightMappingToolBasic.Service;

namespace SilverlightMappingToolBasic.SingleDepth
{
    public class SingleDepthNodeFactory
    {
        private SingleDepthNodeProxy _focalNode;
        private CompendiumFirstDepthMap _control;
        private List<UIElement> _currentChildren;

        public SingleDepthNodeFactory()
        {
            _currentChildren = new List<UIElement>();

            NodeCount = 0;
        }

        public int NumberOfNodes
        {
            get;
            set;
        }

        public double CentreX
        {
            get
            {
                return (_control.Width / 2);
            }
        }

        public double CentreY
        {
            get
            {
                return (_control.Height / 2);
            }
        }

        public int NodeCount
        {
            get;
            private set;
        }

        public void RegisterSurface(CompendiumFirstDepthMap control)
        {
            _control = control;
        }

        private UIElement[] CreateNodeUIElement(SingleDepthNodeProxy nodeProxy)
        {
            nodeProxy.CentreX = CentreX;
            nodeProxy.CentreY = CentreY;
            nodeProxy.IsFocused = false;
            nodeProxy.NodeCount = NumberOfNodes;
            nodeProxy.NodeIndex = NodeCount;
            nodeProxy.Radius = 300;

            return nodeProxy.RenderUIElements();
        }

        private UIElement[] CreateFocalNodeUIElement(SingleDepthNodeProxy nodeProxy)
        {
            nodeProxy.CentreX = CentreX;
            nodeProxy.CentreY = CentreY;
            nodeProxy.IsFocused = true;
            nodeProxy.NodeCount = NumberOfNodes;
            nodeProxy.NodeIndex = NodeCount;
            nodeProxy.Radius = 300;

            return nodeProxy.RenderUIElements();
        }

        public void AddFocalNode(SingleDepthNodeProxy focalNode)
        {
            _focalNode = focalNode;
            UIElement[] focalUIElements = CreateFocalNodeUIElement(focalNode);

            foreach (UIElement focalUIElement in focalUIElements)
            {
                _currentChildren.Add(focalUIElement);
                _control.uxMapSurface.Children.Add(focalUIElement);
            }
        }

        public void NewPage()
        {
            foreach (UIElement child in _currentChildren)
            {
                _control.uxMapSurface.Children.Remove(child);
            }

            NodeCount = 0;
            _currentChildren.Clear();
        }

        public void AddNode(SingleDepthNodeProxy node)
        {
            if (!_focalNode.Equals(node))
            {
                UIElement[] nodeUIElements = CreateNodeUIElement(node);

                NodeCount++;

                foreach (UIElement nodeUIElement in nodeUIElements)
                {
                    _currentChildren.Add(nodeUIElement);
                    _control.uxMapSurface.Children.Add(nodeUIElement);
                }
            }
        }
    }
}
