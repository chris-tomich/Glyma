using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glyma.HttpHandlers.ViewMapClasses
{
    public class JsonMapObjects
    {
        public Guid domain
        {
            get;
            set;
        }

        public JsonNode rootMap
        {
            get;
            set;
        }

        public JsonBreadcrumb[] breadcrumbs
        {
            get;
            set;
        }

        public JsonNode[] nodes
        {
            get;
            set;
        }

        public JsonArrow[] arrows
        {
            get;
            set;
        }
    }
}
