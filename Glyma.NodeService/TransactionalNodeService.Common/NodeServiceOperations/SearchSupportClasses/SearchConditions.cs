using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.Data.SqlClient;
using System.Text;

namespace TransactionalNodeService.Common
{
    public enum SortOrderOptions
    {
        CreatedAscending,
        ModifiedAscending,
        CreatedDescending,
        ModifiedDescending
    }

    public enum SearchOperation
    {
        AND,
        OR
    }

    [DataContract]
    public class SearchConditions
    {
        private const string SearchQueryCount = @"SELECT COUNT(DISTINCT [NodeUid]) FROM [Metadata] WHERE {0} [Metadata].[RootMapUid] = @RootMapUid";
        private const string SearchQuery = @"SELECT TOP {0} [NodeUid], [Modified] FROM (SELECT [NodeUid], MAX([Modified]) AS Modified FROM [Metadata] WHERE {3} [Metadata].[RootMapUid] = @RootMapUid GROUP BY [NodeUid]) Grouped ORDER BY {1} {2}, [NodeUid] ASC";
        private const string PaginationSearchQuery = @"SELECT [NodeUid], [Modified] FROM (SELECT TOP {0} * FROM ({1}) FirstLevelPage ORDER BY {2} {3}, [NodeUid] DESC) SecondLevelPageOrdered ORDER BY {2} {4}, [NodeUid] ASC";

        private const string SearchANDQueryCount = @"SELECT COUNT([NodeUid]) FROM (SELECT [NodeUid], MAX([Modified]) AS Modified, COUNT([NodeUid]) AS ResultCount FROM (SELECT [NodeUid], MAX([Modified]) AS Modified, [MetadataName], [MetadataValue] FROM [Metadata] WHERE {0} [Metadata].[RootMapUid] = @RootMapUid GROUP BY [NodeUid], [MetadataName], [MetadataValue]) Grouped GROUP BY [NodeUid]) Counted WHERE ResultCount = @FilterCount";
        private const string SearchANDQuery = @"SELECT TOP {0} [NodeUid] AS NodeUid, MAX([Modified]) AS Modified FROM (SELECT [NodeUid], MAX([Modified]) AS Modified, COUNT([NodeUid]) AS ResultCount  FROM (SELECT [NodeUid], MAX([Modified]) AS Modified, [MetadataName], [MetadataValue] FROM [Metadata] WHERE {3} [Metadata].[RootMapUid] = @RootMapUid GROUP BY [NodeUid], [MetadataName], [MetadataValue]) Grouped GROUP BY [NodeUid]) Counted WHERE ResultCount = @FilterCount GROUP BY NodeUid  ORDER BY {1} {2}, [NodeUid] ASC";

        public SearchConditions()
        {
        }

        private bool Ascending
        {
            get;
            set;
        }

        private string SortColumn
        {
            get;
            set;
        }

        private string OrderDirection
        {
            get;
            set;
        }

        [DataMember]
        public SortOrderOptions SortOrder
        {
            get;
            set;
        }

        [DataMember]
        public SearchOperation SearchOperation
        {
            get;
            set;
        }

        [DataMember]
        public List<SearchCondition> MetadataFilters
        {
            get;
            set;
        }

        private void BuildOrderByData()
        {
            switch (SortOrder)
            {
                case SortOrderOptions.CreatedAscending:
                    Ascending = true;
                    SortColumn = "[Created]";
                    OrderDirection = "ASC";
                    break;
                case SortOrderOptions.ModifiedAscending:
                    Ascending = true;
                    SortColumn = "[Modified]";
                    OrderDirection = "ASC";
                    break;
                case SortOrderOptions.CreatedDescending:
                    Ascending = false;
                    SortColumn = "[Created]";
                    OrderDirection = "DESC";
                    break;
                case SortOrderOptions.ModifiedDescending:
                    Ascending = false;
                    SortColumn = "[Modified]";
                    OrderDirection = "DESC";
                    break;
                default:
                    Ascending = false;
                    SortColumn = "[Modified]";
                    OrderDirection = "DESC";
                    break;
            }
        }

        public SqlCommand BuildSearchQuery(SqlConnection connection, Guid rootMapUid, int pageNumber = 1, int pageSize = 5)
        {
            BuildOrderByData();

            string searchQueryCount;
            string searchQueryText;

            int numberToTake = pageNumber * pageSize;

            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@RootMapUid", rootMapUid));

            if (MetadataFilters.Count > 0)
            {
                string filter = BuildConditionFilter(parameters);
                if (SearchOperation == SearchOperation.OR)
                {
                    searchQueryCount = string.Format(SearchQueryCount, filter);
                    searchQueryText = string.Format(SearchQuery, numberToTake, SortColumn, OrderDirection, filter);
                }
                else
                {
                    parameters.Add(new SqlParameter("@FilterCount", MetadataFilters.Count));
                    searchQueryCount = string.Format(SearchANDQueryCount, filter);
                    searchQueryText = string.Format(SearchANDQuery, numberToTake, SortColumn, OrderDirection, filter);
                }
            }
            else
            {
                searchQueryCount = string.Format(SearchQueryCount, string.Empty);
                searchQueryText = string.Format(SearchQuery, numberToTake, SortColumn, OrderDirection, string.Empty);
            }

            if (pageNumber > 1)
            {
                string forwardOrder;
                string reverseOrder;

                if (Ascending)
                {
                    forwardOrder = "ASC";
                    reverseOrder = "DESC";
                }
                else
                {
                    forwardOrder = "DESC";
                    reverseOrder = "ASC";
                }

                searchQueryText = string.Format(PaginationSearchQuery, pageSize, searchQueryText, SortColumn, reverseOrder, forwardOrder);
            }

            SqlCommand searchQuery = new SqlCommand(searchQueryCount + ";" + searchQueryText, connection);
            searchQuery.Parameters.AddRange(parameters.ToArray());

            return searchQuery;
        }

        private string BuildConditionFilter(List<SqlParameter> parameters)
        {
            StringBuilder filterBuilder = new StringBuilder();

            int count = 1;

            foreach (SearchCondition condition in MetadataFilters)
            {
                string conditionSqlQuery;

                condition.Index = count;
                count++;

                if (condition.Index == 1)
                {
                    if (MetadataFilters.Count > 1)
                    {
                        filterBuilder.Append("(");
                    }
                }
                if (condition.ValidateCondition(out conditionSqlQuery))
                {
                    filterBuilder.Append(conditionSqlQuery);
                    if (condition.Index < MetadataFilters.Count)
                    {
                        filterBuilder.Append("OR");
                    }
                }
                if (MetadataFilters.Count == condition.Index)
                {
                    if (MetadataFilters.Count > 1)
                    {
                        filterBuilder.Append(") ");
                    }
                    filterBuilder.Append("AND");
                }

                parameters.Add(condition.MetadataNameParameter);
                parameters.Add(condition.SearchConditionParameter);
            }
            return filterBuilder.ToString();
        }
    }
}