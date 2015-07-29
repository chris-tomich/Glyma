using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransactionalNodeService.Common
{
    public interface IMapTransaction
    {
        long TransactionId { get; set; }
        DateTime? TransactionTimestamp { get; set; }
        string User { get; set; }
        Guid? SessionUid { get; set; }
        TransactionType? OperationId { get; set; }
        Guid? NodeTypeUid { get; set; }
        Guid? RelationshipTypeUid { get; set; }
        Guid? DescriptorTypeUid { get; set; }
        Guid? MetadataTypeUid { get; set; }
        string MetadataName { get; set; }
        string MetadataValue { get; set; }
        Guid? DomainParameterUid { get; }
        Guid? RootMapParameterUid { get; }
        Guid? NodeParameterUid { get; }
        Guid? RelationshipParameterUid { get; }
        Guid? DescriptorParameterUid { get; }
        Guid? MetadataParameterUid { get; }
        Guid? ResponseParameterUid { get; }
    }
}