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
using System.Collections.Generic;

namespace SilverlightMappingToolBasic
{
    public class DescriptorCollection : List<IDescriptorProxy>
    {
        public DescriptorCollection()
            : base()
        {
        }

        public DescriptorCollection(int capacity)
            : base(capacity)
        {
        }

        public DescriptorCollection(IEnumerable<IDescriptorProxy> collection)
            : base(collection)
        {
        }

        public IEnumerable<IDescriptorProxy> GetByDescriptorType(IDescriptorTypeProxy descriptorType)
        {
            foreach (IDescriptorProxy descriptor in this)
            {
                if (descriptor.DescriptorType == descriptorType)
                {
                    yield return descriptor;
                }
            }
        }

        public IEnumerable<IDescriptorProxy> GetByDescriptorTypeId(Guid descriptorTypeId)
        {
            foreach (IDescriptorProxy descriptor in this)
            {
                if (descriptor.DescriptorType.Id == descriptorTypeId)
                {
                    yield return descriptor;
                }
            }
        }

        public IEnumerable<IDescriptorProxy> GetByDescriptorTypeName(string descriptorTypeName)
        {
            foreach (IDescriptorProxy descriptor in this)
            {
                if (descriptor.DescriptorType.Name == descriptorTypeName)
                {
                    yield return descriptor;
                }
            }
        }
    }
}
