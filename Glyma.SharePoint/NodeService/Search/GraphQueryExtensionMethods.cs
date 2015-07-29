using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NodeService
{
    public static class GraphQueryExtensionMethods
    {
        public static IEnumerable<Node> ApplyFilters(this GraphQueryFilter[] filters, MappingToolDatabaseDataContext dbContext, IEnumerable<Node> nodes)
        {
            IEnumerable<Node> filteredNodes = new List<Node>();

            if (filters != null)
            {
                foreach (GraphQueryFilter filter in filters)
                {
                    filteredNodes = filteredNodes.Union(filter.ApplySearchConditions(dbContext, nodes));
                }
            }
            else
            {
                filteredNodes = nodes; //no filters so send back unfiltered
            }
            return filteredNodes;
        }
    }
}