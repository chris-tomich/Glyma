using System;
using System.Collections.Generic;
using System.Windows.Browser;
using TransactionalNodeService.Proxy;

namespace SilverlightMappingToolBasic.UI.Extensions.CookieManagement
{
    /// <summary>
    ///         Examples to write to cookie.
    ///        CookieManager.Write("NodeUid",nodeId.ToString(),2);
    ///        CookieManager.Write("DomainUid", domainId.ToString(), 2);
    /// </summary>
    public static class CookieManager
    {
        private static Dictionary<Guid, MapInformation> _mapInfo;
        public static Dictionary<Guid, MapInformation> MapInfo
        {
            get
            {
                if (_mapInfo == null)
                {
                    _mapInfo = new Dictionary<Guid, MapInformation>();
                }
                return _mapInfo;
            }
        }

        public static MapInformation GetMapInfo(Guid mapId)
        {
            if(!MapInfo.ContainsKey(mapId))
            {
                var info = Read(mapId.ToString());
                SetMapInfo(mapId, info != null ? new MapInformation(mapId, info) : new MapInformation(mapId));
            }
            return MapInfo[mapId];
        }

        public static void SetMapInfo(Guid mapId, MapInformation mapInfo)
        {
            if (MapInfo.ContainsKey(mapId))
            {
                MapInfo[mapId] = mapInfo;
            }
            else
            {
                MapInfo.Add(mapId, mapInfo);
            }
        }

        public static void Save()
        {
            foreach (var keyPair in MapInfo)
            {
                Write(keyPair.Key.ToString(), keyPair.Value.ToString(), 1);
            }
        }


        public static bool Exists(string key, string value)
        {
            return HtmlPage.Document.Cookies.Contains(key + "=" + value);
        }

        public static string Read(string key)
        {
            string[] cookies = HtmlPage.Document.Cookies.Split(';');
            foreach (string cookie in cookies)
            {
                string[] keyValuePair = cookie.Split('=');
                if (keyValuePair.Length == 2 && key == keyValuePair[0].Trim())
                    return keyValuePair[1].Trim();
            }

            return null;
        }

        public static void Write(string key, string value, int expireDays)
        {
            // expireDays = 0, indicates a session cookie that will not be written to disk 
            // expireDays = -1, indicates that the cookie will not expire and will be permanent
            // expireDays = n, indicates that the cookie will expire in “n” days
            string expires = "";
            if (expireDays != 0)
            {
                DateTime expireDate = (expireDays > 0 ?
                DateTime.Now + TimeSpan.FromDays(expireDays) :
                DateTime.MaxValue);
                expires = ";path=/;expires=" + expireDate.ToString("R");
            }

            string cookie = key + "=" + value + expires;
            HtmlPage.Document.SetProperty("cookie", cookie);
        }

        public static void Delete(Guid mapId)
        {
            if (MapInfo.ContainsKey(mapId))
            {
                MapInfo.Remove(mapId);
            }
            DateTime expireDate = DateTime.Now - TimeSpan.FromDays(1); // yesterday
            string expires = ";path=/;expires=" + expireDate.ToString("R");
            string cookie = mapId + "=" + expires;
            HtmlPage.Document.SetProperty("cookie", cookie);
        }

        public static void Delete(string key)
        {
            DateTime expireDate = DateTime.Now - TimeSpan.FromDays(1); // yesterday
            string expires = ";path=/;expires=" + expireDate.ToString("R");
            string cookie = key + "=" + expires;
            HtmlPage.Document.SetProperty("cookie", cookie);
        }

        public static Dictionary<string, string> ReadAll()
        {
            var output = new Dictionary<string, string>();
            string[] cookies = HtmlPage.Document.Cookies.Split(';');
            foreach (string cookie in cookies)
            {
                string[] keyValuePair = cookie.Split('=');
                if (keyValuePair.Length == 2)
                {
                    output.Add(keyValuePair[0].Trim(), keyValuePair[1].Trim());
                }
            }
            return output;
        }
    }
}
