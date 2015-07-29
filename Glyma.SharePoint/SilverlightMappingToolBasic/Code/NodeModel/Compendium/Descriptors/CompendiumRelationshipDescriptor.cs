using System;

namespace SilverlightMappingToolBasic.Compendium
{
    public class CompendiumRelationshipDescriptor : IDescriptor
    {
        public CompendiumRelationshipDescriptor()
        {
        }

        public CompendiumRelationshipDescriptor(INode node, IRelationship relationship, IDescriptorType descriptorType)
        {
            Node = node;
            Relationship = relationship;
            DescriptorType = descriptorType;
        }

        #region IDescriptor Members

        public INode Node
        {
            get;
            set;
        }

        public IRelationship Relationship
        {
            get;
            set;
        }

        public IDescriptorType DescriptorType
        {
            get;
            set;
        }

        #endregion
    }
}
