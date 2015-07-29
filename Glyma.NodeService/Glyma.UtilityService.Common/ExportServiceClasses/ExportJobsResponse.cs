using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using Glyma.UtilityService.Common.Model;

namespace Glyma.UtilityService.Common.ExportServiceClasses
{
    [DataContract(Namespace = "http://sevensigma.com.au/GlymaUtilityService/ExportObjects")]
    public class ExportJobsResponse
    {
        private IDictionary<Guid, ExportJob> _exportJobs;

        [DataMember]
        public IDictionary<Guid, ExportJob> ExportJobs
        {
            get
            {
                if (_exportJobs == null)
                {
                    _exportJobs = new Dictionary<Guid, ExportJob>();
                }
                return _exportJobs;
            }
            set
            {
                _exportJobs = value;
            }
        }
    }
}
