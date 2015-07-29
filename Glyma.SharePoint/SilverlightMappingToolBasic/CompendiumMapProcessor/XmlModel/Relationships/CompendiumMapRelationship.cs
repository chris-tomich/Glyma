using System;
using System.Collections.Generic;
using SilverlightMappingToolBasic.CompendiumMapProcessor.XmlModel.Descriptors;
using SilverlightMappingToolBasic.CompendiumMapProcessor.XmlModel.DescriptorTypes;
using SilverlightMappingToolBasic.CompendiumMapProcessor.XmlModel.RelationshipTypes;
using SimpleIoC;

namespace SilverlightMappingToolBasic.CompendiumMapProcessor.XmlModel.Relationships
{
    class CompendiumMapRelationship : IRelationship
    {
        private List<IDescriptor> _descriptors;
        private List<string> _notes;
        private List<string> _attachments;

        public CompendiumMapRelationship()
        {
            _descriptors = new List<IDescriptor>();
            _notes = new List<string>();
            _attachments = new List<string>();
            RelationshipType = IoCContainer.GetInjectionInstance().GetInstance<CompendiumMapRelationshipBaseType>();
        }

        public static void CreateMapRelationship(INode mapNode, INode childNode)
        {
            if (childNode != null && mapNode != null)
            {
                foreach (IDescriptor descriptor in childNode.Descriptors)
                {
                    if (descriptor.Relationship is CompendiumMapRelationship)
                    {
                        foreach (IDescriptor altDesc in descriptor.Relationship.Descriptors)
                        {
                            if (altDesc.DescriptorType is CompendiumToDescriptor)
                            {
                                if (altDesc.Node.Id == mapNode.Id)
                                {
                                    return;
                                }
                            }
                        }
                    }
                }
                CompendiumMapRelationship mapRelationship = new CompendiumMapRelationship();
                mapRelationship.Id = mapNode.Id + childNode.Id;
                IDescriptor toDescriptor = new CompendiumRelationshipDescriptor(mapNode, mapRelationship, IoCContainer.GetInjectionInstance().GetInstance<CompendiumToDescriptor>());
                IDescriptor fromDescriptor = new CompendiumRelationshipDescriptor(childNode, mapRelationship, IoCContainer.GetInjectionInstance().GetInstance<CompendiumFromDescriptor>());
                mapRelationship.Descriptors = new IDescriptor[] { fromDescriptor, toDescriptor };
                childNode.AddDescriptor(fromDescriptor);
                mapNode.AddDescriptor(toDescriptor);
            }
        }

        #region IRelationship Members

        public IRelationshipType RelationshipType
        {
            get;
            set;
        }

        public IDescriptor[] Descriptors
        {
            get
            {
                return _descriptors.ToArray();
            }
            set
            {
                _descriptors.Clear();
                _descriptors.AddRange(value);
            }
        }

        public string[] Notes
        {
            get
            {
                return _notes.ToArray();
            }
            set
            {
                _notes.Clear();
                _notes.AddRange(value);
            }
        }

        public string[] Attachments
        {
            get
            {
                return _attachments.ToArray();
            }
            set
            {
                _attachments.Clear();
                _attachments.AddRange(value);
            }
        }

        public void AddDescriptor(IDescriptor descriptor)
        {
            _descriptors.Add(descriptor);
        }

        public void RemoveDescriptor(IDescriptor descriptor)
        {
            _descriptors.Remove(descriptor);
        }

        public void AddNote(string note)
        {
            _notes.Add(note);
        }

        public void AddAttachment(string attachment)
        {
            _attachments.Add(attachment);
        }

        public void RemoveNote(string note)
        {
            _notes.Remove(note);
        }

        public void RemoveAttachment(string attachment)
        {
            _notes.Remove(attachment);
        }

        #endregion

        #region IStorageElement Members

        public string Id
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string CreatedBy
        {
            get;
            set;
        }

        public string LastModifiedBy
        {
            get;
            set;
        }

        public DateTime Created
        {
            get;
            set;
        }

        public DateTime LastModified
        {
            get;
            set;
        }

        public bool Equals(IStorageElement secondElement)
        {
            return (Id == secondElement.Id);
        }

        #endregion
    }
}
