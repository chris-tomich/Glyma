﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Script.Serialization;
using Microsoft.BusinessData.Runtime;

namespace Glyma.SharePoint.Search
{
   /// <summary>
   /// Get the identifying details of the immediate parents of a Glyma node.
   /// </summary>
   /// <remarks>
   /// The details are returned as a string containing JSON.  The details are not returned as a collection because when the field of a BCS model entity for a custom indexing connector is defined as a collection, 
   /// each entry in the collection results in the creation of a new crawled property in the Search schema.
   /// </remarks>
   public class IncludeParentsTransform : INodeCrawlRule
   {

      public bool Apply(DynamicType glymaNode, IGlymaMapRepository mapRepository)
      {
         bool includeNode = true;
         List<GlymaNodeReference> parentNodes = new List<GlymaNodeReference>();
         string parentNodesJson = string.Empty;

         if (glymaNode == null)
         {
            throw new ArgumentNullException("glymaNode");
         }

         Guid mapId = (Guid)glymaNode[GlymaEntityFields.MapId];
         Guid nodeId = (Guid)glymaNode[GlymaEntityFields.Id];

         if (nodeId.Equals(Guid.Empty))
         {
            throw new ArgumentException("The node ID is invalid because it has an undefined value.");
         }

         parentNodes = mapRepository.GetParentNodes(mapId, nodeId);
         if (parentNodes.Count > 0)
         {
            parentNodes.Sort(new NodeComparer());
            JavaScriptSerializer serialiser = new JavaScriptSerializer();
            parentNodesJson = serialiser.Serialize(parentNodes);
         }
         glymaNode[GlymaEntityFields.ParentNodes] = parentNodesJson;

         return includeNode;
      }


      public INodeCrawlRule DeepCopy()
      {
         return new IncludeParentsTransform();
      }
   }
}
