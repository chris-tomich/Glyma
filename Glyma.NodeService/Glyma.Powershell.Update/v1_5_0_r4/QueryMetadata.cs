using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Glyma.Powershell.Update.v1_5_0_r4.Model;

namespace Glyma.Powershell.Update.v1_5_0_r4
{
    public class QueryMetadata
    {
        private readonly string _metadataName;
        private const string Sql = @"SELECT * FROM [Metadata] where [MetadataTypeUid] = 'C7628C1E-77C1-4A07-A2E8-8DE9F4E9803C' and RelationshipUid is NULL and DescriptorTypeUid is NULL and [MetadataName] = @MetadataName";

        public QueryMetadata(string metadataName)
        {
            _metadataName = metadataName;
        }

        public List<MappingTool.Metadata> GetItems(SqlConnection connection, SqlTransaction transaction)
        {
            var output = new List<MappingTool.Metadata>();
            var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = Sql;
            command.Parameters.Add(new SqlParameter("@MetadataName", _metadataName));
            using (var dr = command.ExecuteReader())
            {
                while (dr.Read())
                {
                    var item = new MappingTool.Metadata();
                    item.MetadataId = new Guid(dr["MetadataId"].ToString());
                    item.MetadataTypeUid = new Guid(dr["MetadataTypeUid"].ToString());
                    if (dr["NodeUid"] != DBNull.Value)
                    {
                        item.NodeUid = new Guid(dr["NodeUid"].ToString());
                    }
                    
                    item.MetadataName = dr["MetadataName"].ToString();
                    item.MetadataValue = dr["MetadataValue"].ToString();

                    if (dr["RootMapUid"] != DBNull.Value)
                    {
                        item.RootMapUid = new Guid(dr["RootMapUid"].ToString());
                    }
                    if (dr["DomainUid"] != DBNull.Value)
                    {
                        item.DomainUid = new Guid(dr["DomainUid"].ToString());
                    }

                    item.Modified = dr["Modified"] == DBNull.Value ? null : (DateTime?)dr["Modified"];
                    item.ModifiedBy = dr["ModifiedBy"] == DBNull.Value? null : dr["ModifiedBy"].ToString();
                    item.Created = dr["Created"] == DBNull.Value ? null : (DateTime?)dr["Created"];
                    item.CreatedBy = dr["CreatedBy"] == DBNull.Value? item.ModifiedBy: dr["CreatedBy"].ToString();
                    output.Add(item);
                }
            }
            return output;
        }
    }
}
