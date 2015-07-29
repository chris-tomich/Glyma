using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace TransactionalNodeService
{
    public interface IMapObjects : IPersistableSessionContainer, IDisposable
    {
        IDbConnectionAbstraction ParametersDbConnection { get; }
        IDbConnectionAbstraction SessionDbConnection { get; }
        IDbConnectionAbstraction MapDbConnection { get; }

        MapParameters Parameters { get; }
        MapSession Session { get; }

        string FindUsersName(string claim);
    }
}
