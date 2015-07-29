using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace TransactionalNodeService
{
    public interface IDbConnectionAbstraction
    {
        SqlConnection Connection { get; }

        void Open();
        void Close();
    }
}
