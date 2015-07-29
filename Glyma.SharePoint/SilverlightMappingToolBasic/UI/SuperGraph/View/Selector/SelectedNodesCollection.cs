using System.Collections.Generic;
using System.Linq;
using SilverlightMappingToolBasic.UI.SuperGraph.ViewModel;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.Selector
{
    public class SelectedNodesCollection
    {
        private List<Relationship> _relatedRelationships;
        private IEnumerable<Relationship> _relationships;
        private IEnumerable<Node> _nodes;

        public SelectedNodesCollection(IEnumerable<Node> selectedNodes, IEnumerable<Relationship> relationships)
        {
            Nodes = selectedNodes.ToList();
            Relationships = relationships.ToList();
            FindRelatedRelationships();
        }

        private IEnumerable<Relationship> Relationships
        {
            get
            {
                if (_relationships == null)
                    _relationships = new List<Relationship>();
                return _relationships;
            }
            set
            {
                _relationships = value;
            }
        }

        public List<Relationship> RelatedRelationships
        {
            get
            {
                if (_relatedRelationships == null)
                    _relatedRelationships = new List<Relationship>();
                return _relatedRelationships;
            }
            set
            {
                _relatedRelationships = value;
            }
        }


        public IEnumerable<Node> Nodes
        {
            get
            {
                if (_nodes == null)
                    _nodes = new List<Node>();
                return _nodes;
            }
            set
            {
                _nodes = value;
            }
        }

        private void FindRelatedRelationships()
        {
            //Copy relationships to a list for modification
            RelatedRelationships = Relationships.ToList();
            foreach (var relationship in Relationships)
            {
                //Check the nodes conntect by this relationship are all in selection or not
                //If any node is not exist in selection, then delete the relationship
                //This is aim to find out relationships between those nodes that have been selected
                var countRelationshipToNodes = Nodes.Count(q => q.Id == relationship.To);
                var countRelationshipFromNodes = Nodes.Count(q => q.Id == relationship.From);
                if (countRelationshipToNodes == 0 || countRelationshipFromNodes == 0)
                {
                    RelatedRelationships.Remove(relationship);
                }
            }
        }

        

        
    }
}
