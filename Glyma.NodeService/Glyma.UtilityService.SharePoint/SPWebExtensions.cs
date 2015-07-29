using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glyma.UtilityService.SharePoint
{
    public static class SPWebExtensions
    {
        /// <summary>
        /// Returns the server relative URL prefix for lists within a site.
        /// </summary>
        /// <param name="site">The site to use.</param>
        /// <returns>A string representing the server relative URL prefix for lists within the site.</returns>
        public static string GetServerRelativeListUrlPrefix(this SPWeb site)
        {
            string listServerRelativeUrlPrefix = site.ServerRelativeUrl.TrimEnd('/') + "/Lists/";
            return listServerRelativeUrlPrefix;
        }

        /// <summary>
        /// Return a reference to a list within the site using its server-relative URL.
        /// </summary>
        /// <param name="site">The site to retrieve the list from.</param>
        /// <param name="serverRelativeUrl">The server relative URL of the list.</param>
        /// <returns>A SPList object representing the list if it found; otherwise, null.</returns>
        public static SPList TryGetList(this SPWeb site, string serverRelativeUrl)
        {
            SPList referenceList = null;

            // Check if the list exists using its URL instead of its Title.
            try
            {
                referenceList = site.GetList(serverRelativeUrl);
            }
            catch (System.IO.FileNotFoundException)
            {
                // The list doesn't exist.
            }
            return referenceList;
        }
    }
}
