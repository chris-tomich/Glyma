using System;

namespace Glyma.SharePoint.Search
{
   public class GlymaNodeReference
   {
      public Guid Id { get; set; }
      public string Name { get; set; }
      public string NodeType { get; set; }
      public Guid MapId { get; set; }
      public Guid DomainId { get; set; }
   }
}
