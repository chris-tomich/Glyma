using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using Microsoft.SharePoint;
using TransactionalNodeService.Common;
using TransactionalNodeService.Common.Model;
using Glyma.Security.Model;
using Glyma.NodeServiceApp.SecurityModel;

namespace Glyma.NodeServiceApp
{
    public class WebAppSPGlymaSession : IGlymaSession
    {
        private const string ReaderRoleName = "Glyma Reader";
        private const string AuthorRoleName = "Glyma Author";

        private string _callingUrl;
        private Guid _sessionId;
        private SPWeb _web = null;
        private SPSite _site = null;
        private SPGlymaUser _glymaUser = null;
        private MapSession _session;
        private MapParameters _parameters;
        private GlymaSessionConfiguration _configuration = null;
        private IGlymaConnectionFactory _connectionFactory = null;

        public WebAppSPGlymaSession(string callingUrl, bool isParameter, MapParameter domainParameter, MapParameter rootMapParameter, params IRight[] requiredRights)
        {
            _callingUrl = callingUrl;
            _sessionId = Guid.Empty;

            IRight[] heightenedRightRequirement = requiredRights;

            Guid? domainId = null;
            Guid? rootMapId = null;

            if (rootMapParameter == null || rootMapParameter.IsDelayed)
            {
                /// If they passed through a delayed rootmap ID parameter, they are going to have to be a Glyma Map Manager at the very minimum to finish this transaction.
                heightenedRightRequirement = new IRight[] { SPGlymaRightFactory.Instance.RootMapCreateRight };
            }
            else
            {
                rootMapId = rootMapParameter.Value;
            }

            if (domainParameter == null || domainParameter.IsDelayed)
            {
                /// If they passed through a delayed domain ID parameter, they are going to have to be a Glyma Project Manager at the very minimum to finish this transaction.
                heightenedRightRequirement = new IRight[] { SPGlymaRightFactory.Instance.ProjectCreateRight };
            }
            else
            {
                domainId = domainParameter.Value;
            }

            if (!IsAuthorised(domainId, rootMapId, heightenedRightRequirement))
            {
                throw new UnauthorizedAccessException("This user does not have the required privileges for this task.");
            }

            _session = new MapSession(this);
            _parameters = new MapParameters(this);
        }

        public WebAppSPGlymaSession(string callingUrl, Guid sessionId, bool isParameter, MapParameter domainParameter, MapParameter rootMapParameter, params IRight[] requiredRights)
        {
            _callingUrl = callingUrl;
            _sessionId = sessionId;

            IRight[] heightenedRightRequirement = requiredRights;

            Guid? domainId = null;
            Guid? rootMapId = null;

            if (rootMapParameter == null || rootMapParameter.IsDelayed)
            {
                /// If they passed through a delayed rootmap ID parameter, they are going to have to be a Glyma Map Manager at the very minimum to finish this transaction.
                heightenedRightRequirement = new IRight[] { SPGlymaRightFactory.Instance.RootMapCreateRight };
            }
            else
            {
                rootMapId = rootMapParameter.Value;
            }

            if (domainParameter == null || domainParameter.IsDelayed)
            {
                /// If they passed through a delayed domain ID parameter, they are going to have to be a Glyma Project Manager at the very minimum to finish this transaction.
                heightenedRightRequirement = new IRight[] { SPGlymaRightFactory.Instance.ProjectCreateRight };
            }
            else
            {
                domainId = domainParameter.Value;
            }

            if (!IsAuthorised(domainId, rootMapId, heightenedRightRequirement))
            {
                throw new UnauthorizedAccessException("This user does not have the required privileges for this task.");
            }

            _session = new MapSession(this, _sessionId);

            _parameters = new MapParameters(this);
        }

        public WebAppSPGlymaSession(string callingUrl, Guid sessionId, bool preload, bool isParameter, MapParameter domainParameter, MapParameter rootMapParameter, params IRight[] requiredRights)
            : this(callingUrl, sessionId, isParameter, domainParameter, rootMapParameter, requiredRights)
        {
            if (preload)
            {
                _session.LoadTransactions();
                _parameters.LoadParameters();
            }
        }

