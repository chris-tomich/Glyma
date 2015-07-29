using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glyma.HttpHandlers.ViewMapClasses
{
    public class JsonBreadcrumb
    {
        public JsonBreadcrumb()
        {
        }

        public JsonBreadcrumb(QueryMapNode node)
        {
            uniqueId = node.NodeUid;
            name = node.FindSingleMetadata("Name").MetadataValue.Replace('"', '\'');
        }

        public Guid uniqueId
        {
            get;
            set;
        }

        public string name
        {
            get;
            set;
        }
    }
}
