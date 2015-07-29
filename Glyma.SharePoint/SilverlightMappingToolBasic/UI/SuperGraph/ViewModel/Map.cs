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
using SilverlightMappingToolBasic.UI.ViewModel;
using System.Collections.Generic;

namespace SilverlightMappingToolBasic.UI.SuperGraph.ViewModel
{
    public class Map : IMap
    {
        private IDictionary<Guid, INode> _nodes = null;
        private IDictionary<Guid, IRelationship> _relationships = null;

        public Map()
        {
        }

        public IDictionary<Guid, INode> Nodes
        {
            get
            {
                if (_nodes == null)
                {
                    _nodes = new Dictionary<Guid, INode>();
                }

                return _nodes;
            }
            set
            {
                _nodes = value;
            }
        }

        public IDictionary<Guid, IRelationship> Relationships
        {
            get
            {
                if (_relationships == null)
                {
                    _relationships = new Dictionary<Guid, IRelationship>();
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