        public WebAppSPGlymaSession(string callingUrl, Guid? domainId, Guid? rootMapId, params IRight[] requiredRights)
        {
            _callingUrl = callingUrl;
            _sessionId = Guid.Empty;

            if (!IsAuthorised(domainId, rootMapId, requiredRights))
            {
                throw new UnauthorizedAccessException("This user does not have the required privileges for this task.");
            }

            _session = new MapSession(this);
            _parameters = new MapParameters(this);
        }

        public WebAppSPGlymaSession(string callingUrl, Guid sessionId, Guid? domainId, Guid? rootMapId, params IRight[] requiredRights)
        {
            _callingUrl = callingUrl;
            _sessionId = sessionId;

            if (!IsAuthorised(domainId, rootMapId, requiredRights))
            {
                throw new UnauthorizedAccessException("This user does not have the required privileges for this task.");
            }

            _session = new MapSession(this, _sessionId);

            _parameters = new MapParameters(this);
        }

        public WebAppSPGlymaSession(string callingUrl, Guid sessionId, bool preload, Guid? domainId, Guid? rootMapId, params IRight[] requiredRights)
            : this(callingUrl, sessionId, domainId, rootMapId, requiredRights)
        {
            if (preload)
            {
                _session.LoadTransactions();
                _parameters.LoadParameters();
            }
        }

        public WebAppSPGlymaSession(string callingUrl)
        {
            _callingUrl = callingUrl;
            _sessionId = Guid.Empty;

            _session = new MapSession(this);
            _parameters = new MapParameters(this);
        }

        public WebAppSPGlymaSession(string callingUrl, Guid sessionId)
        {
            _callingUrl = callingUrl;
            _sessionId = sessionId;

            _session = new MapSession(this, _sessionId);

            _parameters = new MapParameters(this);
        }

        public WebAppSPGlymaSession(string callingUrl, Guid sessionId, bool preload)
            : this(callingUrl, sessionId)
        {
            if (preload)
            {
                _session.LoadTransactions();
                _parameters.LoadParameters();
            }
        }

        public SPSite Site
        {
            get
            {
                if (_site == null)
                {
                    _site = new SPSite(_callingUrl);
                }

                return _site;
            }
        }

        public SPWeb Web
        {
            get
            {
                if (_web == null)
                {
                    _web = Site.OpenWeb();
                }

                return _web;
            }
        }

        private SPGlymaUser GlymaUser
        {
            get
            {
                if (_glymaUser == null)
                {
                    _glymaUser = new SPGlymaUser(Web, this);
                }

                return _glymaUser;
            }
        }

        private bool IsAuthorised(Guid? domainId, Guid? rootMapId, params IRight[] requiredRights)
        {
            if (requiredRights == null || requiredRights.Length <= 0)
            {
                return true;
            }

            if (domainId == null && rootMapId == null)
            {
                return GlymaUser.IsAuthorised(requiredRights);
            }

            if (domainId != null && rootMapId == null)
            {
                return GlymaUser.IsAuthorised(domainId.Value, requiredRights);
            }

            if (domainId != null && rootMapId != null)
            {
                return GlymaUser.IsAuthorised(domainId.Value, rootMapId.Value, requiredRights);
            }

            if (domainId == null && rootMapId != null)
            {
                return GlymaUser.IsAuthorised(null, rootMapId.Value, requiredRights);
            }

            return false;
        }

        private void FindTransactionDbConnectionParameters(ref GlymaSessionConfiguration glymaSessionConfig)
        {
            string transactionDatabaseServer = "";
            string transactionDatabaseName = "";

            bool isExhausted = false;
            SPWeb web = Web;

            while (!isExhausted)
            {
                try
                {
                    if (web.Properties.ContainsKey("Glyma.TransactionDatabaseServer") && web.Properties.ContainsKey("Glyma.TransactionDatabaseName"))
                    {
                        transactionDatabaseServer = web.Properties["Glyma.TransactionDatabaseServer"];
                        transactionDatabaseName = web.Properties["Glyma.TransactionDatabaseName"];
                        glymaSessionConfig.ParametersDbServer = transactionDatabaseServer;
                        glymaSessionConfig.ParametersDbName = transactionDatabaseName;
                        glymaSessionConfig.SessionDbServer = transactionDatabaseServer;
                        glymaSessionConfig.SessionDbName = transactionDatabaseName;
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

                        /// We don't want to dispose the main Web for this bootstrapper object.
                        if (web != Web)
                        {
                            web.Dispose();
                        }

                        web = nextWeb;
                    }
                }
            }

            if (string.IsNullOrEmpty(transactionDatabaseServer) || string.IsNullOrEmpty(transactionDatabaseName))
            {
                throw new NullReferenceException("No database server or database name provided! Please assign the Glyma.TransactionDatabaseServer property and the Glyma.TransactionDatabaseName property on the containing SPWeb.");
            }
        }

