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

namespace SilverlightMappingToolBasic.UI.SuperGraph.ViewModel
{
    public class RelationshipEqualityComparer : IEqualityComparer<KeyValuePair<Relationship, ModelOperationType>>
    {
        public RelationshipEqualityComparer()
        {
        }

        public bool Equals(KeyValuePair<Relationship, ModelOperationType> x, KeyValuePair<Relationship, ModelOperationType> y)
        {
            if (x.Value == y.Value)
            {
                return (x.Key.Id == y.Key.Id);
            }

            return false;
        }

        public int GetHashCode(KeyValuePair<Relationship, ModelOperationType> obj)
        {
            return (obj.Key.Id.GetHashCode() ^ obj.Value.GetHashCode());
        }
    }
}
