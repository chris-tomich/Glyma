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

namespace TransactionalNodeService.Proxy
{
    /// <summary>
    /// TODO: This need to be replaced with a proper and more efficient fix. There are other places this needs to be fix, search for 6218535.
    /// </summary>
    public class EqualityComparer1 : IEqualityComparer<KeyValuePair<ConnectionType, IRelationship>>
    {
        public bool Equals(KeyValuePair<ConnectionType, IRelationship> x, KeyValuePair<ConnectionType, IRelationship> y)
        {
            return (x.Key.Id == y.Key.Id) && (x.Value.Id == y.Value.Id);
        }

        public int GetHashCode(KeyValuePair<ConnectionType, IRelationship> obj)
        {
            unchecked
            {
                int hash = 19;
                int salt = 3;

                hash = obj.Key.Id.GetHashCode() * salt;
                hash = (obj.Value.Id.GetHashCode() * salt) + hash;

                return hash;
            }
        }
    }
}
