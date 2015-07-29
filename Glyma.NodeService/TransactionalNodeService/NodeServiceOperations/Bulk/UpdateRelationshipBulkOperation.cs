using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using TransactionalNodeService.Model;

namespace TransactionalNodeService.NodeServiceOperations.Bulk
{
    [DataContract(Namespace = "http://sevensigma.com.au/TransactionalNodeService/OpObjects", Name = "BRU")]
    public class UpdateRelationshipBulkOperation : IBulkOperation
    {
        public UpdateRelationshipBulkOperation()
        {
        }

        [DataMember(Name = "I")]
        public int BulkOperationId
        {
            get;
            set;
        }

        [DataMember(Name = "RI")]
        public Guid ResponseParameterId
        {
            get;
            set;
        }

        [DataMember(Name = "D")]
        public MapParameter DomainId
        {
            get;
            set;
        }

        [DataMember(Name = "R")]
        MapParameter RelationshipId
        {
            get;
            set;
        }

        [DataMember(Name = "N")]
        Dictionary<DescriptorType, MapParameter> Nodes
        {
            get;
            set;
        }

        [DataMember(Name = "T")]
        RelationshipType RelationshipType
        {
            get;
            set;
        }

        public BulkOperationResponse SubmitOperation(ITransactionalMappingToolService service, string callingUrl, Guid sessionId)
        {
            BulkOperationResponse response = new BulkOperationResponse();
            response.BulkOperationId = BulkOperationId;

            response.ResponseParameter = service.UpdateBulkRelationship(callingUrl, sessionId, ResponseParameterId, DomainId, RelationshipId, Nodes, RelationshipType);

            return response;
        }
    }
}