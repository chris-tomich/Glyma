using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.Data;

namespace TransactionalNodeService.Common.Model
{
    [DataContract(Namespace = "http://sevensigma.com.au/TransactionalNodeService/OpObjects", Name = "RT")]
    public class RelationshipType : MapTypeElement
    {
        public RelationshipType()
        {
        }

        [DataMember(Name = "I")]
        public override Guid Id
        {
            get;
            set;
        }

        [DataMember(Name = "N")]
        public override string Name
        {
            get;
            set;
        }

        public override void LoadSessionObject(IDataRecord record)
        {
            Id = (Guid)record["RelationshipTypeUid"];
            Name = (string)record["RelationshipTypeName"];
        }
    }
}