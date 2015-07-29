using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using Microsoft.SharePoint;

namespace Glyma.HttpHandlers
{
    public class ConnectionStringBuilder
    {
        private string _callingUrl;

        public ConnectionStringBuilder(string callingUrl)
        {
            _callingUrl = callingUrl;
        }

        public string BuildTransactionConnectionString()
        {
            string transactionDatabaseServer = "";
            string transactionDatabaseName = "";

            using (SPSite site = new SPSite(_callingUrl))
            {
                bool isExhausted = false;
                SPWeb web = site.OpenWeb();

                while (!isExhausted)
                {
                    try
                    {
                        if (web.Properties.ContainsKey("Glyma.TransactionDatabaseServer") && web.Properties.ContainsKey("Glyma.TransactionDatabaseName"))
                        {
                            transactionDatabaseServer = web.Properties["Glyma.TransactionDatabaseServer"];
                            transactionDatabaseName = web.Properties["Glyma.TransactionDatabaseName"];
                            isExhausted = true;
                            break;
                        }
                        else
                        {
                            if (web.IsRootWeb)
                            {
                                isExhausted = true;
                            }
                        }
                    }
                    finally
                    {
                        if (web != null)
                        {
                            SPWeb nextWeb = null;

                            if (!isExhausted)
                            {
                                nextWeb = web.ParentWeb;
                            }

                            web.Dispose();
                            web = nextWeb;
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(transactionDatabaseServer) || string.IsNullOrEmpty(transactionDatabaseName))
            {
                throw new NullReferenceException("No database server or database name provided! Please assign the Glyma.TransactionDatabaseServer property and the Glyma.TransactionDatabaseName property on the containing SPWeb.");
            }

            return string.Format("Data Source={0};Initial Catalog={1};Integrated Security=SSPI;", transactionDatabaseServer, transactionDatabaseName);
        }

        public string BuildMapConnectionString()
        {
            string databaseServer = "";
            string databaseName = "";

            using (SPSite site = new SPSite(_callingUrl))
            {
                bool isExhausted = false;
                SPWeb web = site.OpenWeb();

                while (!isExhausted)
                {
                    try
                    {
                        if (web.Properties.ContainsKey("Glyma.DatabaseServer") && web.Properties.ContainsKey("Glyma.DatabaseName"))
                        {
                            databaseServer = web.Properties["Glyma.DatabaseServer"];
                            databaseName = web.Properties["Glyma.DatabaseName"];
                            isExhausted = true;
                            break;
                        }
                        else
                        {
                            if (web.IsRootWeb)
                            {
                                isExhausted = true;
                            }
                        }
                    }
                    finally
                    {
                        if (web != null)
                        {
                            SPWeb nextWeb = null;

                            if (!isExhausted)
                            {
                                nextWeb = web.ParentWeb;
                            }

                            web.Dispose();
                            web = nextWeb;
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(databaseServer) || string.IsNullOrEmpty(databaseName))
            {
                throw new NullReferenceException("No database server or database name provided! Please assign the Glyma.DatabaseServer property and the Glyma.DatabaseName property on the containing SPWeb.");
            }

            return string.Format("Data Source={0};Initial Catalog={1};Integrated Security=SSPI;", databaseServer, databaseName);
        }
    }
}