using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace NodeService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ITransactionalMappingToolService" in both code and config file together.
    [ServiceContract(Namespace = "http://sevensigma.com.au/NodeService", SessionMode=SessionMode.Required)]
    public interface ITransactionalMappingToolService
    {
        [OperationContract(IsInitiating = true,IsTerminating = false)]
        void BeginTransaction();

        [OperationContract(IsInitiating = false, IsTerminating = true)]
        void CompleteTransaction();

        [OperationContract(IsInitiating = false, IsTerminating = false)]
        TransactionToken AddNode(TransactionToken domainId, SoapNodeType nodeType, string originalId);

        [OperationContract(IsInitiating = false, IsTerminating = false)]
        void UpdateNode();

        [OperationContract(IsInitiating = false, IsTerminating = false)]
        void DeleteNode(TransactionToken domainId, TransactionToken nodeId);

        [OperationContract(IsInitiating = false, IsTerminating = false)]
        TransactionToken AddRelationship(TransactionToken domainId, Dictionary<SoapDescriptorType, TransactionToken> nodes, SoapRelationshipType relationshipType, string originalId);

        [OperationContract(IsInitiating = false, IsTerminating = false)]
        void UpdateRelationship();

        [OperationContract(IsInitiating = false, IsTerminating = false)]
        void DeleteRelationship(TransactionToken domainId, TransactionToken relationshipId);
    }
}
