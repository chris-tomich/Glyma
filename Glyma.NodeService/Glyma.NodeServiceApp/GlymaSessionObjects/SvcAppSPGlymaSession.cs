using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TransactionalNodeService.Common;

namespace Glyma.NodeServiceApp
{
    public class SvcAppSPGlymaSession : IGlymaSession
    {
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
