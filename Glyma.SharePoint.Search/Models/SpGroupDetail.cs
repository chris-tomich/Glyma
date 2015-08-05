using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Glyma.SharePoint.Search
{
   public class SpGroupDetail
   {
      [XmlAttribute("ID")]
      public int Id { get; set; }

      [XmlAttribute("Name")]
      public string Name { get; set; }
   }
}
