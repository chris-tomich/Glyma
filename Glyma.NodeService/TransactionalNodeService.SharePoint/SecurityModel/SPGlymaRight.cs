using Glyma.Security.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransactionalNodeService.SharePoint.SecurityModel
{
    public class SPGlymaRight : IRight
    {
        public SPGlymaRight(int rightId, string displayName)
        {
            RightId = rightId;
            DisplayName = displayName;
        }

        public int RightId
        {
            get;
            private set;
        }

        public string DisplayName
        {
            get;
            private set;
        }
    }
}