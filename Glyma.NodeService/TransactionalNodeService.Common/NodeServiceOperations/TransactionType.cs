using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransactionalNodeService.Common
{
    public enum TransactionType
    {
        Undefined = 0,
        BeginSession = 1,
        CompleteSession = 2,
        CreateNode = 3,
        DeleteNode = 4,
        CreateRelationship = 5,
        DeleteRelationship = 6,
        CreateDescriptor = 7,
        CreateMetadata = 8,
        UpdateMetadata = 9,
        DeleteMetadata = 10,
        UpdateNode = 11,
        UpdateRelationship = 12,
        UpdateDescriptor = 13,
        CreateDomain = 14,
        CreateRootMap = 15,
        DeleteDomain = 16,
        DeleteRootMap = 17
    }
}