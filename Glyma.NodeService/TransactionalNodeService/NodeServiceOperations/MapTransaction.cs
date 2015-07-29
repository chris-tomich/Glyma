using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.Data;
using System.Data.SqlClient;

namespace TransactionalNodeService
{
    public partial class MapTransaction : IPersistableSessionObject, IMapTransaction
    {
        private long _transactionId = -1;
        protected Dictionary<string, object> _parameters = null;

        public MapTransaction()
        {
            IsNew = true;
            IsDirty = false;
        }

        [IgnoreDataMember]
        public bool IsNew
        {
            get;
            set;
        }

        [IgnoreDataMember]
        public bool IsDirty
        {
            get;
            set;
        }

        [IgnoreDataMember]
        protected Dictionary<string, object> Parameters
        {
            get
            {
                if (_parameters == null)
                {
                    _parameters = new Dictionary<string, object>();
                }

                return _parameters;
            }
        }

        [DataMember]
        public long TransactionId
        {
            get
            {
                return _transactionId;
            }
            set
            {
                _transactionId = value;
            }
        }

        [DataMember]
        public DateTime? TransactionTimestamp
        {
            get
            {
                if (Parameters.ContainsKey("TransactionTimestamp"))
                {
                    return (DateTime?)Parameters["TransactionTimestamp"];
                }

                return null;
            }
            set
            {
                Parameters["TransactionTimestamp"] = value;

                IsDirty = true;
            }
        }

        [DataMember]
        public string User
        {
            get
            {
                if (Parameters.ContainsKey("User"))
                {
                    return (string)Parameters["User"];
                }

                return null;
            }
            set
            {
                Parameters["User"] = value;

                IsDirty = true;
            }
        }

        [DataMember]
        public Guid? SessionUid
        {
            get
            {
                if (Parameters.ContainsKey("SessionUid"))
                {
                    return (Guid)Parameters["SessionUid"];
                }

                return Guid.Empty;
            }
            set
            {
                Parameters["SessionUid"] = value;

                IsDirty = true;
            }
        }

        [DataMember]
        public TransactionType? OperationId
        {
            get
            {
                if (Parameters.ContainsKey("OperationId"))
                {
                    return (TransactionType)Parameters["OperationId"];
                }

                return TransactionType.Undefined;
            }
            set
            {
                Parameters["OperationId"] = (int)value;

                IsDirty = true;
            }
        }

        [DataMember]
        public Guid? NodeTypeUid
        {
            get
            {
                if (Parameters.ContainsKey("NodeTypeUid"))
                {
                    return (Guid)Parameters["NodeTypeUid"];
                }

                return null;
            }
            set
            {
                Parameters["NodeTypeUid"] = value;

                IsDirty = true;
            }
        }

        [DataMember]
        public Guid? RelationshipTypeUid
        {
            get
            {
                if (Parameters.ContainsKey("RelationshipTypeUid"))
                {
                    return (Guid)Parameters["RelationshipTypeUid"];
                }

                return null;
            }
            set
            {
                Parameters["RelationshipTypeUid"] = value;

                IsDirty = true;
            }
        }

        [DataMember]
        public Guid? DescriptorTypeUid
        {
            get
            {
                if (Parameters.ContainsKey("DescriptorTypeUid"))
                {
                    return (Guid)Parameters["DescriptorTypeUid"];
                }

                return null;
            }
            set
            {
                Parameters["DescriptorTypeUid"] = value;

                IsDirty = true;
            }
        }

        [DataMember]
        public Guid? MetadataTypeUid
        {
            get
            {
                if (Parameters.ContainsKey("MetadataTypeUid"))
                {
                    return (Guid)Parameters["MetadataTypeUid"];
                }

                return null;
            }
            set
            {
                Parameters["MetadataTypeUid"] = value;

                IsDirty = true;
            }
        }

        [DataMember]
        public string MetadataName
        {
            get
            {
                if (Parameters.ContainsKey("MetadataName"))
                {
                    return (string)Parameters["MetadataName"];
                }

                return null;
            }
            set
            {
                Parameters["MetadataName"] = value;

                IsDirty = true;
            }
        }

        [DataMember]
        public string MetadataValue
        {
            get
            {
                if (Parameters.ContainsKey("MetadataValue"))
                {
                    return (string)Parameters["MetadataValue"];
                }

                return null;
            }
            set
            {
                Parameters["MetadataValue"] = value;

                IsDirty = true;
            }
        }

        [DataMember]
        public Guid? DomainParameterUid
        {
            get
            {
                if (Parameters.ContainsKey("DomainParameterUid"))
                {
                    return (Guid)Parameters["DomainParameterUid"];
                }

                return null;
            }
            set
            {
                Parameters["DomainParameterUid"] = value;

                IsDirty = true;
            }
        }

        [DataMember]
        public Guid? RootMapParameterUid
        {
            get
            {
                if (Parameters.ContainsKey("RootMapParameterUid"))
                {
                    return (Guid)Parameters["RootMapParameterUid"];
                }

                return null;
            }
            set
            {
                Parameters["RootMapParameterUid"] = value;

                IsDirty = true;
            }
        }

        [DataMember]
        public Guid? NodeParameterUid
        {
            get
            {
                if (Parameters.ContainsKey("NodeParameterUid"))
                {
                    return (Guid)Parameters["NodeParameterUid"];
                }

                return null;
            }
            set
            {
                Parameters["NodeParameterUid"] = value;

                IsDirty = true;
            }
        }

