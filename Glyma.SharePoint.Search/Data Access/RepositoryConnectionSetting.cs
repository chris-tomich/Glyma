using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glyma.SharePoint.Search
{
   public abstract class RepositoryConnectionSetting : Dictionary<string, object>
   {
      public abstract bool IsValid();
      public abstract void Validate();
   }
}
