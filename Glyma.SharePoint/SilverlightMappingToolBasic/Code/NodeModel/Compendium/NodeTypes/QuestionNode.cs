using System;

namespace SilverlightMappingToolBasic.Compendium
{
    public class QuestionNode : INodeType
    {
        public const string QuestionNodeTypeId = "{9F137808-E88A-4c3f-A1B5-B5FAF5BA9996}";

        public QuestionNode()
        {
        }

        #region INodeType Members

        public Guid Id
        {
            get
            {
                return new Guid(QuestionNodeTypeId);
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
                return "Question";
            }
            set
            {
                return;
            }
        }

        #endregion
    }
}
