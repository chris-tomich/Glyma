using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Text;
using System.Data;
using System.Runtime.Serialization;
using TransactionalNodeService.Common.TransactionOperations;

namespace TransactionalNodeService.Common
{
    [DataContract]
    public class MapTransactionWrapper : IPersistableSessionObject, IMapTransaction
    {
        protected MapParameter _domainParameter = null;
        protected MapParameter _rootMapParameter = null;
        protected MapParameter _nodeParameter = null;
        protected MapParameter _relationshipParameter = null;
        protected MapParameter _descriptorParameter = null;
        protected MapParameter _metadataParameter = null;
        protected MapParameter _responseParameter = null;
        protected MapParameterType _responseParameterType = MapParameterType.Empty;
        protected ITransactionOperation _operationEngine = null;

        protected MapTransactionWrapper()
        {
            Core = new MapTransaction();
        }

        public MapTransactionWrapper(IGlymaSession glymaSession)
            : this()
        {
            GlymaSession = glymaSession;
            Core.SessionUid = GlymaSession.Session.Id;
            Core.User = GlymaSession.Session.User;
            Core.TransactionTimestamp = DateTime.Now.ToUniversalTime();
        }

        public MapTransaction Core
        {
            get;
            protected set;
        }

        protected IGlymaSession GlymaSession
        {
            get;
            set;
        }

        protected MapParameterType ResponseParameterType
        {
            get
            {
                if (_responseParameterType == MapParameterType.Empty)
                {
                    switch (Core.OperationId)
                    {
                        case TransactionType.Undefined:
                            _responseParameterType = MapParameterType.Unknown;
                            break;
                        case TransactionType.BeginSession:
                            _responseParameterType = MapParameterType.Unknown;
                            break;
                        case TransactionType.CompleteSession:
                            _responseParameterType = MapParameterType.Unknown;
                            break;
                        case TransactionType.CreateNode:
                            _responseParameterType = MapParameterType.Node;
                            break;
                        case TransactionType.DeleteNode:
                            _responseParameterType = MapParameterType.Node;
                            break;
                        case TransactionType.CreateRelationship:
                            _responseParameterType = MapParameterType.Relationship;
                            break;
                        case TransactionType.DeleteRelationship:
                            _responseParameterType = MapParameterType.Relationship;
                            break;
                        case TransactionType.CreateDescriptor:
                            _responseParameterType = MapParameterType.Descriptor;
                            break;
                        case TransactionType.CreateMetadata:
                            _responseParameterType = MapParameterType.Metadata;
                            break;
                        case TransactionType.UpdateMetadata:
                            _responseParameterType = MapParameterType.Metadata;
                            break;
                        case TransactionType.DeleteMetadata:
                            _responseParameterType = MapParameterType.Metadata;
                            break;
                        case TransactionType.UpdateNode:
                            _responseParameterType = MapParameterType.Node;
                            break;
                        case TransactionType.UpdateRelationship:
                            _responseParameterType = MapParameterType.Relationship;
                            break;
                        case TransactionType.UpdateDescriptor:
                            _responseParameterType = MapParameterType.Descriptor;
                            break;
                        default:
                            _responseParameterType = MapParameterType.Unknown;
                            break;
                    }
                }

                return _responseParameterType;
            }
        }

        public bool IsNew
        {
            get
            {
                return Core.IsNew;
            }
        }

        public bool IsDirty
        {
            get
            {
                return Core.IsDirty;
            }
        }

        #region Explicit Declaration of IMapTransaction
        long IMapTransaction.TransactionId
        {
            get
            {
                return Core.TransactionId;
            }
            set
            {
                Core.TransactionId = value;
            }
        }

        DateTime? IMapTransaction.TransactionTimestamp
        {
            get
            {
                return Core.TransactionTimestamp;
            }
            set
            {
                Core.TransactionTimestamp = value;
            }
        }

        string IMapTransaction.User
        {
            get
            {
                return Core.User;
            }
            set
            {
                Core.User = value;
            }
        }

        Guid? IMapTransaction.SessionUid
        {
            get
            {
                return Core.SessionUid;
            }
            set
            {
                Core.SessionUid = value;
            }
        }

        TransactionType? IMapTransaction.OperationId
        {
            get
            {
                _operationEngine = null;
                return Core.OperationId;
            }
            set
            {
                Core.OperationId = value;
            }
        }

        Guid? IMapTransaction.NodeTypeUid
        {
            get
            {
                return Core.NodeTypeUid;
            }
            set
            {
                Core.NodeTypeUid = value;
            }
        }

        Guid? IMapTransaction.RelationshipTypeUid
        {
            get
            {
                return Core.RelationshipTypeUid;
            }
            set
            {
                Core.RelationshipTypeUid = value;
            }
        }

