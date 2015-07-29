using System;

namespace SilverlightMappingToolBasic.CompendiumMapProcessor.XmlModel.NodeTypes
{
    public class ProNode : INodeType
    {
        public const string ProNodeTypeId = "{084F38B7-115F-4AF6-9E30-D9D91226F86B}";

        public ProNode()
        {
        }

        #region INodeType Members

        public Guid Id
        {
            get
            {
                return new Guid(ProNodeTypeId);
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
                return "CompendiumProNode";
            }
            set
            {
                return;
            }
        }

        #endregion
    }
}
