using System;
using System.Reflection;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace Glyma.Powershell.Update.v1_5_0_r1
{
    partial class MappingToolDatabaseDataContext
    {
        [Function(Name = "QueryMap")]
        [ResultType(typeof(QueryMapMultiDepthResult))]
        public IMultipleResults QueryMapMultiDepth(Nullable<System.Guid> domainId, Nullable<System.Guid> nodeId, Nullable<int> depth, Nullable<bool> fullDomain)
        {
            IExecuteResult results = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), domainId, nodeId, depth, fullDomain);

            return (IMultipleResults)results.ReturnValue;
        }
    }
}
