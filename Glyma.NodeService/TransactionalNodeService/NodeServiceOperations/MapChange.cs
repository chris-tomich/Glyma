using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TransactionalNodeService.Model;
using System.Runtime.Serialization;

namespace TransactionalNodeService
{
    [DataContract(Namespace = "http://sevensigma.com.au/TransactionalNodeService/OpObjects", Name = "CH")]
    public class MapChange
    {
        public MapChange()
        {
        }

        public MapChange(long transactionId, Node node, TransactionType operation)
        {
            TransactionId = transactionId;
            Node = node;
            Operation = operation;
        }

        public MapChange(long transactionId, Relationship relationship, TransactionType operation)
        {
            TransactionId = transactionId;
            Relationship = relationship;
            Operation = operation;
        }

        public MapChange(long transactionId, Guid mapParameter, Node node, TransactionType operation)
        {
            TransactionId = transactionId;
            MapParameter = mapParameter;
            Node = node;
            Operation = operation;
        }

        public MapChange(long transactionId, Guid mapParameter, Relationship relationship, TransactionType operation)
        {
            TransactionId = transactionId;
            MapParameter = mapParameter;
            Relationship = relationship;
            Operation = operation;
        }

        public MapChange(long transactionId, Guid mapParameter, Metadata metadata, TransactionType operation)
        {
            TransactionId = transactionId;
            MapParameter = mapParameter;
            Metadata = metadata;
            Operation = operation;
        }

        public MapChange(long transactionId, Guid mapParameter, Descriptor descriptor, TransactionType operation)
        {
            TransactionId = transactionId;
            MapParameter = mapParameter;
            Descriptor = descriptor;
            Operation = operation;
        }

        public MapChange(long transactionId, Descriptor descriptor, TransactionType operation)
        {
            TransactionId = transactionId;
            Descriptor = descriptor;
            Operation = operation;
        }

        public MapChange(long transactionId, Metadata metadata, TransactionType operation)
        {
            TransactionId = transactionId;
            Metadata = metadata;
            Operation = operation;
        }

        [DataMember(Name = "T")]
        public long TransactionId
        {
            get;
            set;
        }

        [DataMember(Name = "P")]
        public Guid MapParameter
        {
            get;
            set;
        }

        [DataMember(Name = "O")]
        public TransactionType Operation
        {
            get;
            set;
        }

        [DataMember(Name = "N")]
        public Node Node
        {
            get;
            set;
        }

        [DataMember(Name = "R")]
        public Relationship Relationship
        {
            get;
            set;
        }

        [DataMember(Name = "D")]
        public Descriptor Descriptor
        {
            get;
            set;
        }

        [DataMember(Name = "M")]
        public Metadata Metadata
        {
            get;
            set;
        }
    }
}