using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace TransactionalNodeService.Common.NodeServiceOperations.Bulk
{
    [DataContract(Namespace = "http://sevensigma.com.au/TransactionalNodeService/OpObjects", Name = "BMU")]
    public class UpdateMetadataBulkOperation : IBulkOperation
    {
        public UpdateMetadataBulkOperation()
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

        [DataMember(Name = "M")]
        public MapParameter Metadata
        {
            get;
            set;
        }

        [DataMember(Name = "MN")]
        public string Name
        {
            get;
            set;
        }

        [DataMember(Name = "MV")]
        public string Value
        {
            get;
            set;
        }

        public BulkOperationResponse SubmitOperation(ITransactionalMappingToolService service, string callingUrl, Guid sessionId)
        {
            BulkOperationResponse response = new BulkOperationResponse();
            response.BulkOperationId = BulkOperationId;

            response.ResponseParameter = service.UpdateBulkMetadata(callingUrl, sessionId, ResponseParameterId, DomainId, Metadata, Name, Value);

            return response;
        }
    }
}