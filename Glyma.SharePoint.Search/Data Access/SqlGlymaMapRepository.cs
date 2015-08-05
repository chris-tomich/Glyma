using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.BusinessData.Runtime;
using Microsoft.BusinessData.MetadataModel;
using Microsoft.BusinessData.MetadataModel.Collections;

namespace Glyma.SharePoint.Search
{
   public class SqlGlymaMapRepository : IGlymaMapRepository
   {
      private bool _disposed = false;
      private SqlDbConnectionSetting _connectionSetting;


      public SqlGlymaMapRepository(RepositoryConnectionSetting connectionSetting)
      {
         ConnectionSetting = new SqlDbConnectionSetting(connectionSetting);
      }


      public SqlGlymaMapRepository(string connectionString)
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
            _connectionSetting = (SqlDbConnectionSetting)value;
         }
      }


      public string GetDatabaseName()
      {
         return _connectionSetting.GetDatabaseName();
      }


      public List<DynamicType> GetRootMaps(string repositoryName, Guid domainId, ITypeDescriptorCollection entityFields)
      {
         List<DynamicType> nodes = new List<DynamicType>();

         using (SqlConnection dbConnection = new SqlConnection(_connectionSetting.GetConnectionString()))
         {
            using (SqlCommand dbCommand = new SqlCommand())
            {
               dbCommand.Connection = dbConnection;
               dbCommand.CommandType = CommandType.StoredProcedure; 
               dbCommand.CommandText = "dbo.GetRootMaps";

               // Get the root maps for all domains unless a domain is specified.
               if (!domainId.Equals(Guid.Empty))
               {
                  dbCommand.Parameters.Add("@DomainId", System.Data.SqlDbType.UniqueIdentifier).Value = @domainId;
               }

               dbConnection.Open();
               nodes = ReadNodes(dbCommand, repositoryName, entityFields);
            }
         }

         return nodes;
      }


      public List<DynamicType> GetNodesInMap(string repositoryName, Guid mapId, ITypeDescriptorCollection entityFields, bool mapsOnly)
      {
         List<DynamicType> nodes = new List<DynamicType>();

         if (!mapId.Equals(Guid.Empty))
         {
            using (SqlConnection dbConnection = new SqlConnection(_connectionSetting.GetConnectionString()))
            {
               using (SqlCommand dbCommand = new SqlCommand())
               {
                  dbCommand.Connection = dbConnection;
                  dbCommand.CommandType = CommandType.StoredProcedure;                  
                  dbCommand.CommandText = "dbo.GetNodesInMap";
                  dbCommand.Parameters.Add("@MapId", System.Data.SqlDbType.UniqueIdentifier).Value = mapId;
                  dbCommand.Parameters.Add("@MapsOnly", System.Data.SqlDbType.Bit).Value = mapsOnly;
                  dbConnection.Open();
                  nodes = ReadNodes(dbCommand, repositoryName, entityFields);
               }
            }
         }

         return nodes;
      }


      public DynamicType GetNode(string repositoryName, Guid nodeId, ITypeDescriptorCollection entityFields)
      {
         DynamicType node = null;

         using (SqlConnection dbConnection = new SqlConnection(_connectionSetting.GetConnectionString()))
         {
            using (SqlCommand dbCommand = new SqlCommand())
            {
               dbCommand.Connection = dbConnection;
               dbCommand.CommandText = "dbo.GetNodeInMapDetails";
               dbCommand.CommandType = CommandType.StoredProcedure;
               dbCommand.Parameters.Add("@NodeId", System.Data.SqlDbType.UniqueIdentifier).Value = nodeId;

               dbConnection.Open();
               List<DynamicType> nodes = ReadNodes(dbCommand, repositoryName, entityFields);
               if (nodes.Count > 0)
               {
                  node = nodes[0];
               }
            }
         }
         return node;
      }


      public List<GlymaNodeReference> GetParentNodes(Guid mapId, Guid nodeId)
      {
         List<GlymaNodeReference> parentNodes = new List<GlymaNodeReference>();

         using (SqlConnection dbConnection = new SqlConnection(_connectionSetting.GetConnectionString()))
         {
            using (SqlCommand dbCommand = new SqlCommand())
            {
               dbCommand.Connection = dbConnection;
               dbCommand.CommandText = "dbo.GetParentNodes";
               dbCommand.CommandType = CommandType.StoredProcedure;
               dbCommand.Parameters.Add("@MapId", System.Data.SqlDbType.UniqueIdentifier).Value = mapId;
               dbCommand.Parameters.Add("@NodeId", System.Data.SqlDbType.UniqueIdentifier).Value = nodeId;

               dbConnection.Open();
               using (SqlDataReader dbReader = dbCommand.ExecuteReader())
               {
                  while (dbReader.Read())
                  {
                     GlymaNodeReference parentNode = new GlymaNodeReference();
                     parentNode.Id = dbReader.GetValue<Guid>("ParentId");
                     parentNode.Name = dbReader.GetValue<string>("ParentName");
                     parentNode.NodeType = dbReader.GetValue<string>("ParentNodeType");
                     parentNode.MapId = mapId;
                     parentNode.DomainId = dbReader.GetValue<Guid>(GlymaEntityFields.DomainId);
                     parentNodes.Add(parentNode);
                  }
               }
            }
         }

         return parentNodes;
      }


      public List<GlymaNodeReference> GetChildNodes(Guid mapId, Guid nodeId)
      {
         List<GlymaNodeReference> childNodes = new List<GlymaNodeReference>();

         using (SqlConnection dbConnection = new SqlConnection(_connectionSetting.GetConnectionString()))
         {
            using (SqlCommand dbCommand = new SqlCommand())
            {
               dbCommand.Connection = dbConnection;
               dbCommand.CommandText = "dbo.GetChildNodes";
               dbCommand.CommandType = CommandType.StoredProcedure;
               dbCommand.Parameters.Add("@MapId", System.Data.SqlDbType.UniqueIdentifier).Value = mapId;
               dbCommand.Parameters.Add("@NodeId", System.Data.SqlDbType.UniqueIdentifier).Value = nodeId;

               dbConnection.Open();
               using (SqlDataReader dbReader = dbCommand.ExecuteReader())
               {
                  while (dbReader.Read())
                  {
                     GlymaNodeReference childNode = new GlymaNodeReference();
                     childNode.Id = dbReader.GetValue<Guid>("ChildId");
                     childNode.Name = dbReader.GetValue<string>("ChildName");
                     childNode.NodeType = dbReader.GetValue<string>("ChildNodeType");
                     childNode.MapId = mapId;
                     childNode.DomainId = dbReader.GetValue<Guid>(GlymaEntityFields.DomainId);
                     childNodes.Add(childNode);
                  }
               }
            }
         }

         return childNodes;
      }
   

      /// <summary>
      /// Read the Glyma nodes from a Glyma Repository. 
      /// </summary>
      /// <param name="dbReader">A dbCommand object containing the details required to retrieve nodes from a Glyma repository.  This must reference an open database connection.</param>
      /// <param name="repositoryName">The name of the repository the nodes were retrieved from.</param>
      /// <param name="entityFields">A ITypeDescriptorCollection object that contains the fields to retrieve for each node.</param>
      /// <returns>A collection of DynamicType objects containing details of the nodes retrieved from a Glyma repository.</returns>
      protected virtual List<DynamicType> ReadNodes(SqlCommand dbCommand, string repositoryName, ITypeDescriptorCollection entityFields)
      {
         List<DynamicType> nodes = new List<DynamicType>();
         DynamicType newNode = null;
         Guid currentNodeId = Guid.Empty;

         using (SqlDataReader dbReader = dbCommand.ExecuteReader())
         {
            while (dbReader.Read())
            {
               Guid readNodeId = dbReader.GetValue<Guid>(GlymaEntityFields.Id);
               if (!readNodeId.Equals(currentNodeId))
               {
                  currentNodeId = readNodeId;
                  newNode = new DynamicType();
                  newNode.Initialise(entityFields);
                  ReadBaseNodeProperties(newNode, dbReader, repositoryName);
                  nodes.Add(newNode);
               }
               ReadNodeProperty(newNode, dbReader);
            }
         }

         return nodes;
      }


      /// <summary>
      /// Reads the "base" properties of a Glyma node from a Glyma repository. 
      /// </summary>
      /// <param name="node">A DynamicType object representing the node to read the values into.</param>
      /// <param name="dbReader">A SqlDataReader object containing a reference to the node details retrieved from a Glyma repository.</param>
      /// <param name="repositoryName">The name of the repository the details were retrieved from.</param>
      protected virtual void ReadBaseNodeProperties(DynamicType node, SqlDataReader dbReader, string repositoryName)
      {
         Guid nodeId = dbReader.GetValue<Guid>(GlymaEntityFields.Id);
         node.SetValue(GlymaEntityFields.Id, nodeId);
         node.SetValue(GlymaEntityFields.RepositoryName, repositoryName);
         node.SetValue(GlymaEntityFields.DomainId, dbReader.GetValue<Guid>(GlymaEntityFields.DomainId));
         node.SetValue(GlymaEntityFields.DomainName, dbReader.GetValue<string>(GlymaEntityFields.DomainName));
         Guid rootMapId = dbReader.GetValue<Guid>(GlymaEntityFields.RootMapId);
         node.SetValue(GlymaEntityFields.RootMapId, rootMapId);
         if (nodeId.Equals(rootMapId))
         {
            // Special case where the node is a root map of a domain and therefore doesn't have a containing map.
            node.SetValue(GlymaEntityFields.MapId, Guid.Empty);
            node.SetValue(GlymaEntityFields.MapName, string.Empty);
         }
         else
         {
            node.SetValue(GlymaEntityFields.MapId, dbReader.GetValue<Guid>(GlymaEntityFields.MapId));
            node.SetValue(GlymaEntityFields.MapName, dbReader.GetValue<string>(GlymaEntityFields.MapName));
         }
         node.SetValue(GlymaEntityFields.NodeType, dbReader.GetValue<string>(GlymaEntityFields.NodeType));
      }


      /// <summary>
      /// Reads a metadata value of a Glyma node from a Glyma repository.
      /// </summary>
      /// <param name="node">A DynamicType object representing the node to read the values into.</param>
      /// <param name="dbReader">A SqlDataReader object containing a reference to the node metadata retrieved from a Glyma repository.</param>
      protected virtual void ReadNodeProperty(DynamicType node, SqlDataReader dbReader)
      {
         string fieldName = dbReader.GetValue<string>("FieldName");
         string fieldValue = dbReader.GetValue<string>("FieldValue");
         node.SetValue(fieldName, fieldValue);
      }


      protected virtual void Dispose(bool disposing)
      {
         _disposed = true;
      }


      public void Dispose()
      {
         Dispose(true);
         GC.SuppressFinalize(this);
      }


      ~SqlGlymaMapRepository()
      {
         Dispose(false);
      }

   }
}