        [DataMember]
        public Guid? RelationshipParameterUid
        {
            get
            {
                if (Parameters.ContainsKey("RelationshipParameterUid"))
                {
                    return (Guid)Parameters["RelationshipParameterUid"];
                }

                return null;
            }
            set
            {
                Parameters["RelationshipParameterUid"] = value;

                IsDirty = true;
            }
        }

        [DataMember]
        public Guid? DescriptorParameterUid
        {
            get
            {
                if (Parameters.ContainsKey("DescriptorParameterUid"))
                {
                    return (Guid)Parameters["DescriptorParameterUid"];
                }

                return null;
            }
            set
            {
                Parameters["DescriptorParameterUid"] = value;

                IsDirty = true;
            }
        }

        [DataMember]
        public Guid? MetadataParameterUid
        {
            get
            {
                if (Parameters.ContainsKey("MetadataParameterUid"))
                {
                    return (Guid)Parameters["MetadataParameterUid"];
                }

                return null;
            }
            set
            {
                Parameters["MetadataParameterUid"] = value;

                IsDirty = true;
            }
        }

        [DataMember]
        public Guid? ResponseParameterUid
        {
            get
            {
                if (Parameters.ContainsKey("ResponseParameterUid"))
                {
                    return (Guid)Parameters["ResponseParameterUid"];
                }

                return null;
            }
            set
            {
                Parameters["ResponseParameterUid"] = value;

                IsDirty = true;
            }
        }

        public void PersistSessionObject(IDbConnectionAbstraction connectionAbstraction)
        {
            Dictionary<string, object>.Enumerator parameters = _parameters.GetEnumerator();

            using (SqlCommand command = new SqlCommand())
            {
                IQueryBuilder queryBuilder = null;

                if (IsNew)
                {
                    queryBuilder = new InsertQueryBuilder();
                }
                else if (IsDirty)
                {
                    queryBuilder = new UpdateQueryBuilder();
                }
                else
                {
                    return;
                }

                while (parameters.MoveNext())
                {
                    queryBuilder.AddParameter(parameters.Current.Key, parameters.Current.Value);
                }

                command.Parameters.AddRange(queryBuilder.GenerateSqlParameters());
                command.Connection = connectionAbstraction.Connection;
                command.CommandText = queryBuilder.GenerateSqlQuery();

                connectionAbstraction.Open();
                TransactionId = long.Parse(command.ExecuteScalar().ToString());
                connectionAbstraction.Close();
            }
        }

        public void LoadSessionObject(IDataRecord record)
        {
            TransactionId = (long)record["TransactionId"];
            User = (string)record["User"];
            SessionUid = (Guid)record["SessionUid"];

            if (record["TransactionTimestamp"] != DBNull.Value)
            {
                TransactionTimestamp = (DateTime)record["TransactionTimestamp"];
            }

            if (record["OperationId"] != DBNull.Value)
            {
                OperationId = (TransactionType)record["OperationId"];
            }

            if (record["ResponseParameterUid"] != null && record["ResponseParameterUid"] != DBNull.Value)
            {
                ResponseParameterUid = (Guid)record["ResponseParameterUid"];
            }

            if (record["DomainParameterUid"] != null && record["DomainParameterUid"] != DBNull.Value)
            {
                DomainParameterUid = (Guid)record["DomainParameterUid"];
            }

            if (record["RootMapParameterUid"] != null && record["RootMapParameterUid"] != DBNull.Value)
            {
                RootMapParameterUid = (Guid)record["RootMapParameterUid"];
            }

            if (record["NodeParameterUid"] != null && record["NodeParameterUid"] != DBNull.Value)
            {
                NodeParameterUid = (Guid)record["NodeParameterUid"];
            }

            if (record["DescriptorParameterUid"] != null && record["DescriptorParameterUid"] != DBNull.Value)
            {
                DescriptorParameterUid = (Guid)record["DescriptorParameterUid"];
            }

            if (record["RelationshipParameterUid"] != null && record["RelationshipParameterUid"] != DBNull.Value)
            {
                RelationshipParameterUid = (Guid)record["RelationshipParameterUid"];
            }

            if (record["MetadataParameterUid"] != null && record["MetadataParameterUid"] != DBNull.Value)
            {
                MetadataParameterUid = (Guid)record["MetadataParameterUid"];
            }

            if (record["NodeTypeUid"] != null && record["NodeTypeUid"] != DBNull.Value)
            {
                NodeTypeUid = (Guid)record["NodeTypeUid"];
            }

            if (record["DescriptorTypeUid"] != null && record["DescriptorTypeUid"] != DBNull.Value)
            {
                DescriptorTypeUid = (Guid)record["DescriptorTypeUid"];
            }

            if (record["RelationshipTypeUid"] != null && record["RelationshipTypeUid"] != DBNull.Value)
            {
                RelationshipTypeUid = (Guid)record["RelationshipTypeUid"];
            }

            if (record["MetadataTypeUid"] != null && record["MetadataTypeUid"] != DBNull.Value)
            {
                MetadataTypeUid = (Guid)record["MetadataTypeUid"];
            }

            if (record["MetadataName"] != null && record["MetadataName"] != DBNull.Value)
            {
                MetadataName = (string)record["MetadataName"];
            }

            if (record["MetadataValue"] != null && record["MetadataValue"] != DBNull.Value)
            {
                MetadataValue = (string)record["MetadataValue"];
            }

            IsNew = false;
            IsDirty = false;
        }
    }
}