using System;
using System.Net;
using System.Collections.Generic;
using System.Xml;
using IoC;

namespace SilverlightMappingToolBasic.Compendium
{
    public class XmlCompendiumNode : INode
    {
        private const string ArgumentNodeTypeId = "";
        private const string ConNodeTypeId = "7";
        private const string DecisionNodeTypeId = "8";
        private const string IdeaNodeTypeId = "4";
        private const string ListNodeTypeId = "";
        private const string MapNodeTypeId = "2";
        private const string NoteNodeTypeId = "";
        private const string ProNodeTypeId = "6";
        private const string QuestionNodeTypeId = "3";
        private const string ReferenceTypeId = "";

        private INodeType _nodeType;
        private List<string> _notes;
        private List<string> _attachments;
        private List<IDescriptor> _descriptors;

        public XmlCompendiumNode()
        {
            _notes = new List<string>();
            _attachments = new List<string>();
            _descriptors = new List<IDescriptor>();
        }

        public XmlCompendiumNode(INodeType nodeType)
            : this()
        {
            _nodeType = nodeType;
        }

        public XmlCompendiumNode(XmlReader compendiumNodeXml)
            : this()
        {
            ConsumeNodeXml(compendiumNodeXml);
        }

        public void ConsumeNodeXml(XmlReader xml)
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
                if (xml.MoveToAttribute("type"))
                {
                    switch (xml.Value)
                    {
                        case ConNodeTypeId:
                            _nodeType = IoCContainer.GetInjectionInstance().GetInstance<ConNode>();
                            break;

                        case DecisionNodeTypeId:
                            _nodeType = IoCContainer.GetInjectionInstance().GetInstance<DecisionNode>();
                            break;

                        case IdeaNodeTypeId:
                            _nodeType = IoCContainer.GetInjectionInstance().GetInstance<IdeaNode>();
                            break;

                        case MapNodeTypeId:
                            _nodeType = IoCContainer.GetInjectionInstance().GetInstance<MapNode>();
                            break;

                        case ProNodeTypeId:
                            _nodeType = IoCContainer.GetInjectionInstance().GetInstance<ProNode>();
                            break;

                        case QuestionNodeTypeId:
                            _nodeType = IoCContainer.GetInjectionInstance().GetInstance<QuestionNode>();
                            break;

                        default:
                            break;
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
