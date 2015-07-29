using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using TransactionalNodeService.Common.Model;

namespace TransactionalNodeService.Common.NodeServiceOperations.Bulk
{
    [DataContract(Namespace = "http://sevensigma.com.au/TransactionalNodeService/OpObjects", Name = "BRA")]
    public class AddRelationshipBulkOperation : IBulkOperation
    {
        public AddRelationshipBulkOperation()
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

        [DataMember(Name = "N")]
        public Dictionary<DescriptorType, MapParameter> Nodes
        {
            get;
            set;
        }

        [DataMember(Name = "T")]
        public RelationshipType RelationshipType
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

            response.ResponseParameter = service.AddBulkRelationship(callingUrl, sessionId, ResponseParameterId, DomainId, RootMapId, Nodes, RelationshipType, OriginalId);

            return response;
        }
    }
}