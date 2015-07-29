using SilverlightMappingToolBasic.UI.SuperGraph.View.MessageBox;
using System;
using System.Security;
using System.Windows;
using System.Windows.Browser;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.ContextMenu
{
    public static class ShareWithUrlFactory
    {
        public static void Share(this ShareWithType type, string url, string name)
        {
            Uri uri;
            switch (type)
            {
                case ShareWithType.Clipboard:
                    try
                    {
                        Clipboard.SetText(url);
                    }
                    catch (SecurityException se)
                    {
                        SuperMessageBoxService.ShowError("Clipboard Access Denied", 
                            "Access to the clipboard was denied, this is configured in the Silverlight plugin under the 'Permissions' tab.");
                    }
                    break;
                case ShareWithType.Facebook:
                    var facebookUrl = string.Format(
                            "http://www.facebook.com/sharer/sharer.php?s=100&p[url]={0}&p[title]={1}&p[summary]={2}"
                            , HttpUtility.UrlEncode(url), "Glyma Map", name);
                    if (Uri.TryCreate(facebookUrl, UriKind.RelativeOrAbsolute, out uri))
                    {
                        HtmlPage.Window.Navigate(uri, "_blank");
                    }
                    break;
                case ShareWithType.Email:
                    var emailLink = string.Format("mailto:?subject={0}&body={1}", ("Glyma Map - " + name).Replace(" ", "%20"), HttpUtility.UrlEncode(url));
                    if (Uri.TryCreate(emailLink, UriKind.RelativeOrAbsolute, out uri))
                    {
                        HtmlPage.Window.Navigate(uri, "_blank");
                    }
                    break;
                case ShareWithType.Twitter:
                    var twitter = string.Format("http://twitter.com/home?status={0}", HttpUtility.UrlEncode(url));
                    if (Uri.TryCreate(twitter, UriKind.RelativeOrAbsolute, out uri))
                    {
                        HtmlPage.Window.Navigate(uri, "_blank");
                    }
                    break;
                case ShareWithType.GooglePlus:
                    var googlePlus = string.Format("https://plus.google.com/share?url={0}", HttpUtility.UrlEncode(url));
                    if (Uri.TryCreate(googlePlus, UriKind.RelativeOrAbsolute, out uri))
                    {
                        HtmlPage.Window.Navigate(uri, "_blank");
                    }
                    break;
                case ShareWithType.LinkedIn:
                    var linkedIn = string.Format("http://www.linkedin.com/shareArticle?mini=true&url={0}&source={0}&title=Glyma%20Map&summary={1}", HttpUtility.UrlEncode(url), name);
                    if (Uri.TryCreate(linkedIn, UriKind.RelativeOrAbsolute, out uri))
                    {
                        HtmlPage.Window.Navigate(uri, "_blank");
                    }
                    break;
                case ShareWithType.Yammer:
                    var yammer = string.Format("https://www.yammer.com/messages/new?login=true&trk_event=yammer_share&status=%20{0}", HttpUtility.UrlEncode(HttpUtility.UrlEncode(url)));
                    if (Uri.TryCreate(yammer, UriKind.RelativeOrAbsolute, out uri))
                    {
                        HtmlPage.Window.Navigate(uri, "_blank");
                    }
                    break;
            }
        }
    }
}
