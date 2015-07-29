using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Microsoft.SharePoint;

namespace TransactionalNodeService
{
    public class DbConnectionAbstraction : IDbConnectionAbstraction
    {
        public SqlConnection Connection
        {
            get;
            set;
        }

        public void Open()
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                Connection.Open();
            });
        }

        public void Close()
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                Connection.Close();
            });
        }
    }
}