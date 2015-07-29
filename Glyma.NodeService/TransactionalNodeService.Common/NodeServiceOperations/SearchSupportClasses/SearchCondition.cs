using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.Data.SqlClient;

namespace TransactionalNodeService.Common
{
    public enum SearchType
    {
        Exact,
        Wildcard,
        FreeText,
        Contains,
        NumericallyLessThan,
        NumericallyLessThanOrEqual,
        NumericallyMoreThan,
        NumericallyMoreThanOrEqual,
        NumericallyEqual
    }

    [DataContract]
    public class SearchCondition
    {
        private const string ExactSearchSql = " ([Metadata].[MetadataName] = {0} AND [Metadata].[MetadataValue] = {1}) ";
        private const string WildcardSearchSql = @" ([Metadata].[MetadataName] = {0} AND [Metadata].[MetadataValue] LIKE {1} ESCAPE '\') ";
        private const string FreeTextSearchSql = " ([Metadata].[MetadataName] = {0} AND FREETEXT([Metadata].[MetadataValue], {1})) ";
        private const string ContainsSearchSql = " ([Metadata].[MetadataName] = {0} AND CONTAINS([Metadata].[MetadataValue], {1})) ";

        private const string NumericallyLessThanSearchSql = " ([Metadata].[MetadataName] = {0} AND [Metadata].[MetadataValue] < {1}) ";
        private const string NumericallyLessThanOrEqualSearchSql = " ([Metadata].[MetadataName] = {0} AND [Metadata].[MetadataValue] <= {1}) ";
        private const string NumericallyMoreThanSearchSql = " ([Metadata].[MetadataName] = {0} AND [Metadata].[MetadataValue] > {1}) ";
        private const string NumericallyMoreThanOrEqualSearchSql = " ([Metadata].[MetadataName] = {0} AND [Metadata].[MetadataValue] >= {1}) ";
        //NumericallyEqualSearchSql would be the same as ExactSearchSql

        private int _index;
        private SqlParameter _metadataNameParameter = null;
        private SqlParameter _searchConditionParameter = null;

        public SearchCondition()
        {
        }

        [IgnoreDataMember]
        private string MetadataNameParameterName
        {
            get
            {
                return ("@MetadataName" + _index);
            }
        }

        [IgnoreDataMember]
        private string SearchConditionParameterName
        {
            get
            {
                return ("@SearchCondition" + _index);
            }
        }

        [IgnoreDataMember]
        public int Index
        {
            get
            {
                return _index;
            }
            set
            {
                _index = value;
            }
        }

        [DataMember]
        public string MetadataName
        {
            get;
            set;
        }

        [DataMember]
        public string ConditionValue
        {
            get;
            set;
        }

        [DataMember]
        public SearchType SearchType
        {
            get;
            set;
        }

        [IgnoreDataMember]
        public SqlParameter MetadataNameParameter
        {
            get
            {
                if (_metadataNameParameter == null)
                {
                    _metadataNameParameter = new SqlParameter(MetadataNameParameterName, MetadataName);
                }

                return _metadataNameParameter;
            }
        }

        [IgnoreDataMember]
        public SqlParameter SearchConditionParameter
        {
            get
            {
                if (_searchConditionParameter == null)
                {
                    _searchConditionParameter = new SqlParameter(SearchConditionParameterName, ConditionValue);
                }

                return _searchConditionParameter;
            }
        }

        public bool ValidateCondition(out string sql)
        {
            sql = string.Empty;

            if (string.IsNullOrEmpty(ConditionValue))
            {
                return false;
            }

            switch (SearchType)
            {
                case SearchType.NumericallyEqual:
                case SearchType.Exact:
                    sql = string.Format(ExactSearchSql, MetadataNameParameterName, SearchConditionParameterName);
                    break;
                case SearchType.Wildcard:
                    sql = string.Format(WildcardSearchSql, MetadataNameParameterName, SearchConditionParameterName);
                    break;
                case SearchType.FreeText:
                    sql = string.Format(FreeTextSearchSql, MetadataNameParameterName, SearchConditionParameterName);
                    break;
                case SearchType.Contains:
                    sql = string.Format(ContainsSearchSql, MetadataNameParameterName, SearchConditionParameterName);
                    break;
                case SearchType.NumericallyLessThan:
                    sql = string.Format(NumericallyLessThanSearchSql, MetadataNameParameterName, SearchConditionParameterName);
                    break;
                case SearchType.NumericallyLessThanOrEqual:
                    sql = string.Format(NumericallyLessThanOrEqualSearchSql, MetadataNameParameterName, SearchConditionParameterName);
                    break;
                case SearchType.NumericallyMoreThan:
                    sql = string.Format(NumericallyMoreThanSearchSql, MetadataNameParameterName, SearchConditionParameterName);
                    break;
                case SearchType.NumericallyMoreThanOrEqual:
                    sql = string.Format(NumericallyMoreThanOrEqualSearchSql, MetadataNameParameterName, SearchConditionParameterName);
                    break;
                default:
                    return false;
            }

            return true;
        }

        private string GetUniqueViewName()
        {
            int temp;
            char letter;
            string result = "";
            int position = Index;
            while (position > 0)
            {
                temp = (position - 1) % 26;
                letter = (Char)(97 + temp);
                result += letter.ToString();
                position = (position - temp) / 26;
            }
            return result;
        }
    }
}