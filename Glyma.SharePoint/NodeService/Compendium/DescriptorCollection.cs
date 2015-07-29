using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NodeService
{
    public class DescriptorCollection : List<IDescriptor>
    {
        public DescriptorCollection()
            : base()
        {
        }

        public DescriptorCollection(int capacity)
            : base(capacity)
        {
        }

        public DescriptorCollection(IEnumerable<IDescriptor> collection)
            : base(collection)
        {
        }

        public IEnumerable<IRelationship> GetRelationshipsByDescriptorType(IDescriptorType descriptorType)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IRelationship> GetRelationshipsByDescriptorTypeId(Guid descriptorTypeId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IRelationship> GetRelationshipsByDescriptorTypeName(string descriptorTypeName)
        {
            throw new NotImplementedException();
        }
    }
}
