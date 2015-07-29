using System;
using System.Net;
using WebSvcNs = SilverlightMappingToolBasic.CompendiumSharePointService;
using System.Collections.Generic;
using IoC;

namespace SilverlightMappingToolBasic.Compendium
{
    public class WebServiceCompendiumNode : INode
    {
        private INodeType _nodeType;
        private List<string> _notes;
        private List<string> _attachments;
        private List<IDescriptor> _descriptors;

        public WebServiceCompendiumNode()
        {
            _notes = new List<string>();
            _attachments = new List<string>();
            _descriptors = new List<IDescriptor>();
        }

        public WebServiceCompendiumNode(WebSvcNs.Node node)
            : this()
        {
            ConsumeWebServiceNode(node);
        }

        public void ConsumeWebServiceNode(WebSvcNs.Node node)
        {
            if (NodeType != null && NodeType.Id != Guid.Empty)
            {
                switch (NodeType.Id.ToString())
                {
                    case ConNode.ConNodeTypeId:
                        NodeType = IoCContainer.GetInjectionInstance().GetInstance<ConNode>();
                        break;

                    case DecisionNode.DecisionNodeTypeId:
                        NodeType = IoCContainer.GetInjectionInstance().GetInstance<DecisionNode>();
                        break;

                    case IdeaNode.IdeaNodeTypeId:
                        NodeType = IoCContainer.GetInjectionInstance().GetInstance<IdeaNode>();
                        break;

                    case MapNode.MapNodeTypeId:
                        NodeType = IoCContainer.GetInjectionInstance().GetInstance<MapNode>();
                        break;

                    case ProNode.ProNodeTypeId:
                        NodeType = IoCContainer.GetInjectionInstance().GetInstance<ProNode>();
                        break;

                    case QuestionNode.QuestionNodeTypeId:
                        NodeType = IoCContainer.GetInjectionInstance().GetInstance<QuestionNode>();
                        break;

                    default:
                        break;
                }

                Id = node.Id;
                Name = (string)GetObjectData<string>(node.Data, "Name");
                CreatedBy = (string)GetObjectData<string>(node.Data, "CreatedBy");
                LastModifiedBy = (string)GetObjectData<string>(node.Data, "LastModifiedBy");
                Created = (DateTime)GetObjectData<DateTime>(node.Data, "Created");
                LastModified = (DateTime)GetObjectData<DateTime>(node.Data, "LastModified");
            }
        }

        private object GetObjectData<DataType>(Dictionary<string, object> hashTable, string dataName)
        {
            object data = hashTable[dataName];

            if (data is DataType)
            {
                return data;
            }
            else
            {
                return default(DataType);
            }
        }

        #region INode Members

        public string Id
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string CreatedBy
        {
            get;
            set;
        }

        public string LastModifiedBy
        {
            get;
            set;
        }

        public DateTime Created
        {
            get;
            set;
        }

        public DateTime LastModified
        {
            get;
            set;
        }

        public string[] Notes
        {
            get
            {
                return _notes.ToArray();
            }
            set
            {
                _notes.Clear();
                _notes.AddRange(value);
            }
        }

        public string[] Attachments
        {
            get
            {
                return _attachments.ToArray();
            }
            set
            {
                _notes.Clear();
                _notes.AddRange(value);
            }
        }

        public IDescriptor[] Descriptors
        {
            get
            {
                return _descriptors.ToArray();
            }
            set
            {
                _descriptors.Clear();
                _descriptors.AddRange(value);
            }
        }

        public INodeType NodeType
        {
            get
            {
                return _nodeType;
            }
            set
            {
                _nodeType = value;
            }
        }

        public void AddNote(string note)
        {
            _notes.Add(note);
        }

        public void AddAttachment(string attachment)
        {
            _attachments.Add(attachment);
        }

        public void AddDescriptor(IDescriptor descriptor)
        {
            _descriptors.Add(descriptor);
        }

        public void RemoveNote(string note)
        {
            _notes.Remove(note);
        }

        public void RemoveAttachment(string attachment)
        {
            _attachments.Remove(attachment);
        }

        public void RemoveDescriptor(IDescriptor descriptor)
        {
            _descriptors.Remove(descriptor);
        }

        public bool Equals(IStorageElement secondElement)
        {
            return Id.Equals(secondElement.Id);
        }

        #endregion
    }
}
