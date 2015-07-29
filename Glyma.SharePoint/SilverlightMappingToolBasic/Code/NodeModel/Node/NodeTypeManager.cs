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
using System.Threading;

namespace SilverlightMappingToolBasic
{
    public class NodeTypeManager
    {
        private Dictionary<Guid, INodeTypeProxy> _nodeTypesById = null;
        private Dictionary<string, INodeTypeProxy> _nodeTypesByName = null;

        protected NodeTypeManager()
        {
            _nodeTypesById = new Dictionary<Guid, INodeTypeProxy>();
            _nodeTypesByName = new Dictionary<string, INodeTypeProxy>();
        }

        public NodeTypeManager(INodeService nodeService)
            : this()
        {
            NodeService = nodeService;
            IsInitialising = false;
            IsInitialised = false;
        }

        protected bool IsInitialising
        {
            get;
            set;
        }

        public bool IsInitialised
        {
            get;
            protected set;
        }

        public INodeService NodeService
        {
            get;
            protected set;
        }

        public event EventHandler InitialiseNodeTypeManagerCompleted;

        public void InitialiseNodeTypeManager()
        {
            if (!IsInitialised && !IsInitialising)
            {
                NodeService.GetAllSoapTypesCompleted += new EventHandler<ReturnedTypesEventArgs>(GetAllSoapNodeTypesCompleted);
                NodeService.GetAllSoapTypesAsync();
            }
        }

        private void GetAllSoapNodeTypesCompleted(object sender, ReturnedTypesEventArgs e)
        {
            foreach (INodeTypeProxy nodeTypeProxy in e.NodeTypes)
            {
                _nodeTypesById.Add(nodeTypeProxy.Id, nodeTypeProxy);
                _nodeTypesByName.Add(nodeTypeProxy.Name, nodeTypeProxy);
            }

            IsInitialised = true;

            EventArgs initialiseNodeTypeManagerEventArgs = new EventArgs();
            InitialiseNodeTypeManagerCompleted.Invoke(this, initialiseNodeTypeManagerEventArgs);
        }

        public INodeTypeProxy GetNodeType(SoapNodeType soapNodeType)
        {
            if (!IsInitialised)
            {
                throw new NotSupportedException("This node type manager has not been initialised.");
            }

            return _nodeTypesById[soapNodeType.Id];
        }

        public INodeTypeProxy GetNodeType(Guid soapNodeTypeId)
        {
            if (!IsInitialised)
            {
                throw new NotSupportedException("This node type manager has not been initialised.");
            }

            return _nodeTypesById[soapNodeTypeId];
        }

        public INodeTypeProxy GetNodeType(string soapNodeTypeName)
        {
            if (!IsInitialised)
            {
                throw new NotSupportedException("This node type manager has not been initialised.");
            }

            return _nodeTypesByName[soapNodeTypeName];
        }
    }
}
