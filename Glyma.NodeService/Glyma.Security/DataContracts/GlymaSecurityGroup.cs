using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Glyma.Security
{
    [DataContract]
    public class GlymaSecurityGroup
    {
        [DataMember]
        public string DisplayName
        {
            get;
            set;
        }

        [DataMember]
        public int GroupId
        {
            get;
            set;
        }

        [DataMember]
        public int SecurableContextId
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            GlymaSecurityGroup gsg = obj as GlymaSecurityGroup;
            if (gsg != null)
            {
                if (gsg.GroupId == this.GroupId)
                {
                    return true;
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
