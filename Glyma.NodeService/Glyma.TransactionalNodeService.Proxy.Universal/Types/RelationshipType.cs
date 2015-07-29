using System;
using TransactionalNodeService.Proxy.Universal.Types.Generic;

namespace TransactionalNodeService.Proxy.Universal.Types
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
            RelationshipType comparisonObj = obj as RelationshipType;

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
