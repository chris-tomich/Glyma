using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glyma.SharePoint.Search
{
   public class SpSiteCollectionConnectionSetting : RepositoryConnectionSetting
   {

      public SpSiteCollectionConnectionSetting(Guid siteCollectionId)
      {
         this.Add(ConnectionSettingProperty.SiteCollectionId, siteCollectionId);
         Validate();
      }


      public SpSiteCollectionConnectionSetting(string url)
      {
         this.Add(ConnectionSettingProperty.SiteCollectionUrl, url);
         Validate();
      }


      public SpSiteCollectionConnectionSetting(RepositoryConnectionSetting connection)
      {
         if (connection.ContainsKey(ConnectionSettingProperty.SiteCollectionId))
         {
            Guid siteCollectionId = (System.Guid)connection[ConnectionSettingProperty.SiteCollectionId];
            this.Add(ConnectionSettingProperty.SiteCollectionId, siteCollectionId);
         }
         if (connection.ContainsKey(ConnectionSettingProperty.SiteCollectionUrl))
         {
            string siteCollectionUrl = (string)connection[ConnectionSettingProperty.SiteCollectionUrl];
            this.Add(ConnectionSettingProperty.SiteCollectionUrl, siteCollectionUrl);
         }
         Validate();
      }


      public Guid SiteCollectionId
      {
         get
         {
            Guid siteCollectionId = Guid.Empty;
            if (this.ContainsKey(ConnectionSettingProperty.SiteCollectionId))
            {
               siteCollectionId = (System.Guid)this[ConnectionSettingProperty.SiteCollectionId];
            }
            return siteCollectionId;
         }
      }


      public string SiteCollectionUrl
      {
         get
         {
            string siteCollectionUrl = string.Empty;
            if (this.ContainsKey(ConnectionSettingProperty.SiteCollectionUrl))
            {
               siteCollectionUrl = (string)this[ConnectionSettingProperty.SiteCollectionUrl];
            }
            return siteCollectionUrl;
         }
      }


      public override bool IsValid()
      {
         return !(SiteCollectionId.Equals(Guid.Empty) && string.IsNullOrEmpty(SiteCollectionUrl));
      }


      public override void Validate()
      {
         if (!IsValid())
         {
            throw new ApplicationException("A valid site collection ID or URL has not been provided.");
         }
      }
   }
}
