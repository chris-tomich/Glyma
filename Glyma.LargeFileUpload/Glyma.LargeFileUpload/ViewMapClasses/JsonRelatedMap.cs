using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glyma.HttpHandlers.ViewMapClasses
{
    public class JsonRelatedMap
    {
        public string Name
        {
            get;
            set;
        }

        public Guid DomainId
        {
            get;
            set;
        }

        public Guid MapNodeId
        {
            get;
            set;
        }

        public Guid NodeId
        {
            get;
            set;
        }
    }
}
