using System;
using System.Collections.Generic;

namespace Glyma.SharePoint.Search
{
   public interface IGlymaSecurityRepository : IDisposable
   {
      RepositoryConnectionSetting ConnectionSetting { get; set; }
      List<SpFarmGroupIdentifier> GetAllowedGroups(string repositoryName, Guid domainId, Guid rootMapId);
      List<Guid> GetSiteCollectionAssociations(string databaseName);
   }
}
