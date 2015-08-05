using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;

namespace Glyma.SharePoint.Search.UI
{
   public class SearchCssInjector : Control
   {
      protected override void CreateChildControls()
      {
         try
         {
            if (SPContext.Current == null || SPContext.Current.Site == null)
            {
               throw new ApplicationException("Unable to get the details of the current context.");
            }
            base.CreateChildControls();
            Controls.Add(new LiteralControl("<link rel=\"stylesheet\" type=\"text/css\" href=\"" + SPContext.Current.Site.ServerRelativeUrl.TrimEnd('/') + "/Style Library/Glyma/Search/Css/GlymaSearchResults.css\" />"));
         }
         catch (Exception currentException)
         {
            Controls.Add(new LiteralControl("The following exception has occurred:" + currentException.ToString()));
         }
      }
   }
}
