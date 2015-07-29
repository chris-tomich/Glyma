using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Reflection;

namespace Glyma.NodeService.Search.Model.Sql
{
    partial class GlymaDatabaseDataContext
    {
        [Function(Name = "BasicSearch")]
        [ResultType(typeof(Node))]
        [ResultType(typeof(Metadata))]
        public IMultipleResults BasicSearchFullResults(Nullable<System.Guid> domainId, string searchTerms)
        {
            IExecuteResult results = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), domainId, searchTerms);

            return (IMultipleResults)results.ReturnValue;
        }
    }
}
