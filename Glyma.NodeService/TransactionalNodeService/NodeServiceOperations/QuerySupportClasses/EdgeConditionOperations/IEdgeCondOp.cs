using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TransactionalNodeService.Model;

namespace TransactionalNodeService
{
    public interface IEdgeCondOp
    {
        /// <summary>
        /// Evaluates the object to check whether it represents the boundary.
        /// </summary>
        /// <param name="edge">The object to evaluate.</param>
        /// <returns>Will return a result object that details whether this is a boundary and whether it should be included.</returns>
        EdgeResult EvaluateCondition(IMapElement edge);
    }
}