        Guid? IMapTransaction.DescriptorTypeUid
        {
            get
            {
                return Core.DescriptorTypeUid;
            }
            set
            {
                Core.DescriptorTypeUid = value;
            }
        }

        Guid? IMapTransaction.MetadataTypeUid
        {
            get
            {
                return Core.MetadataTypeUid;
            }
            set
            {
                Core.MetadataTypeUid = value;
            }
        }

        string IMapTransaction.MetadataName
        {
            get
            {
                return Core.MetadataName;
            }
            set
            {
                Core.MetadataName = value;
            }
        }

        string IMapTransaction.MetadataValue
        {
            get
            {
                return Core.MetadataValue;
            }
            set
            {
                Core.MetadataValue = value;
            }
        }

        Guid? IMapTransaction.DomainParameterUid
        {
            get
            {
                return Core.DomainParameterUid;
            }
        }

        Guid? IMapTransaction.RootMapParameterUid
        {
            get
            {
                return Core.RootMapParameterUid;
            }
        }

        Guid? IMapTransaction.NodeParameterUid
        {
            get
            {
                return Core.NodeParameterUid;
            }
        }

        Guid? IMapTransaction.DescriptorParameterUid
        {
            get
            {
                return Core.DescriptorParameterUid;
            }
        }

        Guid? IMapTransaction.RelationshipParameterUid
        {
            get
            {
                return Core.RelationshipParameterUid;
            }
        }

        Guid? IMapTransaction.MetadataParameterUid
        {
            get
            {
                return Core.MetadataParameterUid;
            }
        }

        Guid? IMapTransaction.ResponseParameterUid
        {
            get
            {
                return Core.ResponseParameterUid;
            }
        }
        #endregion

        public MapParameter DomainParameter
        {
            get
            {
                if (_domainParameter == null)
                {
                    if (Core.DomainParameterUid == null || Core.DomainParameterUid.HasValue == false || Core.DomainParameterUid == Guid.Empty)
                    {
                        return null;
                    }
                    else
                    {
                        _domainParameter = GlymaSession.Parameters[Core.DomainParameterUid.Value];
                    }
                }

                return _domainParameter;
            }
            set
            {
                _domainParameter = value;
                Core.DomainParameterUid = _domainParameter.Id;
            }
        }

        public MapParameter RootMapParameter
        {
            get
            {
                if (_rootMapParameter == null)
                {
                    if (Core.RootMapParameterUid == null || Core.RootMapParameterUid.HasValue == false || Core.RootMapParameterUid == Guid.Empty)
                    {
                        return null;
                    }
                    else
                    {
                        _rootMapParameter = GlymaSession.Parameters[Core.RootMapParameterUid.Value];
                    }
                }

                return _rootMapParameter;
            }
            set
            {
                _rootMapParameter = value;

                if (value == null)
                {
                    Core.RootMapParameterUid = null;
                }
                else
                {
                    Core.RootMapParameterUid = _rootMapParameter.Id;
                }
            }
        }

        public MapParameter NodeParameter
        {
            get
            {
                if (_nodeParameter == null)
                {
                    if (Core.NodeParameterUid == null || Core.NodeParameterUid.HasValue == false || Core.NodeParameterUid == Guid.Empty)
                    {
                        return null;
                    }
                    else
                    {
                        _nodeParameter = GlymaSession.Parameters[Core.NodeParameterUid.Value];
                    }
                }

                return _nodeParameter;
            }
            set
            {
                _nodeParameter = value;

                if (value == null)
                {
                    Core.NodeParameterUid = null;
                }
                else
                {
                    Core.NodeParameterUid = _nodeParameter.Id;
                }
            }
        }

        public MapParameter RelationshipParameter
        {
            get
            {
                if (_relationshipParameter == null)
                {
                    if (Core.RelationshipParameterUid == null || Core.RelationshipParameterUid.HasValue == false || Core.RelationshipParameterUid == Guid.Empty)
                    {
                        return null;
                    }
                    else
                    {
                        _relationshipParameter = GlymaSession.Parameters[Core.RelationshipParameterUid.Value];
                    }
                }

                return _relationshipParameter;
            }
            set
            {
                _relationshipParameter = value;

                if (value == null)
                {
                    Core.RelationshipParameterUid = null;
                }
                else
                {
                    Core.RelationshipParameterUid = _relationshipParameter.Id;
                }
            }
        }

        public MapParameter DescriptorParameter
        {
            get
            {
                if (_descriptorParameter == null)
                {
                    if (Core.DescriptorParameterUid == null || Core.DescriptorParameterUid.HasValue == false || Core.DescriptorParameterUid == Guid.Empty)
                    {
                        return null;
                    }
                    else
                    {
                        _descriptorParameter = GlymaSession.Parameters[Core.DescriptorParameterUid.Value];
                    }
                }

                return _descriptorParameter;
            }
            set
            {
                _descriptorParameter = value;

                if (value == null)
                {
                    Core.DescriptorParameterUid = null;
                }
                else
                {
                    Core.DescriptorParameterUid = _descriptorParameter.Id;
                }
            }
        }

