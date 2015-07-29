using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransactionalNodeService.Common
{
    public interface IGlymaDbConnections
    {
        IDbConnectionAbstraction ParametersDbConnection { get; }
        IDbConnectionAbstraction SessionDbConnection { get; }
        IDbConnectionAbstraction MapDbConnection { get; }
        IDbConnectionAbstraction SecurityDbConnection { get; }
    }
}
