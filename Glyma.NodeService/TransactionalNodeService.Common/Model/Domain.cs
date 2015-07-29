using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.Data;
using System.Data.SqlClient;

namespace TransactionalNodeService.Common.Model
{
    public class Domain : IPersistableSessionObject
    {
        public Domain()
        {
            IsNew = false;
        }

        public Domain(bool isNew)
        {
            IsNew = isNew;
        }

        public bool IsNew
        {
            get;
            private set;
        }

        public bool IsDirty
        {
            get;
            private set;
        }

        [DataMember]
        public Guid DomainUid
        {
            get;
            set;
        }

        [DataMember]
        public string DomainOriginalId
        {
            get;
            set;
        }

        public void LoadSessionObject(IDataRecord record)
        {
            throw new NotImplementedException();
        }

        public void PersistSessionObject(IDbConnectionAbstraction connectionAbstraction)
        {
            if (IsNew)
            {
                SqlCommand createDomainCommand = CreateDomainCommand(DomainUid, DomainOriginalId);
                createDomainCommand.Connection = connectionAbstraction.Connection;

                connectionAbstraction.Open();
                createDomainCommand.ExecuteNonQuery();
                connectionAbstraction.Close();
            }
        }

        protected SqlCommand CreateDomainCommand(Guid domainId, string domainOriginalId)
        {
            SqlCommand createDomainCommand = new SqlCommand();
            createDomainCommand.CommandText = "INSERT INTO [Domains] ([DomainUid], [DomainOriginalId]) VALUES (@DomainUid, @DomainOriginalId);";

            createDomainCommand.Parameters.AddWithValue("@DomainUid", domainId);

            if (string.IsNullOrEmpty(domainOriginalId))
            {
                createDomainCommand.Parameters.AddWithValue("@DomainOriginalId", DBNull.Value);
            }
            else
            {
                createDomainCommand.Parameters.AddWithValue("@DomainOriginalId", domainOriginalId);
            }

            return createDomainCommand;
        }
    }
}