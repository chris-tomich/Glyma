using TransactionalNodeService.Proxy;

namespace SilverlightMappingToolBasic.UI.SuperGraph.Controller
{
    public abstract class CommonSuperGraphBase
    {
        private NodeType _mapNodeType;

        private MetadataType _stringType;

        private ConnectionType _toConnectionType;
        private ConnectionType _fromConnectionType;
        private ConnectionType _transclusionMapConnectionType;

        private RelationshipType _mapContainerRelationshipType;
        private RelationshipType _transclusionFromToRelationshipType;
        private RelationshipType _fromToRelationshipType;

        protected CommonSuperGraphBase(IMapManager mapManager)
        {
            MapManager = mapManager;
        }

        #region Common Super Graph Types
        protected RelationshipType MapContainerRelationshipType
        {
            get
            {
                if (_mapContainerRelationshipType == null)
                {
                    _mapContainerRelationshipType = MapManager.RelationshipTypes["MapContainerRelationship"];
                }

                return _mapContainerRelationshipType;
            }
        }

        protected RelationshipType FromToRelationshipType
        {
            get
            {
                if (_fromToRelationshipType == null)
                {
                    _fromToRelationshipType = MapManager.RelationshipTypes["FromToRelationship"];
                }

                return _fromToRelationshipType;
            }
        }

        protected ConnectionType ToConnectionType
        {
            get
            {
                if (_toConnectionType == null)
                {
                    _toConnectionType = MapManager.ConnectionTypes["To"];
                }

                return _toConnectionType;
            }
        }

        protected ConnectionType FromConnectionType
        {
            get
            {
                if (_fromConnectionType == null)
                {
                    _fromConnectionType = MapManager.ConnectionTypes["From"];
                }

                return _fromConnectionType;
            }
        }

        protected MetadataType StringMetadataType
        {
            get
            {
                if (_stringType == null)
                {
                    _stringType = MapManager.MetadataTypes["string"];
                }

                return _stringType;
            }
        }

        protected NodeType MapNodeType
        {
            get
            {
                if (_mapNodeType == null)
                {
                    _mapNodeType = MapManager.NodeTypes["CompendiumMapNode"];
                }

                return _mapNodeType;
            }
        }

        protected ConnectionType TransclusionMapConnectionType
        {
            get
            {
                if (_transclusionMapConnectionType == null)
                {
                    _transclusionMapConnectionType = MapManager.ConnectionTypes["TransclusionMap"];
                }

                return _transclusionMapConnectionType;
            }
        }

        protected RelationshipType TransclusionRelationshipType
        {
            get
            {
                if (_transclusionFromToRelationshipType == null)
                {
                    _transclusionFromToRelationshipType = MapManager.RelationshipTypes["TransclusionFromToRelationship"];
                }

                return _transclusionFromToRelationshipType;
            }
        }

        #endregion

        public IMapManager MapManager
        {
            get;
            set;
        }
    }
}
