using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glyma.SharePoint.Search
{
   /// <summary>
   /// Defines the constants for the Glyma Custom Indexing Connector.
   /// </summary>
   public static class GlymaModelConstants
   {
      public readonly static string Protocol = "glyma";
      public readonly static string Namespace = "Glyma.SharePoint.Search";
      public readonly static string LobSystemName = "Glyma";
      public readonly static string MapEntityName = "GlymaMap";
      public readonly static string NodeEntityName = "GlymaNode";
      public readonly static Guid PropertySetGuid = new Guid("{686A237F-8DBC-4D46-83B1-0EF4EFC1F7AD}");
      public readonly static string WordBreakString = ". ";
      public static readonly string MapConnectionString = "ConnectionString";
      public static readonly string SecurityConnectionString = "SecurityConnectionString";
      public static readonly string DataAccessType = "DataAccessType";
      public static readonly string EnableNodeAclCache = "EnableNodeAclCache";
      public static readonly string NodeAclType = "NodeAclType";
      public static readonly string EnableSecurity = "EnableSecurity";
      public static readonly string NodeAclCacheDuration = "NodeAclCacheDuration";
      public static readonly string NodeAclCacheAutoExpirePeriod = "NodeAclCacheAutoExpirePeriod";
      public static readonly string NodeAclCacheMaxItems = "NodeAclCacheMaxItems";
      public static readonly string NodeAclTaskWaitDuration = "NodeAclTaskWaitDuration";
   }
}
