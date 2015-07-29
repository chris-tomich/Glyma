using System;
using System.Collections.Generic;
using TransactionalNodeService.Proxy.Universal.Types;

namespace Glyma.UtilityService.Export.IBIS.Compendium.Extensions
{
    public static class NodeHelper
    {
        private const string ListNodeTypeId = "1";
        private const string MapNodeTypeId = "2";
        private const string QuestionNodeTypeId = "3";
        private const string IdeaNodeTypeId = "4";
        private const string ArgumentNodeTypeId = "5";
        private const string ProNodeTypeId = "6";
        private const string ConNodeTypeId = "7";
        private const string DecisionNodeTypeId = "8";
        private const string ReferenceNodeTypeId = "9";
        private const string NoteNodeTypeId = "10";

        private static Dictionary<string, string> _dictionary;

        private static object _nodeTypeDictLock = new object();

        public static Dictionary<string, string> Dictionary
        {
            get
            {
                if (_dictionary == null)
                {
                    lock (_nodeTypeDictLock)
                    {
                        if (_dictionary == null)
                        {
                            _dictionary = new Dictionary<string, string>();
                            _dictionary.Add("CompendiumConNode", "Cons");
                            _dictionary.Add("CompendiumDecisionNode", "Decision");
                            _dictionary.Add("CompendiumIdeaNode", "Idea");
                            _dictionary.Add("CompendiumMapNode", "Map");
                            _dictionary.Add("CompendiumProNode", "Pro");
                            _dictionary.Add("CompendiumQuestionNode", "Question");
                            _dictionary.Add("CompendiumReferenceNode", "Note");
                            _dictionary.Add("CompendiumArgumentNode", "Note");
                            _dictionary.Add("CompendiumNoteNode", "Note");
                            _dictionary.Add("CompendiumListNode", "Note");
                        }
                    }
                }
                return _dictionary;
            }
        }

        public static string ToLongString(this Guid origin)
        {
            return origin.ToString();
        }

        public static string ToId(this NodeType nodeType)
        {
            switch (nodeType.Name)
            {
                case "CompendiumConNode":
                    return ConNodeTypeId;
                case "CompendiumDecisionNode":
                    return DecisionNodeTypeId;
                case "CompendiumIdeaNode":
                    return IdeaNodeTypeId;
                case "CompendiumMapNode":
                    return MapNodeTypeId;
                case "CompendiumProNode":
                    return ProNodeTypeId;
                case "CompendiumQuestionNode":
                    return QuestionNodeTypeId;
                case "CompendiumReferenceNode":
                    return ReferenceNodeTypeId;
                case "CompendiumArgumentNode":
                    return ArgumentNodeTypeId;
                case "CompendiumNoteNode":
                    return NoteNodeTypeId;
                case "CompendiumListNode":
                    return ListNodeTypeId;
                default:
                    return NoteNodeTypeId;
            }
        }
        

        public static string GetImageName(this NodeType nodeType)
        {
            if (Dictionary.ContainsKey(nodeType.Name))
            {
                return Dictionary[nodeType.Name];
            }
            return "Note";
        }
    }
}
