using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Microsoft.SharePoint;
using TransactionalNodeService.Common;

namespace TransactionalNodeService.SharePoint
{
    public class DbConnectionAbstraction : IDbConnectionAbstraction, IDisposable
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
                if (Connection.State != System.Data.ConnectionState.Open)
                {
                    Connection.Open();
                }
            });
        }

        public void Close()
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                if (Connection.State != System.Data.ConnectionState.Closed)
                {
                    Connection.Close();
                }
            });
        }

        public void Dispose()
        {
            if (Connection != null)
            {
                Close();

                Connection.Dispose();
                Connection = null;
            }
        }
    }
}