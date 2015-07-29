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

namespace SilverlightMappingToolBasic
{
    public class RelationshipProxy : IRelationshipProxy
    {
        IRelationshipTypeProxy _relationshipType;

        public RelationshipProxy()
        {
            Descriptors = new DescriptorCollection();
        }

        public RelationshipProxy(SoapRelationship soapRelationship)
            : this()
        {
            BaseSoapRelationship = soapRelationship;

            foreach (KeyValuePair<SoapDescriptorType, Guid> nodeDescriptorPair in BaseSoapRelationship.Nodes)
            {
                DescriptorProxy descriptor = new DescriptorProxy(nodeDescriptorPair.Value, nodeDescriptorPair.Key, this, BaseSoapRelationship);

                Descriptors.Add(descriptor);
            }
        }

        public SoapRelationship BaseSoapRelationship
        {
            get;
            protected set;
        }

        public override string ToString()
        {
            return string.Format("RelationshipType: {0}, Descriptor Count: {1}", RelationshipType.Name, Descriptors.Count);
        }

        #region IRelationship Members

        public string[] Notes
        {
            get;
            set;
        }

        public string[] Attachments
        {
            get;
            set;
        }

        public IRelationshipTypeProxy RelationshipType
        {
            get
            {
                if (_relationshipType == null)
                {
                    _relationshipType = IoC.IoCContainer.GetInjectionInstance().GetInstance<TypeManager>().GetRelationshipType(BaseSoapRelationship.RelationshipType);
                }

                return _relationshipType;
            }
            set
            {
                _relationshipType = value;
            }
        }

        public DescriptorCollection Descriptors
        {
            get;
            set;
        }

        public void AddNote(string note)
        {
            throw new NotImplementedException();
        }

        public void AddAttachment(string attachment)
        {
            throw new NotImplementedException();
        }

        //public void AddDescriptor(IDescriptorProxy descriptor)
        //{
        //    throw new NotImplementedException();
        //}

        public void RemoveNote(string note)
        {
            throw new NotImplementedException();
        }

        public void RemoveAttachment(string attachment)
        {
            throw new NotImplementedException();
        }

        //public void RemoveDescriptor(IDescriptorProxy descriptor)
        //{
        //    throw new NotImplementedException();
        //}

        #endregion

        #region IStorageElement Members

        public Guid Id
        {
            get
            {
                return BaseSoapRelationship.Id;
            }
            set
            {
                BaseSoapRelationship.Id = value;
            }
        }

        public string Name
        {
            get
            {
                return BaseSoapRelationship.Metadata["Name"].MetadataValue;
            }
            set
            {
                BaseSoapRelationship.Metadata["Name"].MetadataValue = value;
            }
        }

        public string CreatedBy
        {
            get
            {
                return BaseSoapRelationship.Metadata["CreatedBy"].MetadataValue;
            }
            set
            {
                BaseSoapRelationship.Metadata["CreatedBy"].MetadataValue = value;
            }
        }

        public string LastModifiedBy
        {
            get
            {
                return BaseSoapRelationship.Metadata["LastModifiedBy"].MetadataValue;
            }
            set
            {
                BaseSoapRelationship.Metadata["LastModifiedBy"].MetadataValue = value;
            }
        }

        public DateTime Created
        {
            get
            {
                return DateTime.Parse(BaseSoapRelationship.Metadata["Created"].MetadataValue);
            }
            set
            {
                BaseSoapRelationship.Metadata["Created"].MetadataValue = value.ToString();
            }
        }

        public DateTime LastModified
        {
            get
            {
                return DateTime.Parse(BaseSoapRelationship.Metadata["LastModified"].MetadataValue);
            }
            set
            {
                BaseSoapRelationship.Metadata["LastModified"].MetadataValue = value.ToString();
            }
        }

        public bool Equals(IStorageElement secondElement)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
