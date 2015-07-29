using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using TransactionalNodeService.Model;

namespace TransactionalNodeService.NodeServiceOperations.Bulk
{
    [DataContract(Namespace = "http://sevensigma.com.au/TransactionalNodeService/OpObjects", Name = "BNA")]
    public class AddNodeBulkOperation : IBulkOperation
    {
        public AddNodeBulkOperation()
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

        [DataMember(Name = "RM")]
        public MapParameter RootMapId
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

        [DataMember(Name = "O")]
        public string OriginalId
        {
            get;
            set;
        }

        public BulkOperationResponse SubmitOperation(ITransactionalMappingToolService service, string callingUrl, Guid sessionId)
        {
            BulkOperationResponse response = new BulkOperationResponse();
            response.BulkOperationId = BulkOperationId;

            response.ResponseParameter = service.AddBulkNode(callingUrl, sessionId, ResponseParameterId, DomainId, RootMapId, NodeType, OriginalId);

            return response;
        }
    }
}