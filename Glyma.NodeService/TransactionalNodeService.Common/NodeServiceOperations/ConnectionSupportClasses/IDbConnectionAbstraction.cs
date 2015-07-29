using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace TransactionalNodeService.Common
{
    public interface IDbConnectionAbstraction : IDisposable
    {
        SqlConnection Connection { get; }

        void Open();
        void Close();
    }
}
