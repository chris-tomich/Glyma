using System;
using System.Collections.Generic;
using System.Xml;
using SimpleIoC;

namespace SilverlightMappingToolBasic.CompendiumMapProcessor.XmlModel.Nodes
{
    public class XmlCompendiumNode : INode
    {
        private const string LIST_NODE_TYPE_ID = "1";
        private const string MAP_NODE_TYPE_ID = "2";
        private const string QUESTION_NODE_TYPE_ID = "3";
        private const string IDEA_NODE_TYPE_ID = "4";
        private const string ARGUMENT_NODE_TYPE_ID = "5";
        private const string PRO_NODE_TYPE_ID = "6";
        private const string CON_NODE_TYPE_ID = "7";
        private const string DECISION_NODE_TYPE_ID = "8";
        private const string REFERENCE_NODE_TYPE_ID = "9";
        private const string NOTE_NODE_TYPE_ID = "10";
        
        private TransactionalNodeService.Proxy.NodeType _nodeType;
        private List<string> _notes;
        private string _attachment;
        private List<IDescriptor> _descriptors;

        public XmlCompendiumNode()
        {
            _notes = new List<string>();
            _descriptors = new List<IDescriptor>();
            Properties = new Dictionary<string, string>();
        }

        public XmlCompendiumNode(TransactionalNodeService.Proxy.NodeType nodeType)
            : this()
        {
            _nodeType = nodeType;
        }

        public XmlCompendiumNode(XmlReader compendiumNodeXml, string domainNodeId, string documentLibraryUrl)
            : this()
        {
            ConsumeNodeXml(compendiumNodeXml, domainNodeId, documentLibraryUrl);
        }

