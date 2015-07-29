using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Data;
using System.Data.SqlClient;

namespace TransactionalNodeService.Common.Model
{
    [DataContract(Namespace = "http://sevensigma.com.au/TransactionalNodeService/OpObjects", Name = "ME")]
    [KnownType(typeof(DescriptorType))]
    [KnownType(typeof(MetadataType))]
    [KnownType(typeof(NodeType))]
    [KnownType(typeof(RelationshipType))]
    public abstract class MapTypeElement : IMapTypeElement
    {
        [DataMember(Name = "I")]
        public abstract Guid Id
        {
            get;
            set;
        }

        [DataMember(Name = "N")]
        public abstract string Name
        {
            get;
            set;
        }

        [IgnoreDataMember]
        public bool IsNew
        {
            get
            {
                return false;
            }
        }

        [IgnoreDataMember]
        public bool IsDirty
        {
            get
            {
                return false;
            }
        }

        public abstract void LoadSessionObject(IDataRecord record);

        public void PersistSessionObject(IDbConnectionAbstraction connectionAbstraction)
        {
            throw new NotImplementedException();
        }
    }
}