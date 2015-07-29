using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace TransactionalNodeService.NodeServiceOperations.Bulk
{
    [DataContract(Namespace = "http://sevensigma.com.au/TransactionalNodeService/OpObjects", Name = "BRD")]
    public class DeleteRelationshipBulkOperation : IBulkOperation
    {
        public DeleteRelationshipBulkOperation()
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
        public MapParameter RelationshipId
        {
            get;
            set;
        }

        public BulkOperationResponse SubmitOperation(ITransactionalMappingToolService service, string callingUrl, Guid sessionId)
        {
            BulkOperationResponse response = new BulkOperationResponse();
            response.BulkOperationId = BulkOperationId;

            response.ResponseParameter = service.DeleteBulkRelationship(callingUrl, sessionId, ResponseParameterId, DomainId, RelationshipId);

            return response;
        }
    }
}