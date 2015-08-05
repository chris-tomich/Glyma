using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Glyma.SharePoint.Search
{
   [XmlRoot("Users")]
   public class SpUserDetailContainer
   {
      [XmlElement("User")]
      public List<SpUserDetail> Items { get; set; }

      public SpUserDetailContainer()
      {
         Items = new List<SpUserDetail>();
      }
   }
}
