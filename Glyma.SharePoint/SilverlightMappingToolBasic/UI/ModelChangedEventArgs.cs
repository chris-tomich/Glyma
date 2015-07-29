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
using SilverlightMappingToolBasic.UI.ViewModel;

namespace SilverlightMappingToolBasic.UI
{
    public class ModelChangedEventArgs<NodeType, RelationshipType> : EventArgs where NodeType : INode where RelationshipType : IRelationship
    {
        private List<KeyValuePair<NodeType, ModelOperationType>> _nodes = null;
        private List<KeyValuePair<RelationshipType, ModelOperationType>> _relationships = null;

        public ModelChangedEventArgs()
        {
        }

        public List<KeyValuePair<NodeType, ModelOperationType>> Nodes
        {
            get
            {
                if (_nodes == null)
                {
                    _nodes = new List<KeyValuePair<NodeType, ModelOperationType>>();
                }

                return _nodes;
            }
            set
            {
                _nodes = value;
            }
        }

        public List<KeyValuePair<RelationshipType, ModelOperationType>> Relationships
        {
            get
            {
                if (_relationships == null)
                {
                    _relationships = new List<KeyValuePair<RelationshipType, ModelOperationType>>();
                }

                return _relationships;
            }
            set
            {
                _relationships = value;
            }
        }
    }
}
