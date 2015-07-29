using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using Model = Glyma.Powershell.Model;

namespace Glyma.Powershell.Base
{
    internal class Get_GLDomainBase : IGLCmdletBase
    {
        private Guid _domainNodeType;

        public Get_GLDomainBase()
        {
        }

        private Guid DomainNodeType
        {
            get
            {
                if (_domainNodeType == Guid.Empty)
                {
                    _domainNodeType = new Guid("263754C2-2F31-4D21-B9C4-6509E00A5E94");
                }

                return _domainNodeType;
            }
        }

        private string ConnectionString
        {
            get
            {
                return "Data Source=" + DatabaseServer + ";Initial Catalog=" + DatabaseName + ";Integrated Security=True";
            }
        }

        public string DatabaseServer
        {
            get;
            set;
        }

        public string DatabaseName
        {
            get;
            set;
        }

        public Guid DomainId
        {
            get;
            set;
        }

        public string DomainName
        {
            get;
            set;
        }

        private Model.Domain GetDomainById(MappingToolDatabaseDataContext dataContext, Guid domainId)
        {
            var dbDomainNodes = from qDomainNode in dataContext.Nodes
                                where qDomainNode.DomainUid == domainId && qDomainNode.NodeTypeUid == DomainNodeType
                                select qDomainNode;

            if (dbDomainNodes == null || dbDomainNodes.Count() == 0)
            {
                return null;
            }

            var dbDomainNode = dbDomainNodes.First();

            var dbMatchingMetadata = from qMetadata in dataContext.Metadatas
                                     where qMetadata.NodeUid == dbDomainNode.NodeUid && qMetadata.MetadataName == "Name"
                                     select qMetadata;

            string domainName = string.Empty;

            if (dbMatchingMetadata != null && dbMatchingMetadata.Count() > 0)
            {
                var domainMetadata = dbMatchingMetadata.First();
                domainName = domainMetadata.MetadataValue;
            }

            Model.Domain domain = new Model.Domain();
            domain.DomainId = domainId;
            domain.NodeId = dbDomainNode.NodeUid;
            domain.Name = domainName;

            Model.IDatabaseInfo dbInfo = domain;
            dbInfo.DatabaseServer = DatabaseServer;
            dbInfo.DatabaseName = DatabaseName;

            return domain;
        }

        private List<Model.Domain> GetDomainsByName(MappingToolDatabaseDataContext dataContext, string domainName)
        {
            var dbDomainNodes = from qDomainNode in dataContext.Nodes
                                where qDomainNode.NodeTypeUid == DomainNodeType
                                select qDomainNode;

            var dbMatchingNodes = from qMetadata in dataContext.Metadatas
                                  join qDomainNode in dbDomainNodes on qMetadata.NodeUid equals qDomainNode.NodeUid
                                  where qMetadata.MetadataName == "Name" && qMetadata.MetadataValue == domainName
                                  select qDomainNode;

            List<Model.Domain> matchingDomains = new List<Model.Domain>();

            if (dbMatchingNodes == null || dbMatchingNodes.Count() == 0)
            {
                return matchingDomains;
            }

            foreach (var dbDomainNode in dbMatchingNodes)
            {
                if (!dbDomainNode.DomainUid.HasValue || dbDomainNode.DomainUid.Value == Guid.Empty)
                {
                    continue;
                }

                Model.Domain domain = new Model.Domain();
                domain.DomainId = dbDomainNode.DomainUid.Value;
                domain.NodeId = dbDomainNode.NodeUid;
                domain.Name = domainName;

                Model.IDatabaseInfo dbInfo = domain;
                dbInfo.DatabaseServer = DatabaseServer;
                dbInfo.DatabaseName = DatabaseName;

                matchingDomains.Add(domain);
            }

            return matchingDomains;
        }

        private List<Model.Domain> GetAllDomains(MappingToolDatabaseDataContext dataContext)
        {
            var dbAllDomainNodes = from qDomainNode in dataContext.Nodes
                                   where qDomainNode.NodeTypeUid == DomainNodeType
                                   select qDomainNode;

            var dbAllDomainNodesAndNames = from qMetadata in dataContext.Metadatas
                                           join qDomainNode in dbAllDomainNodes on qMetadata.NodeUid equals qDomainNode.NodeUid
                                           where qMetadata.MetadataName == "Name"
                                           select new { qDomainNode.NodeUid, qDomainNode.DomainUid.Value, qMetadata.MetadataValue };

            List<Model.Domain> allDomains = new List<Model.Domain>();

            if (dbAllDomainNodesAndNames == null || dbAllDomainNodesAndNames.Count() == 0)
            {
                return allDomains;
            }

            foreach (var domainNodeAndName in dbAllDomainNodesAndNames)
            {
                if (domainNodeAndName.Value != Guid.Empty)
                {
                    Model.Domain domain = new Model.Domain();
                    domain.DomainId = domainNodeAndName.Value;
                    domain.NodeId = domainNodeAndName.NodeUid;
                    domain.Name = domainNodeAndName.MetadataValue;

                    Model.IDatabaseInfo dbInfo = domain;
                    dbInfo.DatabaseServer = DatabaseServer;
                    dbInfo.DatabaseName = DatabaseName;

                    allDomains.Add(domain);
                }
            }

            return allDomains;
        }

        public void ExecuteCmdletBase(PSCmdlet callingCmdlet)
        {
            using (MappingToolDatabaseDataContext dataContext = new MappingToolDatabaseDataContext(ConnectionString))
            {
                dataContext.CommandTimeout = 180;
                if (DomainId != Guid.Empty)
                {
                    /// Find domains by domain ID.
                    /// 

                    Model.Domain domain = GetDomainById(dataContext, DomainId);

                    callingCmdlet.WriteObject(domain);
                }
                else if (!string.IsNullOrEmpty(DomainName))
                {
                    /// Find domains by domain name.
                    /// 

                    List<Model.Domain> domains = GetDomainsByName(dataContext, DomainName);

                    callingCmdlet.WriteObject(domains, true);
                }
                else
                {
                    /// Find all domains.
                    /// 

                    List<Model.Domain> domains = GetAllDomains(dataContext);

                    callingCmdlet.WriteObject(domains, true);
                }
            }
        }
    }
}
