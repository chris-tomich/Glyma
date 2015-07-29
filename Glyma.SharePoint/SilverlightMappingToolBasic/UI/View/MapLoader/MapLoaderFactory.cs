using System;

namespace SilverlightMappingToolBasic.UI.View.MapLoader
{
    public class MapLoaderFactory
    {
        private readonly IMapPreloader _mapPreloader;

        public MapLoaderFactory(IMapPreloader mapPreloader)
        {
            _mapPreloader = mapPreloader;
        }

        public IMapLoader CreateMapLoader(Guid domainUid, Guid mapUid, Guid nodeUid)
        {
            if (mapUid != Guid.Empty)
            {
                return new MapLoaderByMapUid(_mapPreloader, domainUid, mapUid, nodeUid);
            }
            if (nodeUid != Guid.Empty)
            {
                return new MapLoaderByNodeUid(_mapPreloader, domainUid, mapUid, nodeUid);
            }
            return null;
        }
    }
}
