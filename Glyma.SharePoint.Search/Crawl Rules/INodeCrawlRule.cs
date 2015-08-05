using System;
using System.Data.SqlClient;
using Microsoft.BusinessData.Runtime;

namespace Glyma.SharePoint.Search
{
   public interface INodeCrawlRule
   {
      bool Apply(DynamicType node, IGlymaMapRepository mapRepository);
      INodeCrawlRule DeepCopy();
   }
}
