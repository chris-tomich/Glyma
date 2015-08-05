using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Script.Serialization;
using Microsoft.BusinessData.Runtime;

namespace Glyma.SharePoint.Search
{
   /// <summary>
   /// Get the identifying details of the immediate children of a Glyma node.
   /// </summary>
   /// <remarks>
   /// The details are returned as a string containing JSON.  The details are not returned as a collection because when the field of a BCS model entity for a custom indexing connector is defined as a collection, 
   /// each entry in the collection results in the creation of a new crawled property in the Search schema.
   /// </remarks>
   public class IncludeChildrenTransform : INodeCrawlRule
   {

      public bool Apply(DynamicType glymaNode, IGlymaMapRepository mapRepository)
      {
         bool includeNode = true;
         List<GlymaNodeReference> childNodes = new List<GlymaNodeReference>();
         string childNodesJson = string.Empty;

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

         childNodes = mapRepository.GetChildNodes(mapId, nodeId);
         if (childNodes.Count > 0)
         {
            childNodes.Sort(new NodeComparer());
            JavaScriptSerializer serialiser = new JavaScriptSerializer();
            childNodesJson = serialiser.Serialize(childNodes);
         }
         glymaNode[GlymaEntityFields.ChildNodes] = childNodesJson;

         return includeNode;
      }


      public INodeCrawlRule DeepCopy()
      {
         return new IncludeChildrenTransform();
      }
   }
}
