using System;
using System.Collections.Generic;
using Microsoft.BusinessData.Runtime;
using Microsoft.BusinessData.MetadataModel;
using Microsoft.BusinessData.MetadataModel.Collections;

namespace Glyma.SharePoint.Search
{
   public interface IGlymaMapRepository : IDisposable
   {
      RepositoryConnectionSetting ConnectionSetting { get; set; }
      string GetDatabaseName();
      List<DynamicType> GetRootMaps(string repositoryName, Guid domainId, ITypeDescriptorCollection entityFields);
      List<DynamicType> GetNodesInMap(string repositoryName, Guid mapId, ITypeDescriptorCollection entityFields, bool mapsOnly);
      DynamicType GetNode(string repositoryName, Guid nodeId, ITypeDescriptorCollection entityFields);
      List<GlymaNodeReference> GetParentNodes(Guid mapId, Guid nodeId);
      List<GlymaNodeReference> GetChildNodes(Guid mapId, Guid nodeId);
   }
}
