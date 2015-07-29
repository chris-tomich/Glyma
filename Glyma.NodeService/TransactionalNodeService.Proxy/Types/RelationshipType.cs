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

namespace TransactionalNodeService.Proxy
{
    public class RelationshipType : IMapType<Service.RT>
    {
        public RelationshipType()
        {
        }

        public Guid Id
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            ConnectionType comparisonObj = obj as ConnectionType;

            if (comparisonObj != null)
            {
                if (Id == comparisonObj.Id && Name == comparisonObj.Name)
                {
                    return true;
                }
            }

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 13;
                int salt = 17;

                hash = Id.GetHashCode() * salt;
                hash = (Name.GetHashCode() * salt) + hash;

                return hash;
            }
        }

        public void LoadTypeInfo(Service.RT mapTypeInfo)
        {
            Id = mapTypeInfo.I;
            Name = mapTypeInfo.N;
        }
    }
}