        public void ConsumeNodeXml(XmlReader xml, string domainNodeId, string documentLibraryUrl)
        {
            if (xml != null && xml.NodeType == XmlNodeType.Element && xml.Name == "node")
            {
                #region Attribute Values
                #region Id
                if (xml.MoveToAttribute("id"))
                {
                    Id = xml.Value;
                }
                #endregion

                #region NodeType
                if (Id == domainNodeId)
                {
                    _nodeType = IoCContainer.GetInjectionInstance().GetInstance<TransactionalNodeService.Proxy.IMapManager>().NodeTypes["DomainNode"];
                }
                else 
                {
                    if (xml.MoveToAttribute("type"))
                    {
                        switch (xml.Value)
                        {
                            case CON_NODE_TYPE_ID:
                                _nodeType = IoCContainer.GetInjectionInstance().GetInstance<TransactionalNodeService.Proxy.IMapManager>().NodeTypes["CompendiumConNode"];
                                break;

                            case DECISION_NODE_TYPE_ID:
                                _nodeType = IoCContainer.GetInjectionInstance().GetInstance<TransactionalNodeService.Proxy.IMapManager>().NodeTypes["CompendiumDecisionNode"];
                                break;

                            case IDEA_NODE_TYPE_ID:
                                _nodeType = IoCContainer.GetInjectionInstance().GetInstance<TransactionalNodeService.Proxy.IMapManager>().NodeTypes["CompendiumIdeaNode"];
                                break;

                            case MAP_NODE_TYPE_ID:
                                _nodeType = IoCContainer.GetInjectionInstance().GetInstance<TransactionalNodeService.Proxy.IMapManager>().NodeTypes["CompendiumMapNode"];
                                break;

                            case PRO_NODE_TYPE_ID:
                                _nodeType = IoCContainer.GetInjectionInstance().GetInstance<TransactionalNodeService.Proxy.IMapManager>().NodeTypes["CompendiumProNode"];
                                break;

                            case QUESTION_NODE_TYPE_ID:
                                _nodeType = IoCContainer.GetInjectionInstance().GetInstance<TransactionalNodeService.Proxy.IMapManager>().NodeTypes["CompendiumQuestionNode"];
                                break;

                            case REFERENCE_NODE_TYPE_ID:
                                _nodeType = IoCContainer.GetInjectionInstance().GetInstance<TransactionalNodeService.Proxy.IMapManager>().NodeTypes["CompendiumReferenceNode"];
                                break;

                            case ARGUMENT_NODE_TYPE_ID:
                                _nodeType = IoCContainer.GetInjectionInstance().GetInstance<TransactionalNodeService.Proxy.IMapManager>().NodeTypes["CompendiumArgumentNode"];
                                break;

                            case NOTE_NODE_TYPE_ID:
                                _nodeType = IoCContainer.GetInjectionInstance().GetInstance<TransactionalNodeService.Proxy.IMapManager>().NodeTypes["CompendiumNoteNode"];
                                break;

                            case LIST_NODE_TYPE_ID:
                                _nodeType = IoCContainer.GetInjectionInstance().GetInstance<TransactionalNodeService.Proxy.IMapManager>().NodeTypes["CompendiumListNode"];
                                break;

                            default:
                                _nodeType = IoCContainer.GetInjectionInstance().GetInstance<TransactionalNodeService.Proxy.IMapManager>().NodeTypes["CompendiumNoteNode"];
                                break;
                        }
                    }
                }
                #endregion

                #region CreatedBy
                if (xml.MoveToAttribute("author"))
                {
                    CreatedBy = xml.Value;
                }
                #endregion

                #region Created
                if (xml.MoveToAttribute("created"))
                {
                    string createdMillisecondsXmlValue = xml.Value;
                    long createdMilliseconds;

                    if (long.TryParse(createdMillisecondsXmlValue, out createdMilliseconds))
                    {
                        Created = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                        Created = Created.AddMilliseconds(createdMilliseconds);
                    }
                }
                #endregion

                #region LastModified
                if (xml.MoveToAttribute("lastModified"))
                {
                    string lastModifiedMillisecondsXmlValue = xml.Value;
                    long lastModifiedMilliseconds;

                    if (long.TryParse(lastModifiedMillisecondsXmlValue, out lastModifiedMilliseconds))
                    {
                        LastModified = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                        LastModified = LastModified.AddMilliseconds(lastModifiedMilliseconds);
                    }
                }
                #endregion

                #region Name
                if (xml.MoveToAttribute("label"))
                {
                    Name = xml.Value;
                }
                #endregion

                #region LastModifiedBy
                if (xml.MoveToAttribute("lastModificationAuthor"))
                {
                    LastModifiedBy = xml.Value;
                }
                #endregion
                #endregion

                while (xml.Read())
                {
                    if (xml.NodeType == XmlNodeType.Element)
                    {
                        if (xml.Name.ToLower() == "details")
                        {
                        }
                        if (_nodeType.Name == "CompendiumReferenceNode")
                        {
                            if (xml.Name.ToLower() == "source")
                            {
                                string fullFilePath = xml.ReadInnerXml();
                                if (!string.IsNullOrEmpty(fullFilePath))
                                {
                                    if (fullFilePath.ToLower().StartsWith("http://") || fullFilePath.ToLower().StartsWith("https://")
                                        || fullFilePath.ToLower().StartsWith("ftp://") || fullFilePath.ToLower().StartsWith("ftps://"))
                                    {
                                        Attachment = fullFilePath;
                                    }
                                    else
                                    {
                                        string linkedFileName = string.Empty;
                                        if (fullFilePath.LastIndexOf("/") < 0 && fullFilePath.LastIndexOf("\\") > 0)
                                        {
                                            linkedFileName = fullFilePath.Substring(fullFilePath.LastIndexOf("\\") + 1);
                                        }
                                        else
                                        {
                                            linkedFileName = fullFilePath.Substring(fullFilePath.LastIndexOf("/") + 1);
                                        }
                                        Attachment = documentLibraryUrl + linkedFileName; //set the file name, the node name is can change independently
                                    }
                                }
                            }
                        }
                    }
                    else if (xml.NodeType == XmlNodeType.EndElement && xml.Name == "node")
                    {
                        break;
                    }
                }
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

        public IDictionary<string, string> Properties
        {
            get;
            private set;
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

        public string Attachment
        {
            get
            {
                return _attachment;
            }
            set
            {
                _attachment = value;
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

        public TransactionalNodeService.Proxy.NodeType NodeType
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

        public void AddDescriptor(IDescriptor descriptor)
        {
            _descriptors.Add(descriptor);
        }

        public void RemoveNote(string note)
        {
            _notes.Remove(note);
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
