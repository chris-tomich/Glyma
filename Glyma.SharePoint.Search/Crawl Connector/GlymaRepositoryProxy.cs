using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using Microsoft.BusinessData.Runtime;
using Microsoft.BusinessData.MetadataModel;
using Microsoft.BusinessData.MetadataModel.Collections;
using Microsoft.SharePoint.Administration;

namespace Glyma.SharePoint.Search
{
   /// <summary>
   /// Provides the operations required to query a Glyma repository.
   /// </summary>
   public class GlymaRepositoryProxy : IDisposable
   {
      public IGlymaMapRepository MapRepository { get; set; }
      public IGlymaSecurityRepository SecurityRepository { get; set; }
      public GlymaSecurityManager SecurityManager { get; set; }
      public bool IsSecurityEnabled { get; set; }
      public NodeCrawlRules CrawlRules { get; set; }
      public string StartAddress { get; set; }
      public string RepositoryName { get; set; }


      /// <summary>
      /// Gets the root map nodes from a Glyma repository.
      /// </summary>
      /// <param name="entityFields">A ITypeDescriptorCollection object that contains the fields to retrieve for each node.</param>
      /// <returns>A collection of DynamicType objects containing details of the nodes retrieved from a Glyma repository.</returns>
      /// <remarks>This is intended to be used as a Finder method in a BCS model.</remarks>
      public List<DynamicType> GetRooMaps(ITypeDescriptorCollection entityFields)
      {
         GlymaSearchLogger.WriteTrace(LogCategoryId.Connector, TraceSeverity.Verbose, "GetRootMaps");

         Uri startUrl = new Uri(StartAddress);
         Guid domainId = Guid.Empty;

         if (startUrl.Segments.Length == 2)
         {
            domainId = new Guid(startUrl.Segments[1].TrimEnd('/'));
         }

         List<DynamicType> nodes = MapRepository.GetRootMaps(RepositoryName, domainId, entityFields);
         nodes.RemoveAll(node => !CrawlRules.Apply(node, MapRepository));

         return nodes;
      }


      /// <summary>
      /// Gets the map nodes contained in a map from a Glyma repository.
      /// </summary>
      /// <param name="repositoryName">The name of the Glyma repository.</param>
      /// <param name="domainId">The GUID of the map node's domain.</param>
      /// <param name="rootMapId">The GUID of the node's root map.</param>
      /// <param name="mapId">The GUID of the map node's parent map.</param>
      /// <param name="nodeId">The GUID of the map node to retrieve nodes from.</param>
      /// <param name="lastCrawlStartTime">Specifies the last crawl time.</param>
      /// <param name="entityFields">A ITypeDescriptorCollection object that contains the fields to retrieve for each node.</param>
      /// <returns>A collection of DynamicType objects containing details of the nodes retrieved from a Glyma repository.</returns>
      /// <remarks>This is intended to be used as an AssociationNavigator method for a map entity in a BCS model.</remarks>
      public List<DynamicType> GetMapNodesInMap(string repositoryName, Guid domainId, Guid rootMapId, Guid mapId, Guid nodeId, DateTime lastCrawlStartTime, ITypeDescriptorCollection entityFields)
      {
         GlymaSearchLogger.WriteTrace(LogCategoryId.Connector, TraceSeverity.Verbose, "GetMapNodesInMap(" + repositoryName + ", " + domainId + ", " + rootMapId + ", " + mapId + ", " + nodeId + ")");

         List<DynamicType> nodes = MapRepository.GetNodesInMap(repositoryName, nodeId, entityFields, true);
         nodes.RemoveAll(node => !CrawlRules.Apply(node, MapRepository));

         return nodes;
      }


