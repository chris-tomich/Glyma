using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Reflection;

using SilverlightMappingToolBasic.Controls;

namespace SilverlightMappingToolBasic
{
    public class SimpleNodeSkin : INodeSkin
    {
        private bool _isNodeTypeInitialised = false;
        private double _nodeSkinWidth = -1.0;
        private double _nodeSkinHeight = -1.0;

        public SimpleNodeSkin()
        {
            SkinProperties = new Dictionary<string, object>();
        }

        #region INodeSkin Members

        public double NodeSkinWidth
        {
            get
            {
                string nodeSkinWidth = SkinProperties["NodeSkinWidth"] as string;

                if (_nodeSkinWidth < 0.0)
                {
                    _nodeSkinWidth = 0.0;

                    if (!string.IsNullOrEmpty(nodeSkinWidth))
                    {
                        double.TryParse(nodeSkinWidth, out _nodeSkinWidth);
                    }
                }

                return _nodeSkinWidth;
            }
            set
            {
                _nodeSkinWidth = value;
                SkinProperties["NodeSkinWidth"] = _nodeSkinWidth.ToString();
            }
        }

        public double NodeSkinHeight
        {
            get
            {
                string nodeSkinHeight = SkinProperties["NodeSkinHeight"] as string;

                if (_nodeSkinHeight < 0.0)
                {
                    _nodeSkinHeight = 0.0;

                    if (!string.IsNullOrEmpty(nodeSkinHeight))
                    {
                        double.TryParse(nodeSkinHeight, out _nodeSkinHeight);
                    }
                }

                return _nodeSkinHeight;
            }
            set
            {
                _nodeSkinHeight = value;
                SkinProperties["NodeSkinHeight"] = _nodeSkinHeight.ToString();
            }
        }

        public INodeTypeProxy NodeType
        {
            get;
            protected set;
        }

        public PathGeometry SkinClippingGeometry
        {
            get;
            protected set;
        }

        public Dictionary<string, object> SkinProperties
        {
            get;
            protected set;
        }

        public void InitialiseNodeType(INodeTypeProxy nodeType)
        {
            if (!_isNodeTypeInitialised)
            {
                throw new NotSupportedException("This method has already been invoked.");
            }

            NodeType = nodeType;
        }

        public NodeControl RenderSkinElements(INodeProxy node, string skinName, Dictionary<string, object> skinProperties)
        {
            NodeControl nodeElement = null;
            try
            {
                nodeElement = new NodeControl();
                nodeElement.DataContext = node;

                foreach (string propertyName in skinProperties.Keys)
                {
                    PropertyInfo propInfo = nodeElement.GetType().GetProperty(propertyName);
                    Type propertyType = propInfo.PropertyType;

                    if (propertyType != typeof(string))
                    {
                        TypeConverter converter = GetTypeConverter(propertyType);
                        object obj = converter.ConvertFrom(skinProperties[propertyName]);
                        propInfo.SetValue(nodeElement, obj, null);
                    }
                    else
                    {
                        propInfo.SetValue(nodeElement, skinProperties[propertyName], null);
                    }
                }
            }
            catch (Exception)
            {
                nodeElement = null;
            }
            
            // TODO: Load an 'ApplicationSettings' object here so that the NodeFontSize can be determined through application/user settings.

            return nodeElement;
        }

        #endregion

        public static TypeConverter GetTypeConverter(Type type)
        {
            TypeConverterAttribute attribute = (TypeConverterAttribute)Attribute.GetCustomAttribute(type, typeof(TypeConverterAttribute), false);
            if (attribute != null)
            {
                try
                {
                    var converterType = Type.GetType(attribute.ConverterTypeName, false);
                    if (converterType != null)
                    {
                        return (Activator.CreateInstance(converterType) as TypeConverter);
                    }
                }
                catch { }
            }
            return new XamlStringConverter(type);
        }
    }

    public class XamlStringConverter : TypeConverter
    {
        private Type _type;
        public XamlStringConverter(Type type)
            : base()
        {
            _type = type;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var strValue = value as string;
            if (strValue != null)
            {
                if (this._type == typeof(bool))
                {
                    return bool.Parse(strValue);
                }
                if (this._type.IsEnum)
                {
                    return Enum.Parse(this._type, strValue, false);
                }
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("<ContentControl xmlns='http://schemas.microsoft.com/client/2007' xmlns:c='" + ("clr-namespace:" + this._type.Namespace + ";assembly=" + this._type.Assembly.FullName.Split(new char[] { ',' })[0]) + "'>\n");
                stringBuilder.Append("<c:" + this._type.Name + ">\n");
                stringBuilder.Append(strValue);
                stringBuilder.Append("</c:" + this._type.Name + ">\n");
                stringBuilder.Append("</ContentControl>");
                ContentControl instance = XamlReader.Load(stringBuilder.ToString()) as ContentControl;
                if (instance != null)
                {
                    return instance.Content;
                }
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}
