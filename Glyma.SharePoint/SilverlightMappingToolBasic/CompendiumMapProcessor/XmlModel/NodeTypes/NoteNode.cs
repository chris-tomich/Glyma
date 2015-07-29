using System;

namespace SilverlightMappingToolBasic.CompendiumMapProcessor.XmlModel.NodeTypes
{
    public class NoteNode : INodeType
    {
        public const string NoteNodeTypeId = "{84B7634B-DB8D-449B-B8CE-D3F3F80E95DD}";

        public NoteNode()
        {
        }

        #region INodeType Members

        public Guid Id
        {
            get
            {
                return new Guid(NoteNodeTypeId);
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
                return "CompendiumNoteNode";
            }
            set
            {
                return;
            }
        }

        #endregion
    }
}
