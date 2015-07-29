using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransactionalNodeService.Common
{
    public interface IQueryOperation
    {
        bool EvaluateObject(IQueryableMapObject mapObject);
    }
}