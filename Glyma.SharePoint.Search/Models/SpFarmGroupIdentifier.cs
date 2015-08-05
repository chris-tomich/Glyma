using System;

namespace Glyma.SharePoint.Search
{
   public class SpFarmGroupIdentifier : IEquatable<SpFarmGroupIdentifier>
   {
      public int GroupId { get; set; }
      public Guid SiteCollectionId { get; set; }
  

      public bool Equals(SpFarmGroupIdentifier other)
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

         return GroupId == other.GroupId && SiteCollectionId.Equals(other.SiteCollectionId);
      }


      public override bool Equals(object obj)
      {
         return this.Equals(obj as SpFarmGroupIdentifier);
      }


      public static bool operator ==(SpFarmGroupIdentifier lhs, SpFarmGroupIdentifier rhs)
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


      public static bool operator !=(SpFarmGroupIdentifier lhs, SpFarmGroupIdentifier rhs)
      {
         return !(lhs == rhs);
      }


      public override int GetHashCode()
      {
         return GroupId.GetHashCode() ^ SiteCollectionId.GetHashCode();
      }

   }
}
