using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Glyma.UtilityService.Common.Model
{
    [DataContract(Namespace = "http://sevensigma.com.au/GlymaUtilityService/ExportObjects")]
    public class ExportJob
    {
        [DataMember]
        public Guid Id
        {
            get;
            set;
        }

        [DataMember]
        public ExportType Type
        {
            get;
            set;
        }

        [DataMember]
        public MapType MapType
        {
            get;
            set;
        }

        [DataMember]
        public DateTime? Created
        {
            get;
            set;
        }

        [DataMember]
        public GlymaUser CreatedBy
        {
            get;
            set;
        }

        [DataMember]
        public ExportStatus Status
        {
            get;
            set;
        }

        [DataMember]
        public bool IsCurrent
        {
            get;
            set;
        }

        [DataMember]
        public string Link
        {
            get;
            set;
        }

        [DataMember]
        public IDictionary<string, string> ExportProperties
        {
            get;
            set;
        }

        [DataMember]
        public IEnumerable<Guid> SelectedNodes
        {
            get;
            set;
        }

        [DataMember]
        public int PercentageComplete
        {
            get;
            set;
        }
    }
}
