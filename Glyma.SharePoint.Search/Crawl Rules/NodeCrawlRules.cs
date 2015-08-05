using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.BusinessData.Runtime;

namespace Glyma.SharePoint.Search
{
   /// <summary>
   /// Represents a collection of objects implementing the INodeCrawlRule interface.
   /// </summary>
   public class NodeCrawlRules : List<INodeCrawlRule>
   {

      /// <summary>
      /// Conditionally execute all the crawl rules on a Glyma node.
      /// </summary>
      /// <param name="glymaNode">The Glyma node to apply the crawl rules to.</param>
      /// <param name="dbConnection">The database connection to a Glyma repository that can be used to execute the crawl rules.</param>
      /// <returns>Returns true if all the crawl rules were successfully applied to the Glyma node; otherwise, false.</returns>
      /// <remarks>
      /// Execution of the crawl rules is stopped if any of the crawl rules return false.
      /// </remarks>
      public bool Apply(DynamicType glymaNode, IGlymaMapRepository mapRepository)
      {
         bool includeNode = true;

         foreach (INodeCrawlRule crawlRule in this)
         {
            includeNode = crawlRule.Apply(glymaNode, mapRepository);
            if (!includeNode)
            {
               break;
            }
         }

         return includeNode;
      }


      public NodeCrawlRules DeepCopy()
      {
         NodeCrawlRules copiedCrawlRules = new NodeCrawlRules();

         foreach (INodeCrawlRule crawlRule in this)
         {
            copiedCrawlRules.Add(crawlRule.DeepCopy());                  
         }

         return copiedCrawlRules;
      }
   }
}
