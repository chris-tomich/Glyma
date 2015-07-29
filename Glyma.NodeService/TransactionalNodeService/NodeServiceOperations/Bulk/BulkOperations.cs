using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace TransactionalNodeService.NodeServiceOperations.Bulk
{
    [DataContract(Namespace = "http://sevensigma.com.au/TransactionalNodeService/OpObjects", Name = "OPS")]
    [KnownType(typeof(AddNodeBulkOperation))]
    [KnownType(typeof(AddRelationshipBulkOperation))]
    [KnownType(typeof(AddMetadataBulkOperation))]
    [KnownType(typeof(DeleteNodeBulkOperation))]
    [KnownType(typeof(DeleteRelationshipBulkOperation))]
    [KnownType(typeof(DeleteMetadataBulkOperation))]
    [KnownType(typeof(UpdateNodeBulkOperation))]
    [KnownType(typeof(UpdateRelationshipBulkOperation))]
    [KnownType(typeof(UpdateMetadataBulkOperation))]
    public class BulkOperations
    {
        public BulkOperations()
        {
        }

        [DataMember(Name = "O")]
        public List<IBulkOperation> Operations
        {
            get;
            set;
        }
    }
}