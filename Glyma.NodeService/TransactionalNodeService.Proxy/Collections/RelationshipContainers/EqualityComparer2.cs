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
    ///  TODO: This need to be replaced with a proper and more efficient fix. There are other places this needs to be fix, search for 6218535.
    /// </summary>
    public class EqualityComparer2 : IEqualityComparer<IRelationship>
    {
        public bool Equals(IRelationship x, IRelationship y)
        {
            return (x.Id == y.Id);
        }

        public int GetHashCode(IRelationship obj)
        {
            unchecked
            {
                return obj.Id.GetHashCode();
            }
        }
    }
}
