using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace TransactionalNodeService.Common
{
    public interface IGlymaSession : IPersistableSessionContainer, IDisposable
    {
        int SecurableContextId { get; }

        MapParameters Parameters { get; }
        MapSession Session { get; }

        IGlymaConnectionFactory ConnectionFactory { get; }

        GlymaSessionConfiguration ExportGlymaSession();

        //string FindUsersName(string claim);
    }
}
