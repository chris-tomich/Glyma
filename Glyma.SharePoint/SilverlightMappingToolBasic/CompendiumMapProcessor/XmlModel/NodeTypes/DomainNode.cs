using System;

namespace SilverlightMappingToolBasic.CompendiumMapProcessor.XmlModel.NodeTypes
{
    public class DomainNode : INodeType
    {
        public const string DomainNodeTypeId = "{263754C2-2F31-4D21-B9C4-6509E00A5E94}";

        public DomainNode()
        {
        }

        #region INodeType Members

        public Guid Id
        {
            get
            {
                return new Guid(DomainNodeTypeId);
            }
            set
            {
                return;
            }
        }

        public string Name
        {
            get
            {
                return "DomainNode";
            }
            set
            {
                return;
            }
        }

        #endregion
    }
}
