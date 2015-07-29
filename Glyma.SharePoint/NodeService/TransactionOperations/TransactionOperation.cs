using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace NodeService
{
    public abstract class TransactionOperation
    {
        private TransactionToken _result = null;

        protected TransactionOperation()
        {
        }

        public TransactionOperation(SqlConnection connection, TransactionTokenFactory tokenFactory)
        {
            Connection = connection;
            TokenFactory = tokenFactory;

            ResultTokens = new List<TransactionToken>();
        }

        public SqlConnection Connection
        {
            get;
            protected set;
        }

        public TransactionTokenFactory TokenFactory
        {
            get;
            protected set;
        }

        public List<TransactionToken> ResultTokens
        {
            get;
            set;
        }

        public abstract void ExecuteOperation();
    }
}