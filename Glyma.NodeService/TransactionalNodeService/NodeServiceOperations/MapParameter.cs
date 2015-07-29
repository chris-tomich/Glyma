using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Runtime.Serialization;

namespace TransactionalNodeService
{
    public enum MapParameterType
    {
        Empty,
        Unknown,
        Domain,
        Node,
        Relationship,
        Descriptor,
        Metadata,
        RootMap
    }

    [DataContract(Namespace = "http://sevensigma.com.au/TransactionalNodeService/OpObjects", Name = "MP")]
    public class MapParameter : IPersistableSessionObject
    {
        protected Guid _id;
        protected Guid _value;
        protected Guid _sessionId;
        protected bool _isDelayed;
        protected const string CheckParameterExists = "SELECT COUNT([ParameterUid]) FROM [Parameters] WHERE [ParameterUid] = @ParameterUid;";
        protected const string InsertParameter = "INSERT INTO [Parameters] ([ParameterUid], [Value], [SessionUid], [IsDelayed], [ParameterType]) VALUES (@ParameterUid, @Value, @SessionUid, @IsDelayed, @ParameterType)";
        //protected const string UpdateParameter = "UPDATE [Parameters] SET [Value] = @Value, [SessionUid] = @SessionUid, [IsDelayed] = @IsDelayed, [ParameterType] = @ParameterType WHERE [ParameterUid] = @ParameterUid";
        protected const string UpdateParameter = "UPDATE [Parameters] SET {0}[SessionUid] = @SessionUid, [IsDelayed] = @IsDelayed, [ParameterType] = @ParameterType WHERE [ParameterUid] = @ParameterUid";

        public MapParameter()
        {
            // This perhaps needs to be replaced in the future with sequential GUIDs. Testing on performance needs to be eventually performed.
            Id = Guid.NewGuid();

            // As this object has just been created, we need to assume that it is completely new. In an event that this will become the container for a
            // pre-existing parameter, the "new" and "dirty" states will be reset.
            IsNew = true;
            IsDirty = false;
            IsDbInstance = false;
            ValueUpdated = false;
        }

        public MapParameter(Guid newGuid)
        {
            Id = newGuid;

            // As this object has just been created, we need to assume that it is completely new. In an event that this will become the container for a
            // pre-existing parameter, the "new" and "dirty" states will be reset.
            IsNew = true;
            IsDirty = false;
            IsDbInstance = false;
            ValueUpdated = false;
        }

