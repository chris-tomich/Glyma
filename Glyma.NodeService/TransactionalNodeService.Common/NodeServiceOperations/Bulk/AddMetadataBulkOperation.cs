using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using TransactionalNodeService.Common.Model;

namespace TransactionalNodeService.Common.NodeServiceOperations.Bulk
{
    [DataContract(Namespace = "http://sevensigma.com.au/TransactionalNodeService/OpObjects", Name = "BMA")]
    public class AddMetadataBulkOperation : IBulkOperation
    {
        public AddMetadataBulkOperation()
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
        public MapParameter Node
        {
            get;
            set;
        }

        [DataMember(Name = "R")]
        public MapParameter Relationship
        {
            get;
            set;
        }

        [DataMember(Name = "DT")]
        public DescriptorType DescriptorType
        {
            get;
            set;
        }

        [DataMember(Name = "T")]
        public MetadataType MetadataType
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

            response.ResponseParameter = service.AddBulkMetadata(callingUrl, sessionId, ResponseParameterId, DomainId, RootMapId, Node, Relationship, DescriptorType, MetadataType, Name, Value);

            return response;
        }
    }
}