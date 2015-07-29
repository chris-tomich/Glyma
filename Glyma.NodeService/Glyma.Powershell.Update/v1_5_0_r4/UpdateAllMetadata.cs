using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Glyma.Powershell.Update.v1_5_0_r4.Model;

namespace Glyma.Powershell.Update.v1_5_0_r4
{
    public class UpdateAllMetadata
    {
        private const string Sql = @"
Update Metadata Set MetadataName = 'Description.Type' where MetadataName = 'DescriptionType';

Update Metadata Set MetadataName = 'Description.Content' where MetadataName = 'Description';
";

        public UpdateAllMetadata()
        {
            
        }

        public void Execute(SqlConnection connection, SqlTransaction transaction)
        {
            var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = Sql;
            command.ExecuteNonQuery();
        }
    }
}
