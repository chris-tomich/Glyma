using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using TransactionalNodeService.Common.Model;

namespace TransactionalNodeService.Common.NodeServiceOperations.Bulk
{
    [DataContract(Namespace = "http://sevensigma.com.au/TransactionalNodeService/OpObjects", Name = "BNU")]
    public class UpdateNodeBulkOperation : IBulkOperation
    {
        public UpdateNodeBulkOperation()
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

        [DataMember(Name = "N")]
        public MapParameter NodeId
        {
            get;
            set;
        }

        [DataMember(Name = "T")]
        public NodeType NodeType
        {
            get;
            set;
        }

        public BulkOperationResponse SubmitOperation(ITransactionalMappingToolService service, string callingUrl, Guid sessionId)
        {
            BulkOperationResponse response = new BulkOperationResponse();
            response.BulkOperationId = BulkOperationId;

            response.ResponseParameter = service.UpdateBulkNode(callingUrl, sessionId, ResponseParameterId, DomainId, NodeId, NodeType);

            return response;
        }
    }
}