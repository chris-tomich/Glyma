﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Glyma.UtilityService.Common.ExportServiceClasses
{
    [DataContract(Namespace = "http://sevensigma.com.au/GlymaUtilityService/ExportObjects")]
    public class ExportAvailabilityResponse
    {
        [DataMember]
        public bool IsAvailable
        {
            get;
            set;
        }
    }
}