        private void FindMapDbConnectionParameters(ref GlymaSessionConfiguration glymaSessionConfig)
        {
            string databaseServer = "";
            string databaseName = "";

            bool isExhausted = false;
            SPWeb web = Web;

            while (!isExhausted)
            {
                try
                {
                    if (web.Properties.ContainsKey("Glyma.DatabaseServer") && web.Properties.ContainsKey("Glyma.DatabaseName"))
                    {
                        databaseServer = web.Properties["Glyma.DatabaseServer"];
                        databaseName = web.Properties["Glyma.DatabaseName"];
                        glymaSessionConfig.MapDbServer = databaseServer;
                        glymaSessionConfig.MapDbName = databaseName;
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

                        /// We don't want to dispose the main Web for this bootstrapper object.
                        if (web != Web)
                        {
                            web.Dispose();
                        }

                        web = nextWeb;
                    }
                }
            }

            if (string.IsNullOrEmpty(databaseServer) || string.IsNullOrEmpty(databaseName))
            {
                throw new NullReferenceException("No database server or database name provided! Please assign the Glyma.DatabaseServer property and the Glyma.DatabaseName property on the containing SPWeb.");
            }
        }

        private void FindSecurityDbConnectionParameters(ref GlymaSessionConfiguration glymaSessionConfig)
        {
            string securityDatabaseServer = "";
            string securityDatabaseName = "";

            bool isExhausted = false;
            SPWeb web = Web;

            while (!isExhausted)
            {
                try
                {
                    if (web.Properties.ContainsKey("Glyma.SecurityDatabaseServer") && web.Properties.ContainsKey("Glyma.SecurityDatabaseName"))
                    {
                        securityDatabaseServer = web.Properties["Glyma.SecurityDatabaseServer"];
                        securityDatabaseName = web.Properties["Glyma.SecurityDatabaseName"];
                        glymaSessionConfig.SecurityDbServer = securityDatabaseServer;
                        glymaSessionConfig.SecurityDbName = securityDatabaseName;
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

                        /// We don't want to dispose the main Web for this bootstrapper object.
                        if (web != Web)
                        {
                            web.Dispose();
                        }

                        web = nextWeb;
                    }
                }
            }

            if (string.IsNullOrEmpty(securityDatabaseServer) || string.IsNullOrEmpty(securityDatabaseName))
            {
                /// TODO: When the security service is finished this must be uncommented.
                //throw new NullReferenceException("No security database server or security database name provided! Please assign the Glyma.SecurityDatabaseServer property and the Glyma.SecurityDatabaseName property on the containing SPWeb.");
            }
        }

        public MapParameters Parameters
        {
            get
            {
                return _parameters;
            }
        }

        public MapSession Session
        {
            get
            {
                return _session;
            }
        }

        public IGlymaConnectionFactory ConnectionFactory
        {
            get
            {
                if (_connectionFactory == null)
                {
                    GlymaSessionConfiguration config = ExportGlymaSession();

                    _connectionFactory = new GlymaDbConnectionFactory(config);
                }

                return _connectionFactory;
            }
        }

        public GlymaSessionConfiguration ExportGlymaSession()
        {
            if (_configuration == null)
            {
                _configuration = new GlymaSessionConfiguration();

                FindTransactionDbConnectionParameters(ref _configuration);
                FindMapDbConnectionParameters(ref _configuration);
                FindSecurityDbConnectionParameters(ref _configuration);
            }

            return _configuration;
        }

        public void PersistSessionObject()
        {
            Parameters.PersistSessionObject();
            Session.PersistSessionObject();
        }

        public void Dispose()
        {
            if (_web != null)
            {
                _web.Dispose();
            }

            if (_site != null)
            {
                _site.Dispose();
            }
        }
    }
}