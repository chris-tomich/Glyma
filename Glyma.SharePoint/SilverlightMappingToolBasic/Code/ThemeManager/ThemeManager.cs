using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Reflection;
using System.Collections.Generic;
using System.Xml;
using System.IO;

using SilverlightMappingToolBasic.ThemeService;
using System.ServiceModel;
using ViewModel = SilverlightMappingToolBasic.UI.ViewModel;
using Proxy = TransactionalNodeService.Proxy;
using System.Windows.Media.Imaging;

namespace SilverlightMappingToolBasic
{
    public class ThemeManager : IThemeManager
    {
        #region Test XML Theme
        private const string TestThemeConfig =
@"<MappingToolConfig>
    <Themes>
        <Theme Name='Generic' Assembly=''>
            <Skin Name='Focal' NodeType='CompendiumGenericNode' Class='SilverlightMappingToolBasic.SimpleNodeSkin' Assembly=''>
                <Property Name='TextMarginX'>-15</Property>
                <Property Name='TextMarginY'>128</Property>
                <Property Name='TextWidth'>159</Property>
                <Property Name='TextHeight'>94</Property>
                <Property Name='ImageUrl'>http://localhost/SilverlightMappingTool/Images/generic.png</Property>
                <Property Name='NodeSkinWidth'>125</Property>
                <Property Name='NodeSkinHeight'>125</Property>
            </Skin>
            <Skin Name='Default' NodeType='CompendiumGenericNode' Class='SilverlightMappingToolBasic.SimpleNodeSkin' Assembly=''>
                <Property Name='TextMarginX'>-45</Property>
                <Property Name='TextMarginY'>60</Property>
                <Property Name='TextWidth'>140</Property>
                <Property Name='TextHeight'>50</Property>
                <Property Name='ImageUrl'>http://localhost/SilverlightMappingTool/Images/generic.png</Property>
                <Property Name='NodeSkinWidth'>50</Property>
                <Property Name='NodeSkinHeight'>50</Property>
            </Skin>
            <Skin Name='Focal' NodeType='CompendiumDecisionNode' Class='SilverlightMappingToolBasic.SimpleNodeSkin' Assembly=''>
                <Property Name='TextMarginX'>-15</Property>
                <Property Name='TextMarginY'>128</Property>
                <Property Name='TextWidth'>159</Property>
                <Property Name='TextHeight'>94</Property>
                <Property Name='ImageUrl'>http://localhost/SilverlightMappingTool/Images/generic.png</Property>
                <Property Name='NodeSkinWidth'>125</Property>
                <Property Name='NodeSkinHeight'>125</Property>
            </Skin>
            <Skin Name='Default' NodeType='CompendiumDecisionNode' Class='SilverlightMappingToolBasic.SimpleNodeSkin' Assembly=''>
                <Property Name='TextMarginX'>-45</Property>
                <Property Name='TextMarginY'>60</Property>
                <Property Name='TextWidth'>140</Property>
                <Property Name='TextHeight'>50</Property>
                <Property Name='ImageUrl'>http://localhost/SilverlightMappingTool/Images/generic.png</Property>
                <Property Name='NodeSkinWidth'>50</Property>
                <Property Name='NodeSkinHeight'>50</Property>
            </Skin>
            <Skin Name='Focal' NodeType='CompendiumConNode' Class='SilverlightMappingToolBasic.SimpleNodeSkin' Assembly=''>
                <Property Name='TextMarginX'>-15</Property>
                <Property Name='TextMarginY'>128</Property>
                <Property Name='TextWidth'>159</Property>
                <Property Name='TextHeight'>94</Property>
                <Property Name='ImageUrl'>http://localhost/SilverlightMappingTool/Images/minus.png</Property>
                <Property Name='NodeSkinWidth'>125</Property>
                <Property Name='NodeSkinHeight'>125</Property>
            </Skin>
            <Skin Name='Default' NodeType='CompendiumConNode' Class='SilverlightMappingToolBasic.SimpleNodeSkin' Assembly=''>
                <Property Name='TextMarginX'>-45</Property>
                <Property Name='TextMarginY'>60</Property>
                <Property Name='TextWidth'>140</Property>
                <Property Name='TextHeight'>50</Property>
                <Property Name='ImageUrl'>http://localhost/SilverlightMappingTool/Images/minus.png</Property>
                <Property Name='NodeSkinWidth'>50</Property>
                <Property Name='NodeSkinHeight'>50</Property>
            </Skin>
            <Skin Name='Focal' NodeType='CompendiumIdeaNode' Class='SilverlightMappingToolBasic.SimpleNodeSkin' Assembly=''>
                <Property Name='TextMarginX'>-15</Property>
                <Property Name='TextMarginY'>128</Property>
                <Property Name='TextWidth'>159</Property>
                <Property Name='TextHeight'>94</Property>
                <Property Name='ImageUrl'>http://localhost/SilverlightMappingTool/Images/exclamation.png</Property>
                <Property Name='NodeSkinWidth'>125</Property>
                <Property Name='NodeSkinHeight'>125</Property>
            </Skin>
            <Skin Name='Default' NodeType='CompendiumIdeaNode' Class='SilverlightMappingToolBasic.SimpleNodeSkin' Assembly=''>
                <Property Name='TextMarginX'>-45</Property>
                <Property Name='TextMarginY'>60</Property>
                <Property Name='TextWidth'>140</Property>
                <Property Name='TextHeight'>50</Property>
                <Property Name='ImageUrl'>http://localhost/SilverlightMappingTool/Images/exclamation.png</Property>
                <Property Name='NodeSkinWidth'>50</Property>
                <Property Name='NodeSkinHeight'>50</Property>
            </Skin>
            <Skin Name='Focal' NodeType='CompendiumMapNode' Class='SilverlightMappingToolBasic.SimpleNodeSkin' Assembly=''>
                <Property Name='TextMarginX'>-15</Property>
                <Property Name='TextMarginY'>128</Property>
                <Property Name='TextWidth'>159</Property>
                <Property Name='TextHeight'>94</Property>
                <Property Name='ImageUrl'>http://localhost/SilverlightMappingTool/Images/network.png</Property>
                <Property Name='NodeSkinWidth'>125</Property>
                <Property Name='NodeSkinHeight'>125</Property>
            </Skin>
            <Skin Name='Default' NodeType='CompendiumMapNode' Class='SilverlightMappingToolBasic.SimpleNodeSkin' Assembly=''>
                <Property Name='TextMarginX'>-45</Property>
                <Property Name='TextMarginY'>60</Property>
                <Property Name='TextWidth'>140</Property>
                <Property Name='TextHeight'>50</Property>
                <Property Name='ImageUrl'>http://localhost/SilverlightMappingTool/Images/network.png</Property>
                <Property Name='NodeSkinWidth'>50</Property>
                <Property Name='NodeSkinHeight'>50</Property>
            </Skin>
            <Skin Name='Focal' NodeType='CompendiumProNode' Class='SilverlightMappingToolBasic.SimpleNodeSkin' Assembly=''>
                <Property Name='TextMarginX'>-15</Property>
                <Property Name='TextMarginY'>128</Property>
                <Property Name='TextWidth'>159</Property>
                <Property Name='TextHeight'>94</Property>
                <Property Name='ImageUrl'>http://localhost/SilverlightMappingTool/Images/plus.png</Property>
                <Property Name='NodeSkinWidth'>125</Property>
                <Property Name='NodeSkinHeight'>125</Property>
            </Skin>
            <Skin Name='Default' NodeType='CompendiumProNode' Class='SilverlightMappingToolBasic.SimpleNodeSkin' Assembly=''>
                <Property Name='TextMarginX'>-45</Property>
                <Property Name='TextMarginY'>60</Property>
                <Property Name='TextWidth'>140</Property>
                <Property Name='TextHeight'>50</Property>
                <Property Name='ImageUrl'>http://localhost/SilverlightMappingTool/Images/plus.png</Property>
                <Property Name='NodeSkinWidth'>50</Property>
                <Property Name='NodeSkinHeight'>50</Property>
            </Skin>
            <Skin Name='Focal' NodeType='CompendiumQuestionNode' Class='SilverlightMappingToolBasic.SimpleNodeSkin' Assembly=''>
                <Property Name='TextMarginX'>-15</Property>
                <Property Name='TextMarginY'>128</Property>
                <Property Name='TextWidth'>159</Property>
                <Property Name='TextHeight'>94</Property>
                <Property Name='ImageUrl'>http://localhost/SilverlightMappingTool/Images/question.png</Property>
                <Property Name='NodeSkinWidth'>125</Property>
                <Property Name='NodeSkinHeight'>125</Property>
            </Skin>
            <Skin Name='Default' NodeType='CompendiumQuestionNode' Class='SilverlightMappingToolBasic.SimpleNodeSkin' Assembly=''>
                <Property Name='TextMarginX'>-45</Property>
                <Property Name='TextMarginY'>60</Property>
                <Property Name='TextWidth'>140</Property>
                <Property Name='TextHeight'>50</Property>
                <Property Name='ImageUrl'>http://localhost/SilverlightMappingTool/Images/question.png</Property>
                <Property Name='NodeSkinWidth'>50</Property>
                <Property Name='NodeSkinHeight'>50</Property>
            </Skin>
            <Skin Name='Focal' NodeType='DomainNode' Class='SilverlightMappingToolBasic.SimpleNodeSkin' Assembly=''>
                <Property Name='TextMarginX'>-15</Property>
                <Property Name='TextMarginY'>128</Property>
                <Property Name='TextWidth'>159</Property>
                <Property Name='TextHeight'>94</Property>
                <Property Name='ImageUrl'>http://localhost/SilverlightMappingTool/Images/network.png</Property>
                <Property Name='NodeSkinWidth'>125</Property>
                <Property Name='NodeSkinHeight'>125</Property>
            </Skin>
            <Skin Name='Default' NodeType='DomainNode' Class='SilverlightMappingToolBasic.SimpleNodeSkin' Assembly=''>
                <Property Name='TextMarginX'>-45</Property>
                <Property Name='TextMarginY'>60</Property>
                <Property Name='TextWidth'>140</Property>
                <Property Name='TextHeight'>50</Property>
                <Property Name='ImageUrl'>http://localhost/SilverlightMappingTool/Images/network.png</Property>
                <Property Name='NodeSkinWidth'>50</Property>
                <Property Name='NodeSkinHeight'>50</Property>
            </Skin>
        </Theme>
    </Themes>
</MappingToolConfig>";
        #endregion

