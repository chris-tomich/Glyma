using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glyma.SharePoint.Search
{
    /// <summary>
    /// Defines the base field names for a Glyma node.
    /// </summary>
    public static class GlymaEntityFields
    {
        public readonly static string Id = "Id";
        public readonly static string Name = "Name";
        public readonly static string RepositoryName = "RepositoryName";
        public readonly static string DomainId = "DomainId";
        public readonly static string DomainName = "DomainName";
        public readonly static string RootMapId = "RootMapId";
        public readonly static string MapId = "MapId";
        public readonly static string MapName = "MapName";
        public readonly static string NodeType = "NodeType";
        public readonly static string LastModified = "LastModified";
        public readonly static string ParentNodes = "ParentNodes";
        public readonly static string ChildNodes = "ChildNodes";
        public readonly static string Content = "Content";
    }
}
