using System;

namespace NodeService
{
    public class QuestionNode : INodeType
    {
        public const string QuestionNodeTypeId = "{99FD1475-8099-45D3-BEDF-BEC396CCB4DD}";

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
                return "CompendiumQuestionNode";
            }
            set
            {
                return;
            }
        }

        #endregion
    }
}
