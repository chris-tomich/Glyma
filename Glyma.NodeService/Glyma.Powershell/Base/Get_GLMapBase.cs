using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using Model = Glyma.Powershell.Model;

namespace Glyma.Powershell.Base
{
    internal class Get_GLMapBase : IGLCmdletBase
    {
        public Model.Domain Domain
        {
            get;
            set;
        }

        public Guid MapId
        {
            get;
            set;
        }

        public string MapName
        {
            get;
            set;
        }

        private Model.Map GetMapById(MappingToolDatabaseDataContext dataContext, Guid mapId)
        {
            var dbMapNodes = from qNode in dataContext.Nodes
                             where qNode.NodeUid == mapId && qNode.DomainUid == Domain.DomainId
                             select qNode;

            if (dbMapNodes == null || dbMapNodes.Count() == 0)
            {
                return null;
            }

            var dbMapNameMetadata = from qMetadata in dataContext.Metadatas
                                    where qMetadata.NodeUid == mapId && qMetadata.MetadataName == "Name"
                                    select qMetadata;

            if (dbMapNameMetadata == null || dbMapNameMetadata.Count() == 0)
            {
                return null;
            }

            var dbMapNode = dbMapNodes.First();
            var dbMapNameDatum = dbMapNameMetadata.First();

            Model.Map map = new Model.Map();
            map.Domain = Domain;
            map.NodeId = mapId;
            map.Name = dbMapNameDatum.MetadataValue;

            return map;
        }

        private List<Model.Map> GetMapsByName(MappingToolDatabaseDataContext dataContext, string mapName)
        {
            var dbLocalNodeResultSets = dataContext.QueryMapMultiDepth(Domain.DomainId, Domain.NodeId, 1, true);

            var dbLocalNodeLevel0 = dbLocalNodeResultSets.GetResult<QueryMapMultiDepthResult>();
            var dbLocalNodeLevel1 = dbLocalNodeResultSets.GetResult<QueryMapMultiDepthResult>();

            List<Model.Map> matchingMaps = new List<Model.Map>();

            if (dbLocalNodeLevel1 != null)
            {
                foreach (var dbLocalNode in dbLocalNodeLevel1)
                {
                    if (dbLocalNode.Level.HasValue && dbLocalNode.Level.Value == 1)
                    {
                        IQueryable<Metadata> dbMapNameMetadata;

                        if (mapName == "*")
                        {
                            dbMapNameMetadata = from qMetadata in dataContext.Metadatas
                                                where qMetadata.NodeUid == dbLocalNode.NodeUid && qMetadata.MetadataName == "Name"
                                                select qMetadata;
                        }
                        else
                        {
                            dbMapNameMetadata = from qMetadata in dataContext.Metadatas
                                                where qMetadata.NodeUid == dbLocalNode.NodeUid && qMetadata.MetadataName == "Name" && qMetadata.MetadataValue == mapName
                                                select qMetadata;
                        }

                        if (dbMapNameMetadata != null && dbMapNameMetadata.Count() > 0)
                        {
                            if (dbLocalNode.NodeUid != Guid.Empty)
                            {
                                var dbMapNameMetadatum = dbMapNameMetadata.First();

                                Model.Map map = new Model.Map();
                                map.Domain = Domain;
                                map.NodeId = dbLocalNode.NodeUid.Value;
                                map.Name = dbMapNameMetadatum.MetadataValue;

                                matchingMaps.Add(map);
                            }
                        }
                    }
                }
            }

            return matchingMaps;
        }

        private List<Model.Map> GetAllMaps(MappingToolDatabaseDataContext dataContext)
        {
            return GetMapsByName(dataContext, "*");
        }

        public void ExecuteCmdletBase(PSCmdlet callingCmdlet)
        {
            if (Domain == null)
            {
                callingCmdlet.WriteWarning("No valid domain has been provided.");

                return;
            }

            if (!Domain.CheckIsValid())
            {
                callingCmdlet.WriteWarning("An invalid domain object has been provided.");

                return;
            }

            Model.IDatabaseInfo dbInfo = Domain;

            using (MappingToolDatabaseDataContext dataContext = new MappingToolDatabaseDataContext(dbInfo.ConnectionString))
            {
                dataContext.CommandTimeout = 180;
                if (MapId != Guid.Empty)
                {
                    /// Find maps by ID.
                    /// 

                    Model.Map map = GetMapById(dataContext, MapId);

                    callingCmdlet.WriteObject(map);
                }
                else if (!string.IsNullOrEmpty(MapName))
                {
                    /// Find maps by name.
                    /// 

                    List<Model.Map> maps = GetMapsByName(dataContext, MapName);

                    callingCmdlet.WriteObject(maps, true);
                }
                else
                {
                    /// Fine all maps.
                    /// 

                    List<Model.Map> maps = GetAllMaps(dataContext);

                    callingCmdlet.WriteObject(maps, true);
                }
            }
        }
    }
}
