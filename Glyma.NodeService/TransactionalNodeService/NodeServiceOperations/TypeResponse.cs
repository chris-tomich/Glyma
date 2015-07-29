using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TransactionalNodeService.Model;
using System.Runtime.Serialization;

namespace TransactionalNodeService
{
    [DataContract]
    public class TypeResponse
    {
        protected Dictionary<string, NodeType> _nodeTypes = null;
        protected Dictionary<string, RelationshipType> _relationshipTypes = null;
        protected Dictionary<string, DescriptorType> _descriptorTypes = null;
        protected Dictionary<string, MetadataType> _metadataTypes = null;

        public TypeResponse()
        {
        }

        [DataMember]
        public Dictionary<string, NodeType> NodeTypes
        {
            get
            {
                if (_nodeTypes == null)
                {
                    _nodeTypes = new Dictionary<string, NodeType>();
                }

                return _nodeTypes;
            }
            set
            {
                _nodeTypes = value;
            }
        }

        [DataMember]
        public Dictionary<string, RelationshipType> RelationshipTypes
        {
            get
            {
                if (_relationshipTypes == null)
                {
                    _relationshipTypes = new Dictionary<string, RelationshipType>();
                }

                return _relationshipTypes;
            }
            set
            {
                _relationshipTypes = value;
            }
        }

        [DataMember]
        public Dictionary<string, DescriptorType> DescriptorTypes
        {
            get
            {
                if (_descriptorTypes == null)
                {
                    _descriptorTypes = new Dictionary<string, DescriptorType>();
                }

                return _descriptorTypes;
            }
            set
            {
                _descriptorTypes = value;
            }
        }

        [DataMember]
        public Dictionary<string, MetadataType> MetadataTypes
        {
            get
            {
                if (_metadataTypes == null)
                {
                    _metadataTypes = new Dictionary<string, MetadataType>();
                }

                return _metadataTypes;
            }
            set
            {
                _metadataTypes = value;
            }
        }
    }
}