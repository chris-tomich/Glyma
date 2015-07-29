using Microsoft.SharePoint.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glyma.NodeServiceApp
{
    public class NodeServiceInstance : SPIisWebServiceInstance
    {
        public NodeServiceInstance()
        {
        }

        public NodeServiceInstance(SPServer server, NodeService service)
            : base(server, service)
        {
        }

        public override string DisplayName
        {
            get
            {
                return "Glyma Node Service";
            }
        }

        public override string Description
        {
            get
            {
                return "The Glyma Node Service provides transactional-based graph persistence and retrieval services to the Glyma web application.";
            }
        }

        public override string TypeName
        {
            get
            {
                return "Glyma Node Service";
            }
        }
    }
}
