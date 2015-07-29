using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace TransactionalNodeService.SharePoint.SecurityModel.SupportClasses
{
    public class MultipleOrExpressionFilter<QueryResult, FilterObject>
    {
        private string _comparisonProperty;
        private IQueryable<QueryResult> _originalResultSet;
        private IEnumerable<FilterObject> _filterObjects;

        public MultipleOrExpressionFilter(IQueryable<QueryResult> originalResultSet, IEnumerable<FilterObject> filterObjects, string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
            _originalResultSet = originalResultSet;
            _filterObjects = filterObjects;
        }

        private Expression BuildOrFilterExpression(ParameterExpression comparisonObjectPE)
        {
            Expression orComparisons = null;

            foreach (FilterObject filterObject in _filterObjects)
            {
                Expression left = Expression.Property(comparisonObjectPE, _comparisonProperty);
                Expression right = Expression.Constant(filterObject);
                Expression equals;

                if (left.Type == right.Type)
                {
                    equals = Expression.Equal(left, right);
                }
                else
                {
                    Expression rightWithCorrectType = Expression.Convert(right, left.Type);
                    equals = Expression.Equal(left, rightWithCorrectType);
                }

                if (orComparisons == null)
                {
                    orComparisons = equals;
                }
                else
                {
                    orComparisons = Expression.OrElse(orComparisons, equals);
                }
            }

            return orComparisons;
        }

        public IQueryable<QueryResult> FilterResultSet()
        {
            ParameterExpression comparisonObjectPE = Expression.Parameter(typeof(QueryResult), "ComparisonObject");

            Expression orFilterExpression = BuildOrFilterExpression(comparisonObjectPE);

            if (orFilterExpression == null)
            {
                return _originalResultSet;
            }

            MethodCallExpression whereInfoCallExpression = Expression.Call(typeof(Queryable),
                                                                           "Where",
                                                                           new Type[] { _originalResultSet.ElementType },
                                                                           _originalResultSet.Expression,
                                                                           Expression.Lambda<Func<QueryResult, bool>>(orFilterExpression, new ParameterExpression[] { comparisonObjectPE }));

            IQueryable<QueryResult> filteredResultSet = _originalResultSet.Provider.CreateQuery<QueryResult>(whereInfoCallExpression);

            return filteredResultSet;
        }
    }
}
