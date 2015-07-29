using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace TransactionalNodeService.NodeServiceOperations.Bulk
{
    public interface IBulkOperation
    {
        int BulkOperationId { get; set; }
        MapParameter DomainId { get; set; }

        BulkOperationResponse SubmitOperation(ITransactionalMappingToolService service, string callingUrl, Guid sessionId);
    }
}