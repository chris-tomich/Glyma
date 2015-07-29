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
    public class TypeManager
    {
        private Dictionary<Guid, INodeTypeProxy> _nodeTypesById = null;
        private Dictionary<string, INodeTypeProxy> _nodeTypesByName = null;

        private Dictionary<Guid, IDescriptorTypeProxy> _descriptorTypesById = null;
        private Dictionary<string, IDescriptorTypeProxy> _descriptorTypesByName = null;

        private Dictionary<Guid, IRelationshipTypeProxy> _relationshipTypesById = null;
        private Dictionary<string, IRelationshipTypeProxy> _relationshipTypesByName = null;

        private Dictionary<Guid, IMetadataTypeProxy> _metadataTypesById = null;
        private Dictionary<string, IMetadataTypeProxy> _metadataTypesByName = null;

        protected TypeManager()
        {
            _nodeTypesById = new Dictionary<Guid, INodeTypeProxy>();
            _nodeTypesByName = new Dictionary<string, INodeTypeProxy>();

            _descriptorTypesById = new Dictionary<Guid, IDescriptorTypeProxy>();
            _descriptorTypesByName = new Dictionary<string, IDescriptorTypeProxy>();

            _relationshipTypesById = new Dictionary<Guid, IRelationshipTypeProxy>();
            _relationshipTypesByName = new Dictionary<string, IRelationshipTypeProxy>();

            _metadataTypesById = new Dictionary<Guid, IMetadataTypeProxy>();
            _metadataTypesByName = new Dictionary<string, IMetadataTypeProxy>();
        }

        public TypeManager(INodeService nodeService)
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
                IsInitialising = true;
                NodeService.GetAllSoapTypesCompleted += new EventHandler<ReturnedTypesEventArgs>(GetAllSoapTypesCompleted);
                NodeService.GetAllSoapTypesAsync();
            }
        }

        private void GetAllSoapTypesCompleted(object sender, ReturnedTypesEventArgs e)
        {
            foreach (INodeTypeProxy nodeTypeProxy in e.NodeTypes)
            {
                if (!_nodeTypesById.ContainsKey(nodeTypeProxy.Id))
                {
                    _nodeTypesById.Add(nodeTypeProxy.Id, nodeTypeProxy);
                }

                if (!_nodeTypesByName.ContainsKey(nodeTypeProxy.Name))
                {
                    _nodeTypesByName.Add(nodeTypeProxy.Name, nodeTypeProxy);
                }
            }

            foreach (IDescriptorTypeProxy descriptorTypeProxy in e.DescriptorTypes)
            {
                if (!_descriptorTypesById.ContainsKey(descriptorTypeProxy.Id))
                {
                    _descriptorTypesById.Add(descriptorTypeProxy.Id, descriptorTypeProxy);
                }

                if (!_descriptorTypesByName.ContainsKey(descriptorTypeProxy.Name))
                {
                    _descriptorTypesByName.Add(descriptorTypeProxy.Name, descriptorTypeProxy);
                }
            }

            foreach (IRelationshipTypeProxy relationshipTypeProxy in e.RelationshipTypes)
            {
                if (!_relationshipTypesById.ContainsKey(relationshipTypeProxy.Id))
                {
                    _relationshipTypesById.Add(relationshipTypeProxy.Id, relationshipTypeProxy);
                }

                if (!_relationshipTypesByName.ContainsKey(relationshipTypeProxy.Name))
                {
                    _relationshipTypesByName.Add(relationshipTypeProxy.Name, relationshipTypeProxy);
                }
            }

            foreach (IMetadataTypeProxy metadataTypeProxy in e.MetadataTypes)
            {
                if (!_metadataTypesById.ContainsKey(metadataTypeProxy.Id))
                {
                    _metadataTypesById.Add(metadataTypeProxy.Id, metadataTypeProxy);
                }

                if (!_metadataTypesByName.ContainsKey(metadataTypeProxy.Name))
                {
                    _metadataTypesByName.Add(metadataTypeProxy.Name, metadataTypeProxy);
                }
            }

            IsInitialised = true;
            IsInitialising = false;

            EventArgs initialiseNodeTypeManagerEventArgs = new EventArgs();
            InitialiseNodeTypeManagerCompleted.Invoke(this, initialiseNodeTypeManagerEventArgs);
        }

        #region NodeTypes
        public INodeTypeProxy GetNodeType(SoapNodeType soapNodeType)
        {
            if (!IsInitialised)
            {
                throw new NotSupportedException("This type manager has not been initialised.");
            }

            return _nodeTypesById[soapNodeType.Id];
        }

        public INodeTypeProxy GetNodeType(Guid soapNodeTypeId)
        {
            if (!IsInitialised)
            {
                throw new NotSupportedException("This type manager has not been initialised.");
            }

            return _nodeTypesById[soapNodeTypeId];
        }

        public INodeTypeProxy GetNodeType(string soapNodeTypeName)
        {
            if (!IsInitialised)
            {
                throw new NotSupportedException("This type manager has not been initialised.");
            }

            return _nodeTypesByName[soapNodeTypeName];
        }
        #endregion

        #region DescriptorTypes
        public IDescriptorTypeProxy GetDescriptorType(SoapDescriptorType soapDescriptorType)
        {
            if (!IsInitialised)
            {
                throw new NotSupportedException("This type manager has not been initialised.");
            }

            return _descriptorTypesById[soapDescriptorType.Id];
        }

        public IDescriptorTypeProxy GetDescriptorType(Guid soapDescriptorTypeId)
        {
            if (!IsInitialised)
            {
                throw new NotSupportedException("This type manager has not been initialised.");
            }

            return _descriptorTypesById[soapDescriptorTypeId];
        }

        public IDescriptorTypeProxy GetDescriptorType(string soapDescriptorTypeName)
        {
            if (!IsInitialised)
            {
                throw new NotSupportedException("This type manager has not been initialised.");
            }

            return _descriptorTypesByName[soapDescriptorTypeName];
        }
        #endregion

        #region RelationshipTypes
        public IRelationshipTypeProxy GetRelationshipType(SoapRelationshipType soapRelationshipType)
        {
            if (!IsInitialised)
            {
                throw new NotSupportedException("This type manager has not been initialised.");
            }

            return _relationshipTypesById[soapRelationshipType.Id];
        }

        public IRelationshipTypeProxy GetRelationshipType(Guid soapRelationshipTypeId)
        {
            if (!IsInitialised)
            {
                throw new NotSupportedException("This type manager has not been initialised.");
            }

            return _relationshipTypesById[soapRelationshipTypeId];
        }

        public IRelationshipTypeProxy GetRelationshipType(string soapRelationshipTypeName)
        {
            if (!IsInitialised)
            {
                throw new NotSupportedException("This type manager has not been initialised.");
            }

            return _relationshipTypesByName[soapRelationshipTypeName];
        }
        #endregion

        #region MetadataTypes
        public IMetadataTypeProxy GetMetadataType(SoapRelationshipType soapMetadataType)
        {
            if (!IsInitialised)
            {
                throw new NotSupportedException("This type manager has not been initialised.");
            }

            return _metadataTypesById[soapMetadataType.Id];
        }

        public IMetadataTypeProxy GetMetadataType(Guid soapMetadataTypeId)
        {
            if (!IsInitialised)
            {
                throw new NotSupportedException("This type manager has not been initialised.");
            }

            return _metadataTypesById[soapMetadataTypeId];
        }

        public IMetadataTypeProxy GetMetadataType(string soapMetadataTypeName)
        {
            if (!IsInitialised)
            {
                throw new NotSupportedException("This type manager has not been initialised.");
            }

            return _metadataTypesByName[soapMetadataTypeName];
        }

        public IMetadataTypeProxy[] GetAllMetadataTypes()
        {
            if (!IsInitialised)
            {
                throw new NotSupportedException("This type manager has not been initialised.");
            }

            List<IMetadataTypeProxy> metaDataTypes = new List<IMetadataTypeProxy>(_metadataTypesByName.Values);
            return metaDataTypes.ToArray();
        }
        #endregion
    }
}
