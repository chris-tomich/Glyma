using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace TransactionalNodeService.TransactionOperations
{
    public interface ITransactionOperation
    {
        MapParameters SessionMapParameters { get; }
        TransactionType OperationType { get; }

        void ExecuteTransaction(IDbConnectionAbstraction connection, MapTransactionWrapper transactionWrapper, ref MapResponse response);
    }
}