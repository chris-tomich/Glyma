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

namespace SilverlightMappingToolBasic
{
    public class RelationshipRendererCollection : Dictionary<Guid, RelationshipRenderer>
    {
        public RelationshipRendererCollection()
            : base()
        {
        }

        public RelationshipRendererCollection(int capacity)
            : base(capacity)
        {
        }

        public RelationshipRendererCollection(IEqualityComparer<Guid> comparer)
            : base(comparer)
        {
        }

        public RelationshipRendererCollection(IDictionary<Guid, RelationshipRenderer> dictionary)
            : base(dictionary)
        {
        }

        public RelationshipRendererCollection(int capacity, IEqualityComparer<Guid> comparer)
            : base(capacity, comparer)
        {
        }

        public RelationshipRendererCollection(IDictionary<Guid, RelationshipRenderer> dictionary, IEqualityComparer<Guid> comparer)
            : base(dictionary, comparer)
        {
        }

        public void UpdateAllRelationships()
        {
            foreach (RelationshipRenderer relationshipRenderer in this.Values)
            {
                relationshipRenderer.UpdateRelationship();
            }
        }

        public void UpdateRelationship(NodeRenderer nodeRenderer)
        {
            foreach (RelationshipRenderer relationshipRenderer in this.Values)
            {
                foreach (IDescriptorProxy descriptor in relationshipRenderer.Relationship.Descriptors)
                {
                    if (descriptor.Node != null && descriptor.Node.Id == nodeRenderer.Node.Id)
                    {
                        relationshipRenderer.UpdateRelationship();
                    }
                }
            }
        }
    }
}
