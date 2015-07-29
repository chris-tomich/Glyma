using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.IO;
using System.Xml;
using System.Net;
using System.ServiceModel.Activation;
using System.Web.Hosting;
using System.Xml.Serialization;

using Microsoft.SharePoint.Client.Services;
using Microsoft.SharePoint.Utilities;

namespace ThemeService
{
    /// <summary>
    /// This is set to handle multiple threads but also be a single instance, this means the file is read once on startup
    /// </summary>
    [BasicHttpBindingServiceMetadataExchangeEndpointAttribute]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(ConcurrencyMode=ConcurrencyMode.Multiple, InstanceContextMode=InstanceContextMode.Single, Namespace="http://sevensigma.com.au/ThemeService")]
    public class ThemeService : IThemeService
    {
        private static MappingToolConfig _loadedConfig;
        private object _lockObject = new object();
        private bool _loaded;

        private ThemeService()
        {
            lock (_lockObject)
            {
                if (!_loaded)
                {
                    XmlReader xmlReader = null;
                    try
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(MappingToolConfig));
                        string path = SPUtility.GetGenericSetupPath(@"ISAPI\SevenSigma\Configs\DefaultConfig.xml");
                        xmlReader = XmlTextReader.Create(path);
                        MappingToolConfig config = serializer.Deserialize(xmlReader) as MappingToolConfig;

                        //re-read to manually find the context menu sections since 
                        //they aren't picked up by thedeserialization process.
                        xmlReader = XmlReader.Create(path);
                        bool foundContextMenus = xmlReader.ReadToFollowing("ContextMenus");
                        if (foundContextMenus)
                        {
                            while (xmlReader.ReadToFollowing("ContextMenu"))
                            {
                                string contextMenuName = xmlReader.GetAttribute("Name");
                                ContextMenu cm = config.GetContextMenuByName(contextMenuName);
                                if (cm != null)
                                {
                                    cm.ContextMenuXaml = xmlReader.ReadInnerXml();
                                }
                            }
                        }
                        _loadedConfig = config;
                        _loaded = true;
                    }
                    catch (Exception)
                    {
                        _loaded = false;
                    }
                    finally
                    {
                        if (xmlReader != null) 
                            xmlReader.Close();
                    }
                }
            }
        }

        public ThemeResult GetTheme(string name)
        {
            ThemeResult result = new ThemeResult();
            Theme theme = _loadedConfig.GetThemeByName(name);
            if (theme != null)
            {
                result.Success = true;
                result.Theme = theme;
            }
            return result;
        }

        public string GetContextMenuXaml(string name)
        {
            ContextMenu contextMenu = _loadedConfig.GetContextMenuByName(name);
            if (contextMenu != null)
            {
                return contextMenu.ContextMenuXaml;
            }
            else
            {
                return null;
            }
        }
    }
}
