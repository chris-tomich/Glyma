using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.BusinessData.Runtime;

namespace Glyma.SharePoint.Search
{
    /// <summary>
    /// Filters out a Glyma node if it is identified as a user specified question.
    /// </summary>
    public class SpecifiedQuestionFilter : INodeCrawlRule
    {
        private string _configuration = string.Empty;
        private List<string> _specifiedQuestions = new List<string>();


        public SpecifiedQuestionFilter() 
        {
        }


        public SpecifiedQuestionFilter(string configuration)
        {
            Configuration = configuration;
        }


        public string Configuration
        {
            get
            {
                return _configuration;
            }

            set
            {
                _configuration = value;
                _specifiedQuestions.Clear();
                string[] questions = _configuration.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                _specifiedQuestions.AddRange(questions);
            }
        }

       
        public ReadOnlyCollection<string> SpecifiedQuestions
        {
            get
            {
                return _specifiedQuestions.AsReadOnly();
            }
        }


        public bool Apply(DynamicType glymaNode, IGlymaMapRepository mapRepository)
        {
            bool includeNode = true;

            if (glymaNode == null)
            {
               throw new ArgumentNullException("glymaNode");
            }

            string nodeType = (string)glymaNode[GlymaEntityFields.NodeType];
            if (nodeType.Equals(GlymaNodeTypes.Question, StringComparison.OrdinalIgnoreCase))
            {
                string nodeName = (string)glymaNode[GlymaEntityFields.Name];
                if (!string.IsNullOrEmpty(nodeName))
                {
                  foreach (string specifiedQuestion in SpecifiedQuestions)
                  {

                        if (nodeName.Equals(specifiedQuestion, StringComparison.OrdinalIgnoreCase))
                        {
                           includeNode = false;
                           break;
                        }
                  }
                }
                else
                {
                    includeNode = false;
                }
            }

            return includeNode;
        }


        public INodeCrawlRule DeepCopy()
        {
           return new SpecifiedQuestionFilter(Configuration);
        }
    }
}