        private string _currentThemeName;
        private Dictionary<string, Dictionary<INodeTypeProxy, Dictionary<string, INodeSkin>>> _themes;
        private Dictionary<string, Theme> _themes2;
        private Dictionary<Guid, ImageSource> _skinImages;

        public ThemeManager()
        {
            _themes = new Dictionary<string, Dictionary<INodeTypeProxy, Dictionary<string, INodeSkin>>>(); // Theme, NodeTypeProxy, SkinName
            _themes2 = new Dictionary<string, Theme>();
        }

        public event EventHandler ThemeLoaded;

        private void LoadXml(XmlReader xmlReader)
        {
            while (xmlReader.ReadToFollowing("Theme"))
            {
                string themeName = xmlReader.GetAttribute("Name");
                string themeAssembly = xmlReader.GetAttribute("Assembly");

                Dictionary<INodeTypeProxy, Dictionary<string, INodeSkin>> theme;

                if (_themes.ContainsKey(themeName))
                {
                    theme = _themes[themeName];
                }
                else
                {
                    theme = new Dictionary<INodeTypeProxy, Dictionary<string, INodeSkin>>();

                    _themes.Add(themeName, theme);
                }

                while (!(xmlReader.Name == "Theme" && xmlReader.NodeType == XmlNodeType.EndElement) && xmlReader.Read())
                {
                    if (xmlReader.Name == "Skin" && xmlReader.NodeType == XmlNodeType.Element)
                    {
                        string skinNameXmlValue = null;
                        string skinClassXmlValue = null;
                        string skinAssemblyXmlValue = null;
                        string skinNodeTypeXmlValue = null;
                        INodeSkin skin = null;

                        if (xmlReader.NodeType == XmlNodeType.Element)
                        {
                            skinNameXmlValue = xmlReader.GetAttribute("Name");
                            skinClassXmlValue = xmlReader.GetAttribute("Class");
                            skinAssemblyXmlValue = xmlReader.GetAttribute("Assembly");
                            skinNodeTypeXmlValue = xmlReader.GetAttribute("NodeType");
                        }

                        Dictionary<string, INodeSkin> nodeTypes;
                        INodeTypeProxy nodeType = IoC.IoCContainer.GetInjectionInstance().GetInstance<TypeManager>().GetNodeType(skinNodeTypeXmlValue);

                        if (nodeType != null && theme.ContainsKey(nodeType))
                        {
                            nodeTypes = theme[nodeType];
                        }
                        else
                        {
                            nodeTypes = new Dictionary<string, INodeSkin>();

                            theme.Add(nodeType, nodeTypes);
                        }

                        if (!string.IsNullOrEmpty(skinClassXmlValue) && !string.IsNullOrEmpty(skinNodeTypeXmlValue))
                        {
                            object skinObject = Assembly.GetExecutingAssembly().CreateInstance(skinClassXmlValue);

                            if (skinObject is INodeSkin)
                            {
                                skin = skinObject as INodeSkin;

                                if (!nodeTypes.ContainsKey(skinNameXmlValue))
                                {
                                    nodeTypes.Add(skinNameXmlValue, skin);
                                }
                            }
                            else if (skinObject == null)
                            {
                                throw new NotSupportedException("The provided theme does not exist.");
                            }
                            else
                            {
                                throw new NotSupportedException("The provided theme is not a skin.");
                            }
                        }

                        while (!(xmlReader.Name == "Skin" && xmlReader.NodeType == XmlNodeType.EndElement))
                        {
                            if (xmlReader.Name == "Property" && xmlReader.NodeType == XmlNodeType.Element)
                            {
                                string stylePropertyName = xmlReader.GetAttribute("Name");

                                string stylePropertyValue = xmlReader.ReadElementContentAsString();
                                stylePropertyValue = stylePropertyValue.Trim();

                                skin.SkinProperties[stylePropertyName] = stylePropertyValue;
                            }
                            else
                            {
                                xmlReader.Read();
                            }
                        }
                    }
                    else
                    {
                        xmlReader.Read();
                    }
                }
            }
        }

