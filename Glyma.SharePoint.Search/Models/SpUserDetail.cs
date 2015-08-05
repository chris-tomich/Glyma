using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Glyma.SharePoint.Search
{
   public class SpUserDetail : IEquatable<SpUserDetail>
   {
      [XmlAttribute("ID")]
      public int Id { get; set; }

      [XmlAttribute("Name")]
      public string Name { get; set; }

      [XmlAttribute("LoginName")]
      public string LoginName { get; set; }

      [XmlAttribute("Sid")]
      public string Sid { get; set; }

      [XmlAttribute("IsSiteAdmin")]
      public string IsSiteAdminString { get; set; }

      [XmlIgnore]
      public bool IsSiteAdmin
      {
         get
         {
            return bool.Parse(IsSiteAdminString);
         }

         set
         {
            IsSiteAdminString = value.ToString();
         }
      }


      public bool Equals(SpUserDetail other)
      {
         if (Object.ReferenceEquals(other, null))
         {
            return false;
         }

         if (Object.ReferenceEquals(this, other))
         {
            return true;
         }

         if (this.GetType() != other.GetType())
         {
            return false;
         }

         return LoginName.Equals(other.LoginName, StringComparison.OrdinalIgnoreCase);
      }


      public override bool Equals(object obj)
      {
         return this.Equals(obj as SpUserDetail);
      }


      public static bool operator ==(SpUserDetail lhs, SpUserDetail rhs)
      {
         if (Object.ReferenceEquals(lhs, null))
         {
            if (Object.ReferenceEquals(rhs, null))
            {
               return true;
            }
            else
            {
               return false;
            }
         }
         else
         {
            return lhs.Equals(rhs);
         }
      }


      public static bool operator !=(SpUserDetail lhs, SpUserDetail rhs)
      {
         return !(lhs == rhs);
      }


      public override int GetHashCode()
      {
         return LoginName.GetHashCode();
      }
   }
}
