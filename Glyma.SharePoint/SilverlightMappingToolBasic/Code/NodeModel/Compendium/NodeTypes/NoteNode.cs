using System;

namespace SilverlightMappingToolBasic.Compendium
{
    public class NoteNode : INodeType
    {
        public const string NoteNodeTypeId = "{AB0E9BF3-E2E0-47b1-AB0E-8F62B7F62FDA}";

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
                return "Note";
            }
            set
            {
                return;
            }
        }

        #endregion
    }
}
