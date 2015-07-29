using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data.SqlClient;
using Microsoft.SharePoint;

namespace NodeService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "TransactionalMappingToolService" in code, svc and config file together.
    public class TransactionalMappingToolService : ITransactionalMappingToolService
    {
        private TransactionTokenFactory _tokenFactory = null;
        private Queue<TransactionOperation> _operations = null;

        private Queue<TransactionOperation> Operations
        {
            get
            {
                if (_operations == null)
                {
                    _operations = new Queue<TransactionOperation>();
                }

                return _operations;
            }
        }

        private SqlConnection Connection
        {
            get;
            set;
        }

        private TransactionTokenFactory TokenFactory
        {
            get
            {
                if (_tokenFactory == null)
                {
                    _tokenFactory = new TransactionTokenFactory();
                }

                return _tokenFactory;
            }
        }

        public void BeginTransaction()
        {
            Operations.Clear();
        }

        public void CompleteTransaction()
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                Connection.Open();
            });

            foreach (TransactionOperation operation in Operations)
            {
                operation.ExecuteOperation();
            }

            Connection.Close();
        }

        public TransactionToken AddNode(TransactionToken domainId, SoapNodeType nodeType, string originalId)
        {
            AddNodeTransactionOperation operation = new AddNodeTransactionOperation(Connection, TokenFactory);

            operation.DomainId = TokenFactory.ProcessToken(domainId);
            operation.NodeType = nodeType;
            operation.OriginalId = originalId;

            Operations.Enqueue(operation);

            return operation.ResultTokens.First();
        }

        public void UpdateNode()
        {
            throw new NotImplementedException();
        }

        public void DeleteNode(TransactionToken domainId, TransactionToken nodeId)
        {
            DeleteNodeTransactionOperation operation = new DeleteNodeTransactionOperation(Connection, TokenFactory);

            operation.DomainId = TokenFactory.ProcessToken(domainId);
            operation.NodeId = TokenFactory.ProcessToken(nodeId);

            Operations.Enqueue(operation);
        }

        public TransactionToken AddRelationship(TransactionToken domainId, Dictionary<SoapDescriptorType, TransactionToken> nodes, SoapRelationshipType relationshipType, string originalId)
        {
            AddRelationshipTransactionOperation operation = new AddRelationshipTransactionOperation(Connection, TokenFactory);

            foreach (KeyValuePair<SoapDescriptorType, TransactionToken> keyValuePair in nodes)
            {
                nodes[keyValuePair.Key] = TokenFactory.ProcessToken(keyValuePair.Value);
            }

            operation.DomainId = TokenFactory.ProcessToken(domainId);
            operation.Nodes = nodes;
            operation.RelationshipType = relationshipType;
            operation.OriginalId = originalId;

            Operations.Enqueue(operation);

            return operation.ResultTokens.First();
        }

        public void UpdateRelationship()
        {
            throw new NotImplementedException();
        }

        public void DeleteRelationship(TransactionToken domainId, TransactionToken relationshipId)
        {
            DeleteRelationshipTransactionOperation operation = new DeleteRelationshipTransactionOperation(Connection, TokenFactory);

            operation.DomainId = TokenFactory.ProcessToken(domainId);
            operation.RelationshipId = TokenFactory.ProcessToken(relationshipId);

            Operations.Enqueue(operation);
        }
    }
}
