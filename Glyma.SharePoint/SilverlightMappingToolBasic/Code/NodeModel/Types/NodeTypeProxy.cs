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
using SilverlightMappingToolBasic.MappingService;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace SilverlightMappingToolBasic
{
    public class NodeTypeProxy : INodeTypeProxy
    {
        protected NodeTypeProxy()
        {
        }

        public NodeTypeProxy(SoapNodeType soapNodeType)
        {
            BaseSoapNodeType = soapNodeType;
        }

        public SoapNodeType BaseSoapNodeType
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
        private static Dictionary<Guid, NodeTypeProxy> _nodeTypes = null;

        public static NodeTypeProxy GetNodeType(SoapNodeType soapNodeType)
        {
            lock (_padlock)
            {
                if (_nodeTypes == null)
                {
                    _nodeTypes = new Dictionary<Guid, NodeTypeProxy>();
                }

                NodeTypeProxy nodeType;

                if (_nodeTypes.ContainsKey(soapNodeType.Id))
                {
                    nodeType = _nodeTypes[soapNodeType.Id];
                }
                else
                {
                    nodeType = new NodeTypeProxy(soapNodeType);

                    _nodeTypes[soapNodeType.Id] = nodeType;
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
