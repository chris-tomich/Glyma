using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.BusinessData.Runtime;

namespace Glyma.SharePoint.Search
{
   /// <summary>
   /// Filters out a Glyma node if it is identified as a "simple" question.
   /// </summary>
   public class SimpleQuestionFilter : INodeCrawlRule
   {

      public bool Apply(DynamicType glymaNode, IGlymaMapRepository mapRepository)
      {
         bool includeNode = true;

         if (glymaNode == null)
         {
            throw new ArgumentNullException("glymaNode");
         }

         string nodeType = (string)glymaNode[GlymaEntityFields.NodeType];
         if (nodeType.Equals(GlymaNodeTypes.Question, StringComparison.OrdinalIgnoreCase))
         {
            string nodeName = (string)glymaNode[GlymaEntityFields.Name];
            if (!string.IsNullOrEmpty(nodeName))
            {
               // Apply default logic that excludes nodes that are less than two "words".  The number of words is determined by the number of spaces.
               int wordCount = nodeName.Count(f => f == ' ') + 1;
               if (wordCount <= 2)
               {
                  includeNode = false;
               }
            }
            else
            {
               includeNode = false;
            }
         }

         return includeNode;
      }


      public INodeCrawlRule DeepCopy()
      {
         return new SimpleQuestionFilter();
      }
   }
}
