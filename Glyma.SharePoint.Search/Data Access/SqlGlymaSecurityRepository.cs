using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Glyma.SharePoint.Search
{
   public class SqlGlymaSecurityRepository : IGlymaSecurityRepository
   {
      private const string _sharePointSiteCollectionIdColumnName = "SiteSPId";
      private const string _sharePointGroupIdColumnName = "GroupSPId";

      private bool _disposed = false;
      private SqlDbConnectionSetting _connectionSetting;


      public SqlGlymaSecurityRepository(RepositoryConnectionSetting connectionSetting)
      {
         ConnectionSetting = new SqlDbConnectionSetting(connectionSetting);
      }


      public SqlGlymaSecurityRepository(string connectionString)
      {
         ConnectionSetting = new SqlDbConnectionSetting(connectionString);
      }


      public RepositoryConnectionSetting ConnectionSetting
      {
         get
         {
            return _connectionSetting;
         }

         set
         {
            _connectionSetting = (SqlDbConnectionSetting) value;
         }
      }


      public List<SpFarmGroupIdentifier> GetAllowedGroups(string repositoryName, Guid domainId, Guid rootMapId)
      {
         List<SpFarmGroupIdentifier> allowedGroups = new List<SpFarmGroupIdentifier>();

         using (SqlConnection dbConnection = new SqlConnection(_connectionSetting.GetConnectionString()))
         {
            using (SqlCommand dbCommand = new SqlCommand())
            {
               dbCommand.Connection = dbConnection;
               dbCommand.CommandText = "dbo.GetAllowedGroups";
               dbCommand.CommandType = CommandType.StoredProcedure;
               dbCommand.Parameters.Add("@DomainId", System.Data.SqlDbType.UniqueIdentifier).Value = domainId;
               dbCommand.Parameters.Add("@MapId", System.Data.SqlDbType.UniqueIdentifier).Value = rootMapId;

               dbConnection.Open();
               using (SqlDataReader dbReader = dbCommand.ExecuteReader())
               {
                  while (dbReader.Read())
                  {
                     SpFarmGroupIdentifier newGroupId = new SpFarmGroupIdentifier();
                     newGroupId.SiteCollectionId = dbReader.GetValue<Guid>(_sharePointSiteCollectionIdColumnName);
                     newGroupId.GroupId = dbReader.GetValue<int>(_sharePointGroupIdColumnName);
                     if (!allowedGroups.Contains(newGroupId))
                     {
                        allowedGroups.Add(newGroupId);
                     }
                  }
               }
            }
         }

         return allowedGroups;
      }


      public List<Guid> GetSiteCollectionAssociations(string databaseName)
      {
         List<Guid> siteCollectionIds = new List<Guid>();

         using (SqlConnection dbConnection = new SqlConnection(_connectionSetting.GetConnectionString()))
         {
            using (SqlCommand dbCommand = new SqlCommand())
            {
               dbCommand.Connection = dbConnection;
               dbCommand.CommandText = "dbo.GetSiteCollectionAssociations";
               dbCommand.CommandType = CommandType.StoredProcedure;
               dbCommand.Parameters.Add("@DatabaseName", System.Data.SqlDbType.NVarChar).Value = databaseName;

               dbConnection.Open();
               using (SqlDataReader dbReader = dbCommand.ExecuteReader())
               {
                  while (dbReader.Read())
                  {
                     Guid siteCollectionId = dbReader.GetValue<Guid>(_sharePointSiteCollectionIdColumnName);
                     if (!siteCollectionIds.Contains(siteCollectionId))
                     {
                        siteCollectionIds.Add(siteCollectionId);
                     }
                  }
               }
            }
         }

         return siteCollectionIds;
      }


      public void Dispose()
      {
         Dispose(true);
         GC.SuppressFinalize(this);
      }


      ~SqlGlymaSecurityRepository()
      {
         Dispose(false);
      }


      protected virtual void Dispose(bool disposing)
      {
         _disposed = true;
      }

   }
}
