using System;
using System.Net;
using System.Collections.Generic;
using System.Xml;
using IoC;

namespace NodeService
{
    public class CompendiumLinkRelationship : IRelationship
    {
        private List<IDescriptor> _descriptors;
        private List<string> _notes;
        private List<string> _attachments;

        public CompendiumLinkRelationship()
        {
            _descriptors = new List<IDescriptor>();
            _notes = new List<string>();
            _attachments = new List<string>();
            RelationshipType = IoCContainer.GetInjectionInstance().GetInstance<CompendiumNodeRelationshipBaseType>();
        }

        public bool ConsumeLinkXml(Dictionary<string, INode> nodeList, XmlReader xml, string domainNodeId)
        {
            bool foundDomainNodeLink = false;
            if (xml != null && xml.NodeType == XmlNodeType.Element && xml.Name == "link")
            {
                #region Attribute Values
                #region Id
                if (xml.MoveToAttribute("id"))
                {
                    Id = xml.Value;
                }
                #endregion

                #region NodeType - Not Currently Used
                /*
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
                */
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

                #region LastModifiedBy - Not Currently Used
                /*
                if (xml.MoveToAttribute("lastModificationAuthor"))
                {
                    LastModifiedBy = xml.Value;
                }
                */
                #endregion
                #endregion

                INode fromNode = null;
                if (xml.MoveToAttribute("from"))
                {
                    string from = xml.Value;
                    fromNode = nodeList[from];

                    IDescriptor fromDescriptor = new CompendiumRelationshipDescriptor(fromNode, this, IoCContainer.GetInjectionInstance().GetInstance<CompendiumFromDescriptor>());

                    _descriptors.Add(fromDescriptor);
                    fromNode.AddDescriptor(fromDescriptor);
                }

                INode toNode = null;
                if (xml.MoveToAttribute("to"))
                {
                    string to = xml.Value;
                    toNode = nodeList[to];

                    IDescriptor toDescriptor = new CompendiumRelationshipDescriptor(toNode, this, IoCContainer.GetInjectionInstance().GetInstance<CompendiumToDescriptor>());

                    _descriptors.Add(toDescriptor);
                    toNode.AddDescriptor(toDescriptor);
                }

                while (xml.Read())
                {
                    if (xml.NodeType == XmlNodeType.Element && xml.Name == "linkview")
                    {
                        string idAttVal = xml.GetAttribute("id");
                        if (domainNodeId == idAttVal)
                        {
                            foundDomainNodeLink = true;
                        }
                    }
                    else if (xml.NodeType == XmlNodeType.EndElement && xml.Name == "link")
                    {
                        break;
                    }
                }
            }
            return foundDomainNodeLink;
        }

        #region IRelationship Members

        public IRelationshipType RelationshipType
        {
            get;
            set;
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
                _attachments.Clear();
                _attachments.AddRange(value);
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
            _notes.Remove(attachment);
        }

        public void RemoveDescriptor(IDescriptor descriptor)
        {
            _descriptors.Remove(descriptor);
        }

        #endregion

        #region IStorageElement Members

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

        public bool Equals(IStorageElement secondElement)
        {
            return (Id == secondElement.Id);
        }

        #endregion
    }
}
