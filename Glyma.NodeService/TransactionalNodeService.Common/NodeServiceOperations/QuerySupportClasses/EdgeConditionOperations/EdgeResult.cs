﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransactionalNodeService.Common
{
    public class EdgeResult
    {
        public bool? IsEdge
        {
            get;
            set;
        }

        public bool IsIncluded
        {
            get;
            set;
        }
    }
}