        #region IThemeManager Members

        public void LoadTheme(string themeSvcUrl, string themeName)
        {
            _currentThemeName = themeName;
            ThemeServiceClient client = null;
            if (string.IsNullOrEmpty(themeSvcUrl))
            {
                client = new ThemeServiceClient();
            }
            else
            {
                BasicHttpBinding binding = new BasicHttpBinding();
                binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;

                EndpointAddress address = new EndpointAddress(new Uri(themeSvcUrl));
                client = new ThemeServiceClient(binding, address);
            }
            client.GetThemeCompleted += new EventHandler<GetThemeCompletedEventArgs>(client_GetThemeCompleted);
            client.GetThemeAsync(themeName);
        }

        private void client_GetThemeCompleted(object sender, GetThemeCompletedEventArgs e)
        {
            if (e.Error == null) 
            {
                ThemeResult themeResult = e.Result;
                if (!_themes2.ContainsKey(themeResult.Theme.Name))
                {
                    _themes2.Add(themeResult.Theme.Name, themeResult.Theme);
                }
                if (ThemeLoaded != null)
                {
                    ThemeLoaded.Invoke(this, new EventArgs());
                }
            }
        }

        public INodeSkin GetSkin(INodeTypeProxy nodeTypeProxy, string skinName)
        {
            //Dictionary<INodeTypeProxy, Dictionary<string, INodeSkin>> nodeTypes = _themes[_currentThemeName];
            //Dictionary<string, INodeSkin> nodeSkins = nodeTypes[nodeTypeProxy];

            foreach (ThemeSkin skin in _themes2[_currentThemeName].Skin)
            {
                if (skin.Name == skinName && skin.NodeType == nodeTypeProxy.Name)
                {
                    object skinObject = Assembly.GetExecutingAssembly().CreateInstance(skin.Class);
                    if (skinObject is INodeSkin)
                    {
                        INodeSkin skinObj = skinObject as INodeSkin;
                        foreach (ThemeSkinProperty skinProperty in skin.Property)
                        {
                            skinObj.SkinProperties[skinProperty.Name] = skinProperty.Value;
                        }
                        return skinObj;
                    }
                }
            }
            return null;
            //INodeSkin nodeSkin = nodeSkins[skinName];

           // return nodeSkin;
        }

