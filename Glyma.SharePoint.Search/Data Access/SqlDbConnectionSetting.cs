using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Glyma.SharePoint.Search
{
   public class SqlDbConnectionSetting : RepositoryConnectionSetting
   {

      public SqlDbConnectionSetting(string connectionString)
      {
         this.Add(ConnectionSettingProperty.ConnectionString, connectionString);
         Validate();
      }


      public SqlDbConnectionSetting(RepositoryConnectionSetting connection)
      {
         string connectionString = string.Empty;

         if (connection.ContainsKey(ConnectionSettingProperty.ConnectionString))
         {
            connectionString = (string)connection[ConnectionSettingProperty.ConnectionString];
         }
         this.Add(ConnectionSettingProperty.ConnectionString, connectionString);
         Validate();
      }


      public string GetConnectionString()
      {
         string connectionString = string.Empty;
         if (this.ContainsKey(ConnectionSettingProperty.ConnectionString))
         {
            connectionString = (string)this[ConnectionSettingProperty.ConnectionString];
         }
         return connectionString;
      }


      public string GetDatabaseName()
      {
         SqlConnectionStringBuilder connectionString = new SqlConnectionStringBuilder(GetConnectionString());
         return connectionString.InitialCatalog;
      }


      public override bool IsValid()
      {
         return !string.IsNullOrEmpty(GetConnectionString());
      }


      public override void Validate()
      {
         if (!IsValid())
         {
            throw new ApplicationException("A valid database connection string has not been provided.");
         }
      }
   }
}
