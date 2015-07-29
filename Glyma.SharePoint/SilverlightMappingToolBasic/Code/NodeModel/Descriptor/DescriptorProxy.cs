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
    public class DescriptorProxy : IDescriptorProxy
    {
        public DescriptorProxy()
        {
        }

        public DescriptorProxy(Guid nodeId, SoapDescriptorType descriptorType, IRelationshipProxy relationship, SoapRelationship soapRelationship)
        {
            Relationship = relationship;

            NodeId = nodeId;

            DescriptorType = DescriptorTypeProxy.GetDescriptorType(descriptorType);
        }

        public override string ToString()
        {
            return string.Format("Node: {0}, DescriptorType: {1}", Node.Name, DescriptorType.Name);
        }

        public override bool Equals(object obj)
        {
            DescriptorProxy secondObj = obj as DescriptorProxy;
            if (secondObj != null)
            {
                if (this.Relationship.Id == secondObj.Relationship.Id &&
                    this.DescriptorType.Id == secondObj.DescriptorType.Id &&
                    this.NodeId == secondObj.NodeId)
                {
                    return true;
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public Guid NodeId
        {
            get;
            set;
        }

        #region IDescriptor Members

        public INodeProxy Node
        {
            get;
            set;
        }

        public IRelationshipProxy Relationship
        {
            get;
            set;
        }

        public IDescriptorTypeProxy DescriptorType
        {
            get;
            set;
        }

        #endregion
    }
}