        public MapParameter MetadataParameter
        {
            get
            {
                if (_metadataParameter == null)
                {
                    if (Core.MetadataParameterUid == null || Core.MetadataParameterUid.HasValue == false || Core.MetadataParameterUid == Guid.Empty)
                    {
                        return null;
                    }
                    else
                    {
                        _metadataParameter = GlymaSession.Parameters[Core.MetadataParameterUid.Value];
                    }
                }

                return _metadataParameter;
            }
            set
            {
                _metadataParameter = value;

                if (value == null)
                {
                    Core.MetadataParameterUid = null;
                }
                else
                {
                    Core.MetadataParameterUid = _metadataParameter.Id;
                }
            }
        }

        public MapParameter ResponseParameter
        {
            get
            {
                if (_responseParameter == null)
                {
                    if (Core.ResponseParameterUid == null || Core.ResponseParameterUid.HasValue == false || Core.ResponseParameterUid == Guid.Empty)
                    {
                        _responseParameter = GlymaSession.Parameters.AddParameter(ResponseParameterType, Guid.Empty, true);
                        Core.ResponseParameterUid = _responseParameter.Id;
                    }
                    else
                    {
                        _responseParameter = GlymaSession.Parameters[Core.ResponseParameterUid.Value];
                    }
                }

                return _responseParameter;
            }
        }

        public ITransactionOperation OperationEngine
        {
            get
            {
                if (_operationEngine == null)
                {
                    if (Core.OperationId != null && Core.OperationId.HasValue)
                    {
                        _operationEngine = CreateTransactionOperation(Core.OperationId.Value);
                    }
                    else
                    {
                        throw new NullReferenceException("This transaction does not have a type. Can't build operation engine.");
                    }
                }

                return _operationEngine;
            }
        }

        protected ITransactionOperation CreateTransactionOperation(TransactionType transactionType)
        {
            ITransactionOperation transactionOperation;

            switch (transactionType)
            {
                case TransactionType.CreateNode:
                    transactionOperation = new CreateNode(GlymaSession.Parameters);
                    break;
                case TransactionType.DeleteNode:
                    transactionOperation = new DeleteNode(GlymaSession.Parameters);
                    break;
                case TransactionType.CreateRelationship:
                    transactionOperation = new CreateRelationship(GlymaSession.Parameters);
                    break;
                case TransactionType.DeleteRelationship:
                    transactionOperation = new DeleteRelationship(GlymaSession.Parameters);
                    break;
                case TransactionType.CreateDescriptor:
                    transactionOperation = new CreateDescriptor(GlymaSession.Parameters);
                    break;
                case TransactionType.CreateMetadata:
                    transactionOperation = new CreateMetadata(GlymaSession.Parameters);
                    break;
                case TransactionType.UpdateMetadata:
                    transactionOperation = new UpdateMetadata(GlymaSession.Parameters);
                    break;
                case TransactionType.DeleteMetadata:
                    transactionOperation = new DeleteMetadata(GlymaSession.Parameters);
                    break;
                case TransactionType.UpdateNode:
                    transactionOperation = new UpdateNode(GlymaSession.Parameters);
                    break;
                case TransactionType.UpdateRelationship:
                    transactionOperation = new UpdateRelationship(GlymaSession.Parameters);
                    break;
                case TransactionType.UpdateDescriptor:
                    transactionOperation = new UpdateDescriptor(GlymaSession.Parameters);
                    break;
                default:
                    throw new NotSupportedException("The requested transaction type doesn't exist.");
            }

            return transactionOperation;
        }

        public void LoadSessionObject(IDataRecord record)
        {
            Core.LoadSessionObject(record);
        }

        public void PersistSessionObject(IDbConnectionAbstraction connectionAbstraction)
        {
            Core.PersistSessionObject(connectionAbstraction);
        }

        public void ExecuteOperation(ref MapResponse response)
        {
            if (OperationEngine != null)
            {
                using (IDbConnectionAbstraction mapDbConnection = GlymaSession.ConnectionFactory.CreateMapDbConnection())
                {
                    OperationEngine.ExecuteTransaction(mapDbConnection, this, ref response);
                }
            }
        }

        internal MapParameter BuildResponseParameterFromGuid(Guid responseParameter)
        {
            _responseParameter = GlymaSession.Parameters.AddParameter(responseParameter, ResponseParameterType, Guid.Empty, true);
            Core.ResponseParameterUid = _responseParameter.Id;

            return _responseParameter;
        }
    }
}