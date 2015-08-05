using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Glyma.SharePoint.Search
{
   [XmlRoot("Groups")]
   public class SpGroupDetailContainer 
   {
      [XmlElement("Group")]
      public List<SpGroupDetail> Items { get; set; }

      public SpGroupDetailContainer()
      {
         Items = new List<SpGroupDetail>();
      }
   }
}
