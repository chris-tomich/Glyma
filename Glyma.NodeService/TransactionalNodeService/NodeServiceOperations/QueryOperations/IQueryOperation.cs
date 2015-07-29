using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransactionalNodeService
{
    public interface IQueryOperation
    {
        bool EvaluateObject(IQueryableMapObject mapObject);
    }
}