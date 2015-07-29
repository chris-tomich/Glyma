using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Glyma.NodeService.Search
{
    public partial class OpenSearchDescriptionDocument
    {
        public OpenSearchDescriptionDocument()
        {
        }

        public string ShortName
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public string Tags
        {
            get;
            set;
        }

        public string Contact
        {
            get;
            set;
        }

        public FeedFormat FeedType
        {
            get;
            set;
        }

        public string Template
        {
            get;
            set;
        }
    }
}