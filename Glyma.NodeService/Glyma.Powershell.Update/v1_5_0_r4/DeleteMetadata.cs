using System;
using System.Data.SqlClient;
using Glyma.Powershell.Update.v1_5_0_r4.Model;

namespace Glyma.Powershell.Update.v1_5_0_r4
{
    public class DeleteMetadata
    {
        private readonly Guid _metadataId;

        private const string Sql = @"Delete From Metadata Where MetadataId = @MetadataId";

        public DeleteMetadata(Guid metadataId)
        {
            _metadataId = metadataId;
        }

        public void Execute(SqlConnection connection, SqlTransaction transaction)
        {
            var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = Sql;
            command.Parameters.Add(new SqlParameter("@MetadataId", _metadataId.ToString()));
            command.ExecuteNonQuery();
        }
    }
}