        #endregion

        #region New Skinning Operations

        private Dictionary<Guid, ImageSource> SkinImages
        {
            get
            {
                if (_skinImages == null)
                {
                    _skinImages = new Dictionary<Guid, ImageSource>();
                }

                return _skinImages;
            }
        }

        private string GetSkinImageUri(Guid nodeType)
        {
            if (nodeType == new Guid("3B53600F-39EC-42FB-B08A-325062037130"))
            {
                // Idea Node
                return App.Params.GlymaIconLibraryUrl + "/idea.png";
            }

            if (nodeType == new Guid("99FD1475-8099-45D3-BEDF-BEC396CCB4DD"))
            {
                // Question Node
                return App.Params.GlymaIconLibraryUrl + "/question.png";
            }

            if (nodeType == new Guid("B8C354CB-C7D0-4982-9A0F-6C4368FAB749"))
            {
                // Map Node
                return App.Params.GlymaIconLibraryUrl + "/map.png";
            }

            if (nodeType == new Guid("084F38B7-115F-4AF6-9E30-D9D91226F86B"))
            {
                // Pro Node
                return App.Params.GlymaIconLibraryUrl + "/pro.png";
            }

            if (nodeType == new Guid("DA66B603-F6B3-4ECF-8ED0-AB34A288CF08"))
            {
                // Con Node
                return App.Params.GlymaIconLibraryUrl + "/cons.png";
            }

            if (nodeType == new Guid("53EC78E3-F189-4340-B251-AAF9D78CF56D"))
            {
                // Decision Node
                return App.Params.GlymaIconLibraryUrl + "/decision.png";
            }
            // Generic Node
            return App.Params.GlymaIconLibraryUrl + "/note.png";
        }

