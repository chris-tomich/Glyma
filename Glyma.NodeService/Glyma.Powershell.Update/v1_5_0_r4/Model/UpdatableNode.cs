using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glyma.Powershell.Update.v1_5_0_r4.Model
{
    public class UpdatableNode
    {
        public Guid NodeUid { get; set; }

        public Guid DomainUid { get; set; }

        public Guid RootMapUid { get; set; }

        public DateTime DescriptionCreated { get; set; }

        public DateTime DescriptionModified { get; set; }

        public string DescriptionCreatedBy { get; set; }

        public string DescriptionModifiedBy { get; set; }

        public Guid DescriptionMetadataId { get; set; }

        public string Description { get; set; }

        public Guid DescriptionTypeMetadataId { get; set; }

        public string DescriptionType { get; set; }

        public bool HasDescription { get; set; }
    }
}
