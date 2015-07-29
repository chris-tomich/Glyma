using System;

namespace SilverlightMappingToolBasic.CompendiumMapProcessor.XmlModel.NodeTypes
{
    public class ConNode : INodeType
    {
        public const string ConNodeTypeId = "{DA66B603-F6B3-4ECF-8ED0-AB34A288CF08}";

        public ConNode()
        {
        }

        #region INodeType Members

        public Guid Id
        {
            get
            {
                return new Guid(ConNodeTypeId);
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
                return "CompendiumConNode";
            }
            set
            {
                return;
            }
        }

        #endregion
    }
}