      /// <summary>
      /// Gets the non-map nodes contained in a map from a Glyma repository.
      /// </summary>
      /// <param name="repositoryName">The name of the Glyma repository.</param>
      /// <param name="domainId">The GUID of the map node's domain.</param>
      /// <param name="rootMapId">The GUID of the node's root map.</param>
      /// <param name="mapId">The GUID of the map node's parent map.</param>
      /// <param name="nodeId">The GUID of the map node to retrieve nodes from.</param>
      /// <param name="lastCrawlStartTime">Specifies the last crawl time.</param>
      /// <param name="entityFields">A ITypeDescriptorCollection object that contains the fields to retrieve for each node.</param>
      /// <returns>A collection of DynamicType objects containing details of the nodes retrieved from a Glyma repository.</returns>
      /// <remarks>This is intended to be used as an AssociationNavigator method for a node entity in a BCS model.</remarks>
      public List<DynamicType> GetNodesInMap(string repositoryName, Guid domainId, Guid rootMapId, Guid mapId, Guid nodeId, DateTime lastCrawlStartTime, ITypeDescriptorCollection entityFields)
      {
         GlymaSearchLogger.WriteTrace(LogCategoryId.Connector, TraceSeverity.Verbose, "GetNodesInMap(" + repositoryName + ", " + domainId + ", " + rootMapId + ", " + mapId + ", " + nodeId + ")");

         List<DynamicType> nodes = MapRepository.GetNodesInMap(repositoryName, nodeId, entityFields, false);
         nodes.RemoveAll(node => !CrawlRules.Apply(node, MapRepository));

         return nodes;
      }


      /// <summary>
      /// Get the details of a Glyma node.
      /// </summary>
      /// <param name="repositoryName">The name of the Glyma repository.</param>
      /// <param name="domainId">The GUID of the node's domain.</param>
      /// <param name="rootMapId">The GUID of the node's root map.</param>
      /// <param name="mapId">The GUID of the node's parent map.</param>
      /// <param name="nodeId">The GUID of the node to retrieve details from.</param>
      /// <param name="entityFields">A ITypeDescriptorCollection object that contains the fields to retrieve for each node.</param>
      /// <returns>A DynamicType object containing the details for the node.</returns>
      public DynamicType GetNode(string repositoryName, Guid domainId, Guid rootMapId, Guid mapId, Guid nodeId, ITypeDescriptorCollection entityFields)
      {
         GlymaSearchLogger.WriteTrace(LogCategoryId.Connector, TraceSeverity.Verbose, "GetNode(" + repositoryName + ", " + domainId + ", " + rootMapId + ", " + mapId + ", " + nodeId + ")");

         DynamicType node = null;

         node = MapRepository.GetNode(repositoryName, nodeId, entityFields);
         if (node != null)
         {
            CrawlRules.Apply(node, MapRepository);
         }

         return node;
      }


      /// <summary>
      /// Get the access control list (ACL) for a node.
      /// </summary>
      /// <param name="repositoryName">The name of the Glyma repository.</param>
      /// <param name="domainId">The GUID of the node's domain.</param>
      /// <param name="rootMapId">The GUID of the node's root map.</param>
      /// <param name="mapId">The GUID of the node's parent map.</param>
      /// <param name="nodeId">The GUID of the node to retrieve details from.</param>
      /// <param name="entityFields">A ITypeDescriptorCollection object that contains the fields to retrieve for each node.</param>
      /// <returns></returns>
      public byte[] GetNodeAcl(string repositoryName, Guid domainId, Guid rootMapId, Guid mapId, Guid nodeId, ITypeDescriptorCollection entityFields)
      {
         GlymaSearchLogger.WriteTrace(LogCategoryId.Connector, TraceSeverity.Verbose, "GetNodeAcl(" + repositoryName + ", " + domainId + ", " + rootMapId + ", " + mapId + ", " + nodeId + ")");

         byte[] nodeAcl = null;

         if (IsSecurityEnabled)
         {
            nodeAcl = SecurityManager.GetNodeAcl(MapRepository.GetDatabaseName(), repositoryName, domainId, rootMapId);
         }
         else
         {
            nodeAcl = SecurityManager.GetAllowAllNodeAcl(repositoryName, domainId, rootMapId);
         }

         return nodeAcl;
      }


      public void Dispose()
      {
      }

   }
}
