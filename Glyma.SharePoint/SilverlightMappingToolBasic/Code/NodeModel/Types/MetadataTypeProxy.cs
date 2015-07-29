using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using SilverlightMappingToolBasic.MappingService;

namespace SilverlightMappingToolBasic
{
    public class MetadataTypeProxy : IMetadataTypeProxy
    {
        protected MetadataTypeProxy()
        {
        }

        public MetadataTypeProxy(SoapMetadataType soapMetadataType)
        {
            BaseSoapNodeType = soapMetadataType;
        }

        public SoapMetadataType BaseSoapNodeType
        {
            get;
            protected set;
        }

        public override string ToString()
        {
            return Name;
        }

        #region Static Members

        private static object _padlock = new object();
        private static Dictionary<Guid, MetadataTypeProxy> _nodeTypes = null;

        public static MetadataTypeProxy GetMetadataType(SoapMetadataType soapMetadataType)
        {
            lock (_padlock)
            {
                if (_nodeTypes == null)
                {
                    _nodeTypes = new Dictionary<Guid, MetadataTypeProxy>();
                }

                MetadataTypeProxy nodeType;

                if (_nodeTypes.ContainsKey(soapMetadataType.Id))
                {
                    nodeType = _nodeTypes[soapMetadataType.Id];
                }
                else
                {
                    nodeType = new MetadataTypeProxy(soapMetadataType);

                    _nodeTypes[soapMetadataType.Id] = nodeType;
                }

                return nodeType;
            }
        }

        #endregion

        #region ITypeElement Members

        public Guid Id
        {
            get
            {
                return BaseSoapNodeType.Id;
            }
            set
            {
                BaseSoapNodeType.Id = value;
            }
        }

        public string Name
        {
            get
            {
                return BaseSoapNodeType.Name;
            }
            set
            {
                BaseSoapNodeType.Name = value;
            }
        }

        #endregion
    }
}