        public void DressNode(ViewModel.ISkinnableNode skinnableNode)
        {
            ViewModel.INode viewModelNode = skinnableNode as ViewModel.INode;

            if (viewModelNode != null)
            {
                ImageSource skinImage;

                if (SkinImages.ContainsKey(viewModelNode.MapObjectType))
                {
                    skinImage = SkinImages[viewModelNode.MapObjectType];
                }
                else
                {
                    string skinImageAddress = GetSkinImageUri(viewModelNode.MapObjectType);
                    Uri skinImageUri = new Uri(skinImageAddress);

                    skinImage = new BitmapImage(skinImageUri);
                    SkinImages[viewModelNode.MapObjectType] = skinImage;
                }

                skinnableNode.NodeImage = skinImage;
            }
        }

        #endregion
    }

//    public class ThemeManager<Theme>
//    {
//        private const string TestThemeConfig =
//@"<MappingToolConfig>
//    <Themes>
//        <Theme Name='' Assembly=''>
//            <Skin Class='' Namespace='' Assembly=''>
//                <Property Name=''>
//                </Property>
//                <Property Name=''>
//                </Property>
//                <Property Name=''>
//                </Property>
//            </Skin>
//            <Skin Class='' Namespace=''>
//                <Property Name=''>
//                </Property>
//                <Property Name=''>
//                </Property>
//                <Property Name=''>
//                </Property>
//            </Skin>
//            <Skin Class='' Namespace=''>
//                <Property Name=''>
//                </Property>
//                <Property Name=''>
//                </Property>
//                <Property Name=''>
//                </Property>
//            </Skin>
//        </Theme>
//        <Theme Name='Default' Namespace='SilverlightMappingToolBasic.SingleDepth' >
//            <Style Name='Details'>
//                <Property Name='SomeDetails1'>
//                    Some Details 1
//                </Property>
//            </Style>
//            <Style Name='ExtraDetails'>
//                <Property Name='SomeExtraDetails1'>
//                    SomeExtraDetails1
//                </Property>
//                <Property Name='SomeExtraDetails2'>
//                    SomeExtraDetails2
//                </Property>
//            </Style>
//        </Theme>
//    </Themes>
//</MappingToolConfig>";

//        private Dictionary<string, Dictionary<string, string>> _themeStyles = new Dictionary<string, Dictionary<string, string>>();

//        public ThemeManager()
//        {
//        }

//        public ThemeManager(string themeName, string themeNamespace)
//        {
//            XmlReaderSettings settings = new XmlReaderSettings();
//            settings.IgnoreComments = true;
//            settings.IgnoreWhitespace = true;

//            StringReader stringReader = new StringReader(TestThemeConfig);
//            XmlReader xmlReader = XmlReader.Create(stringReader, settings);

//            LoadXml(themeName, themeNamespace, xmlReader);
//            LoadTheme();
//        }

//        #region Static Methods

//        public static ThemeManager<Theme> CurrentTheme()
//        {
//            return new ThemeManager<Theme>("Default", "SilverlightMappingToolBasic.SingleDepth");
//        }

//        #endregion

//        private void LoadXml(string themeName, string themeNamespace, XmlReader xmlReader)
//        {
//            bool isLoaded = false;

//            while (xmlReader.ReadToFollowing("Theme") && !isLoaded)
//            {
//                string currentThemeName = xmlReader.GetAttribute("Name");
//                string currentThemeNamespace = xmlReader.GetAttribute("Namespace");

//                if (currentThemeName == themeName && currentThemeNamespace == themeNamespace)
//                {
//                    isLoaded = true;

//                    while (!(xmlReader.Name == "Theme" && xmlReader.NodeType == XmlNodeType.EndElement) && xmlReader.Read())
//                    {
//                        if (xmlReader.Name == "Style" && xmlReader.NodeType == XmlNodeType.Element)
//                        {
//                            string styleName = null;

//                            if (xmlReader.NodeType == XmlNodeType.Element)
//                            {
//                                styleName = xmlReader.GetAttribute("Name");
//                            }

//                            while (!(xmlReader.Name == "Style" && xmlReader.NodeType == XmlNodeType.EndElement))
//                            {
//                                if (xmlReader.Name == "Property" && xmlReader.NodeType == XmlNodeType.Element)
//                                {
//                                    string stylePropertyName = xmlReader.GetAttribute("Name");

//                                    string stylePropertyValue = xmlReader.ReadElementContentAsString();
//                                    stylePropertyValue = stylePropertyValue.Trim();

//                                    if (!_themeStyles.ContainsKey(styleName))
//                                    {
//                                        _themeStyles[styleName] = new Dictionary<string, string>();
//                                    }

//                                    _themeStyles[styleName][stylePropertyName] = stylePropertyValue;
//                                }
//                                else
//                                {
//                                    xmlReader.Read();
//                                }
//                            }
//                        }
//                        else
//                        {
//                            xmlReader.Read();
//                        }
//                    }
//                }
//            }
//        }

//        private void LoadTheme()
//        {
//            Type themeType = typeof(Theme);

//            PropertyInfo[] themeProperties = themeType.GetProperties();

//            foreach (PropertyInfo themePropertyInfo in themeProperties)
//            {
//                object[] unknownStyleAttributes = themePropertyInfo.GetCustomAttributes(false);

//                foreach (object unknownStyleAttribute in unknownStyleAttributes)
//                {
//                    if (unknownStyleAttribute is ThemeStyleAttribute)
//                    {
//                        ThemeStyleAttribute styleAttribute = (ThemeStyleAttribute)unknownStyleAttribute;
//                        object themeStyle = themePropertyInfo.GetValue(this, null);

//                        if (themeStyle == null)
//                        {
//                            ConstructorInfo themePropertyConstructorInfo = themePropertyInfo.PropertyType.GetConstructor(Type.EmptyTypes);

//                            if (themePropertyConstructorInfo != null)
//                            {
//                                themeStyle = themePropertyConstructorInfo.Invoke(null);
//                                themePropertyInfo.SetValue(this, themeStyle, null);
//                            }
//                            else
//                            {
//                                throw new NotSupportedException("The provided style is not supported by the theme manager. The style must have a default constructor.");
//                            }
//                        }

//                        Type themeStyleType = themeStyle.GetType();
//                        PropertyInfo[] styleProperties = themeStyleType.GetProperties();

//                        Dictionary<string, string> themeStyleProperties = new Dictionary<string, string>();

//                        foreach (PropertyInfo stylePropertyInfo in styleProperties)
//                        {
//                            object[] unknownPropertyAttributes = stylePropertyInfo.GetCustomAttributes(false);

//                            foreach (object unknownPropertyAttribute in unknownPropertyAttributes)
//                            {
//                                if (unknownPropertyAttribute is ThemePropertyAttribute)
//                                {
//                                    ThemePropertyAttribute themePropertyAttribute = (ThemePropertyAttribute)unknownPropertyAttribute;

//                                    string stylePropertyValue = _themeStyles[styleAttribute.StyleKeyName][themePropertyAttribute.PropertyKeyName];

//                                    stylePropertyInfo.SetValue(themeStyle, stylePropertyValue, null);
//                                }
//                            }
//                        }
//                    }
//                }
//            }
//        }
//    }
}
