using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TransactionalNodeService.Common;

namespace TransactionalNodeService.SharePoint
{
    public class SvcAppSPGlymaSession : IGlymaSession
    {
        private int _securableContextId = -1;
        private Guid _sessionId;
        private MapSession _session;
        private MapParameters _parameters;
        private GlymaSessionConfiguration _configuration;
        private IGlymaConnectionFactory _connectionFactory = null;

        public SvcAppSPGlymaSession(GlymaSessionConfiguration configuration)
        {
            _configuration = configuration;
            _sessionId = Guid.Empty;

            _session = new MapSession(this);
            _parameters = new MapParameters(this);
        }

        public SvcAppSPGlymaSession(GlymaSessionConfiguration configuration, Guid sessionId)
        {
            _configuration = configuration;
            _sessionId = sessionId;

            _session = new MapSession(this, _sessionId);

            _parameters = new MapParameters(this);
        }

        public SvcAppSPGlymaSession(GlymaSessionConfiguration configuration, Guid sessionId, bool preload)
            : this(configuration, sessionId)
        {
            if (preload)
            {
                _session.LoadTransactions();
                _parameters.LoadParameters();
            }
        }

        public int SecurableContextId
        {
            get
            {
                if (_securableContextId == -1)
                {
                    GlymaSessionConfiguration config = ExportGlymaSession();

                    _securableContextId = config.SecurableContextId;
                }

                return _securableContextId;
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
            return _configuration;
        }

        public void PersistSessionObject()
        {
            Parameters.PersistSessionObject();
            Session.PersistSessionObject();
        }

        public void Dispose()
        {
        }
    }
}
