using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransactionalNodeService.Common
{
    public interface IGlymaConnectionFactory
    {
        IDbConnectionAbstraction CreateParametersDbConnection();
        IDbConnectionAbstraction CreateSessionDbConnection();
        IDbConnectionAbstraction CreateMapDbConnection();
        IDbConnectionAbstraction CreateSecurityDbConnection();
    }
}
