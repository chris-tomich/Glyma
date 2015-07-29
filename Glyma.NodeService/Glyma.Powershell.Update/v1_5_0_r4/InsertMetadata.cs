using System;
using System.Data.SqlClient;
using Glyma.Powershell.Update.v1_5_0_r4.Model;

namespace Glyma.Powershell.Update.v1_5_0_r4
{
    public class InsertMetadata
    {
        private readonly string _name;
        private readonly string _value;
        private readonly Guid? _nodeUid;
        private readonly Guid? _rootMapUid;
        private readonly Guid? _domainUid;
        private readonly DateTime? _created;
        private readonly DateTime? _modified;
        private readonly string _createdBy;
        private readonly string _modifiedBy;

        private const string Sql = @"INSERT INTO [Metadata]
           ([MetadataId]
           ,[MetadataTypeUid]
           ,[NodeUid]
           ,[RelationshipUid]
           ,[DescriptorTypeUid]
           ,[MetadataName]
           ,[MetadataValue]
           ,[RootMapUid]
           ,[DomainUid]
           ,[Created]
           ,[Modified]
           ,[CreatedBy]
           ,[ModifiedBy])
     VALUES
           (@MetadataId
           ,@MetadataTypeUid
           ,@NodeUid
           ,NULL
           ,NULL
           ,@MetadataName
           ,@MetadataValue
           ,@RootMapUid
           ,@DomainUid
           ,@Created
           ,@Modified
           ,@CreatedBy
           ,@ModifiedBy);";

        public InsertMetadata(string name, string value, Guid? nodeUid, Guid? rootMapUid, Guid? domainUid, DateTime? created, DateTime? modified, string createdBy, string modifiedBy)
        {
            _name = name;
            _value = value;
            _nodeUid = nodeUid;
            _rootMapUid = rootMapUid;
            _domainUid = domainUid;
            _created = created;
            _modified = modified;
            _createdBy = createdBy;
            _modifiedBy = modifiedBy;
        }

        public void Execute(SqlConnection connection, SqlTransaction transaction)
        {
            var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = Sql;
            command.Parameters.Add(new SqlParameter("@MetadataId", Guid.NewGuid().ToString()));
            command.Parameters.Add(new SqlParameter("@MetadataTypeUid", "C7628C1E-77C1-4A07-A2E8-8DE9F4E9803C"));
            command.Parameters.Add(_nodeUid.HasValue ? new SqlParameter("@NodeUid", _nodeUid.Value) : new SqlParameter("@NodeUid", DBNull.Value));
            command.Parameters.Add(new SqlParameter("@MetadataName", _name));
            command.Parameters.Add(new SqlParameter("@MetadataValue", _value));
            command.Parameters.Add(_rootMapUid.HasValue ? new SqlParameter("@RootMapUid", _rootMapUid.Value) : new SqlParameter("@RootMapUid", DBNull.Value));
            command.Parameters.Add(_domainUid.HasValue ? new SqlParameter("@DomainUid", _domainUid.Value) : new SqlParameter("@DomainUid", DBNull.Value));
            command.Parameters.Add(_created.HasValue ? new SqlParameter("@Created", _created.Value) : new SqlParameter("@Created", DBNull.Value));
            command.Parameters.Add(_modified.HasValue ? new SqlParameter("@Modified", _modified.Value) : new SqlParameter("@Modified", DBNull.Value));
            command.Parameters.Add(_createdBy != null ? new SqlParameter("@CreatedBy", _createdBy) : new SqlParameter("@CreatedBy", DBNull.Value));
            command.Parameters.Add(_modifiedBy != null ? new SqlParameter("@ModifiedBy", _modifiedBy) : new SqlParameter("@ModifiedBy", DBNull.Value));
            command.ExecuteNonQuery();
        }
    }
}
