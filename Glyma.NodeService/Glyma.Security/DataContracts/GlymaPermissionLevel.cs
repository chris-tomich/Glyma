using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Glyma.Security
{
    [DataContract]
    public enum GlymaPermissionLevel
    {
        [EnumMember]
        GlymaSecurityManager,
        [EnumMember]
        GlymaProjectManager,
        [EnumMember]
        GlymaMapManager,
        [EnumMember]
        GlymaMapAuthor,
        [EnumMember]
        GlymaMapReader,
        [EnumMember]
        GlymaMapAuthorOld,
        [EnumMember]
        GlymaMapReaderOld,
        [EnumMember]
        None
    }
}
