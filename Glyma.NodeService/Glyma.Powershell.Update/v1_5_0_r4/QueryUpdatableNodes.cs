using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Glyma.Powershell.Update.v1_5_0_r4.Model;

namespace Glyma.Powershell.Update.v1_5_0_r4
{
    public class QueryUpdatableNodes
    {
        private const string Sql = @"
Update Metadata Set MetadataName = 'Description.Type' where MetadataName = 'DescriptionType';

Update Metadata Set MetadataName = 'Description.Content' where MetadataName = 'Description';

SELECT m1.NodeUid, m1.MetadataId as DescriptionTypeMetadataId, 
m1.MetadataValue as DescriptionType, 
m2.MetadataId as DescriptionMetadataId, 
m2.MetadataValue as Description,
m2.DomainUid as DomainUid,
m1.RootMapUid as RootMapUid,
m1.Created as DescriptionCreated,
m2.Modified as DescriptionModified,
m2.CreatedBy as DescriptionCreatedBy,
m2.ModifiedBy as DescriptionModifiedBy 
  FROM [Metadata] as m1 left outer join [Metadata] as m2 on m1.[NodeUid] = m2.[NodeUid] and m2.[MetadataName] = 'Description.Content' 
  where m1.[MetadataName] = 'Description.Type';";

        public QueryUpdatableNodes()
        {
            
        }

        public List<UpdatableNode> GetItems(SqlConnection connection, SqlTransaction transaction)
        {
            var output = new List<UpdatableNode>();
            var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = Sql;
            using (var dr = command.ExecuteReader())
            {
                while (dr.Read())
                {
                    var item = new UpdatableNode();
                    item.NodeUid = new Guid(dr["NodeUid"].ToString());
                    if (dr["DescriptionMetadataId"] != DBNull.Value)
                    {
                        item.DescriptionMetadataId = new Guid(dr["DescriptionMetadataId"].ToString());
                        item.Description = dr["Description"].ToString();
                        item.DescriptionCreated = (DateTime)dr["DescriptionCreated"];
                        item.DescriptionModified = (DateTime)dr["DescriptionModified"];
                        item.DescriptionModifiedBy = dr["DescriptionModifiedBy"].ToString();
                        item.DescriptionCreatedBy = dr["DescriptionCreatedBy"] == DBNull.Value ? item.DescriptionModifiedBy : dr["DescriptionCreatedBy"].ToString();
                        item.HasDescription = true;
                    }
                    
                    item.DescriptionType = dr["DescriptionType"].ToString();
                    item.DescriptionTypeMetadataId = new Guid(dr["DescriptionTypeMetadataId"].ToString());
                    item.RootMapUid = new Guid(dr["RootMapUid"].ToString());
                    item.DomainUid = new Guid(dr["DomainUid"].ToString());
                    
                }
            }
            return output;
        }
    }
}
