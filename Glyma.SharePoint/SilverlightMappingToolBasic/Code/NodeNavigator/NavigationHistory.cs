using System;
using System.Collections;
using System.Collections.Specialized;
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

namespace SilverlightMappingToolBasic
{
    public class NavigationHistory
    {
        private static Stack<NodeHistoryElement> _history = new Stack<NodeHistoryElement>();

        public void AddToHistory(NodeHistoryElement nodeHistoryElem)
        {
            //put the last focal node into the history before setting the new one
            if (_history.Count == 0)
            {
                _history.Push(nodeHistoryElem);
            }
            else if (_history.Peek().Node.Id != nodeHistoryElem.Node.Id)
            {
                _history.Push(nodeHistoryElem);
            }
        }

        public Stack<NodeHistoryElement> Stack
        {
            get
            {
                return _history;
            }
        }

        public Guid GoBack()
        {
            Guid previousNodeId = Guid.Empty;
            if (HasHistory)
            {
                NodeHistoryElement nhe = _history.Pop();
                if (HasHistory)
                {
                    previousNodeId = _history.Peek().Node.Id;
                }
                else
                {
                    previousNodeId = nhe.Node.Id;
                }
            }
            return previousNodeId;
        }

        public void GoBackTo(Guid nodeId)
        {
            Guid tempId = _history.Peek().Node.Id;
            if (tempId == nodeId)
            {
                return;
            }
            else
            {
                _history.Pop(); //discard the top of the stack
                GoBackTo(nodeId);
            }
        }

        public bool HasHistory
        {
            get 
            {
                return _history.Count > 0;
            }
        }
    }

    public class NodeHistoryElement
    {
        public string Name
        {
            get;
            set;
        }

        public INodeProxy Node
        {
            get;
            set;
        }

        //public override bool Equals(object obj)
        //{
        //    NodeHistoryElement compareTo = obj as NodeHistoryElement;
        //    if (compareTo != null)
        //    {
        //        if (compareTo.Node.Id == this.Node.Id)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}
    }
}
