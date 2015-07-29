using System;
using System.Data.SqlClient;

namespace Glyma.Powershell.Update.v1_5_0_r4
{
    public class UpdateMetadata
    {
        private readonly Guid _metadataId;
        private readonly string _name;
        private readonly string _value;

        private const string Sql = @"Update Metadata Set MetadataName = @MetadataName, MetadataValue = @MetadataValue Where MetadataId = @MetadataId;";

        public UpdateMetadata(Guid metadataId, string name, string value)
        {
            _metadataId = metadataId;
            _name = name;
            _value = value;
        }

        public void Execute(SqlConnection connection, SqlTransaction transaction)
        {
            var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = Sql;
            command.Parameters.Add(new SqlParameter("@MetadataName", _name));
            command.Parameters.Add(new SqlParameter("@MetadataValue", _value));
            command.Parameters.Add(new SqlParameter("@MetadataId", _metadataId.ToString()));
            command.ExecuteNonQuery();
        }
    }
}