        [DataMember(Name = "I")]
        public Guid Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
                IsNew = true;
            }
        }

        [DataMember(Name = "V")]
        public Guid Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (value == Guid.Empty && _value != Guid.Empty)
                {
                    return;
                }

                _value = value;
                IsDirty = true;
                ValueUpdated = true;
            }
        }

        [DataMember(Name = "S")]
        public Guid SessionId
        {
            get
            {
                return _sessionId;
            }
            set
            {
                _sessionId = value;
                IsDirty = true;
            }
        }

        [DataMember(Name = "D")]
        public bool IsDelayed
        {
            get
            {
                return _isDelayed;
            }
            set
            {
                _isDelayed = value;
                IsDirty = true;
            }
        }

        [DataMember(Name = "T")]
        public MapParameterType ParameterType
        {
            get;
            set;
        }

        [IgnoreDataMember]
        protected SqlParameter IdSqlParameter
        {
            get
            {
                SqlParameter idParameter = new SqlParameter("@ParameterUid", Id);

                return idParameter;
            }
        }

        [IgnoreDataMember]
        protected SqlParameter ValueSqlParameter
        {
            get
            {
                SqlParameter valueParameter = new SqlParameter("@Value", Value);

                return valueParameter;
            }
        }

        [IgnoreDataMember]
        protected SqlParameter SessionIdSqlParameter
        {
            get
            {
                SqlParameter sessionIdParameter = new SqlParameter("@SessionUid", SessionId);

                return sessionIdParameter;
            }
        }

        [IgnoreDataMember]
        protected SqlParameter IsDelayedSqlParameter
        {
            get
            {
                SqlParameter isDelayedParameter = new SqlParameter("@IsDelayed", IsDelayed);

                return isDelayedParameter;
            }
        }

        [IgnoreDataMember]
        protected SqlParameter ParameterTypeSqlParameter
        {
            get
            {
                SqlParameter paramaterTypeParameter = new SqlParameter("@ParameterType", ParameterType);

                return paramaterTypeParameter;
            }
        }

        [IgnoreDataMember]
        public bool IsNew
        {
            get;
            protected set;
        }

        [IgnoreDataMember]
        public bool IsDirty
        {
            get;
            protected set;
        }

        [IgnoreDataMember]
        public bool ValueUpdated
        {
            get;
            protected set;
        }

        /// <summary>
        /// This is true if this MapParameter's data was loaded from the database. It is false if this MapParameter came from a SOAP call or was created manually.
        /// </summary>
        [IgnoreDataMember]
        public bool IsDbInstance
        {
            get;
            protected set;
        }

        public bool ExistsInDb(IDbConnectionAbstraction connectionAbstraction)
        {
            SqlCommand checkExistsSqlCommand = new SqlCommand(CheckParameterExists, connectionAbstraction.Connection);
            checkExistsSqlCommand.Parameters.Add(IdSqlParameter);

            connectionAbstraction.Open();
            int numberOfItems = (int)checkExistsSqlCommand.ExecuteScalar();
            connectionAbstraction.Close();

            if (numberOfItems > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void PersistSessionObject(IDbConnectionAbstraction connectionAbstraction)
        {
            /// If this is a delayed transaction then we'll never need to update this. The reasons are -
            /// 1. Changing the ID of this object is pointless as it would violate referential integrity.
            /// 2. Changing the Session ID would never happen as this would breach Session fidelity.
            /// 3. Changing the Parameter Type is not going to happen. As this would already be referenced
            ///    in multiple places it would be changing a pre-existing and expected context.
            /// 4. The value never needs to be inserted as this is delayed and will be defined when the
            ///    transaction operation is run. - This isn't true, we do need the value to be updated in case we're referring to a MapParameter from an old transaction.
            if (!ValueUpdated && IsDelayed && !IsNew)
            {
                return;
            }

            SqlCommand parameterSqlCommand = null;

            try
            {
                bool addValue;

                if (IsNew && !ExistsInDb(connectionAbstraction))
                {
                    addValue = true;
                    parameterSqlCommand = new SqlCommand(InsertParameter, connectionAbstraction.Connection);
                }
                else if (IsDirty)
                {
                    string updateSql;

                    if (ValueUpdated && Value != Guid.Empty)
                    {
                        updateSql = string.Format(UpdateParameter, "[Value] = @Value, ");
                        addValue = true;
                    }
                    else
                    {
                        updateSql = string.Format(UpdateParameter, "");
                        addValue = false;
                    }

                    parameterSqlCommand = new SqlCommand(updateSql, connectionAbstraction.Connection);
                }
                else
                {
                    return;
                }

                parameterSqlCommand.Parameters.Add(IdSqlParameter);

                if (addValue)
                {
                    parameterSqlCommand.Parameters.Add(ValueSqlParameter);
                }

                parameterSqlCommand.Parameters.Add(SessionIdSqlParameter);
                parameterSqlCommand.Parameters.Add(IsDelayedSqlParameter);
                parameterSqlCommand.Parameters.Add(ParameterTypeSqlParameter);

                connectionAbstraction.Open();
                parameterSqlCommand.ExecuteNonQuery();

                ValueUpdated = false;

                connectionAbstraction.Close();
            }
            finally
            {
                if (parameterSqlCommand != null)
                {
                    parameterSqlCommand.Dispose();
                }
            }
        }

        public void LoadSessionObject(IDataRecord record)
        {
            Id = (Guid)record["ParameterUid"];
            Value = (Guid)record["Value"];
            SessionId = (Guid)record["SessionUid"];
            IsDelayed = (bool)record["IsDelayed"];
            ParameterType = (MapParameterType)record["ParameterType"];

            // As we are loading the object, we need to reset it so that it is aware when it becomes "dirty" or is actually a "new" object.
            IsNew = false;
            IsDirty = false;
            IsDbInstance = true;
        }
    }
}