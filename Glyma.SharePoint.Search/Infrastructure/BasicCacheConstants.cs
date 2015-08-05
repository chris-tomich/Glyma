using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glyma.SharePoint.Search
{
   public static class BasicCacheConstants
   {
      public static readonly TimeSpan DefaultAutoExpirePeriod = new TimeSpan(0, 10, 0);
      public static readonly int DefaultMaxItems = 10000;
      public static readonly DateTime NoAbsoluteExpiration = DateTime.MaxValue;
      public static readonly TimeSpan NoSlidingExpiration = TimeSpan.Zero;
   }
}
