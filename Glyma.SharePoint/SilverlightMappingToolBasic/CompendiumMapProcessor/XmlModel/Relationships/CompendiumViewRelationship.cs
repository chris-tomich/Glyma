using System;
using System.Collections.Generic;
using System.Xml;
using SilverlightMappingToolBasic.CompendiumMapProcessor.XmlModel.Descriptors;
using SilverlightMappingToolBasic.CompendiumMapProcessor.XmlModel.DescriptorTypes;
using SilverlightMappingToolBasic.CompendiumMapProcessor.XmlModel.RelationshipTypes;
using SimpleIoC;

namespace SilverlightMappingToolBasic.CompendiumMapProcessor.XmlModel.Relationships
{
    public class CompendiumViewRelationship : IRelationship
    {
        private string _from;
        private string _to;
        private INode _toNode;
        private INode _fromNode;
        private List<IDescriptor> _descriptors;
        private List<string> _notes;
        private List<string> _attachments;
        private bool _isRootView = false;

        public CompendiumViewRelationship()
        {
            _descriptors = new List<IDescriptor>();
            _notes = new List<string>();
            _attachments = new List<string>();
            RelationshipType = IoCContainer.GetInjectionInstance().GetInstance<CompendiumMapRelationshipBaseType>();
        }

        public void ConsumeViewXml(XmlReader xml, string domainNodeId)
        {
            if (xml != null && xml.NodeType == XmlNodeType.Element && xml.Name == "view")
            {
                #region Attribute Values
                #region Id
                string nodeViewRefId = string.Empty;

                if (xml.MoveToAttribute("viewref"))
                {
                    ViewId = xml.Value;
                    nodeViewRefId += xml.Value;
                }

                if (xml.MoveToAttribute("noderef"))
                {
                    nodeViewRefId += xml.Value;
                }

                Id = nodeViewRefId;
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

                #region CreatedBy - Not Currently Used
                /*
                if (xml.MoveToAttribute("author"))
                {
                    CreatedBy = xml.Value;
                }
                */
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

                #region Name - Not Currently Used
                /*
                if (xml.MoveToAttribute("label"))
                {
                    Name = xml.Value;
                }
                */
                #endregion

                #region LastModifiedBy - Not Currently Used
                /*
                if (xml.MoveToAttribute("lastModificationAuthor"))
                {
                    LastModifiedBy = xml.Value;
                }
                */
                #endregion

                #region XPosition
                if (xml.MoveToAttribute("XPosition"))
                {
                    string xPositionXmlValue = xml.Value;
                    int xPosition;

                    if (int.TryParse(xPositionXmlValue, out xPosition))
                    {
                        XPosition = xPosition;
                    }
                }
                #endregion

                #region YPosition
                if (xml.MoveToAttribute("YPosition"))
                {
                    string yPositionXmlValue = xml.Value;
                    int yPosition;

                    if (int.TryParse(yPositionXmlValue, out yPosition))
                    {
                        YPosition = yPosition;
                    }
                }
                #endregion

                #endregion

                if (xml.MoveToAttribute("noderef"))
                {
                    _from = xml.Value;
                }

                if (xml.MoveToAttribute("viewref"))
                {
                    _to = xml.Value;
                    if (_to == domainNodeId)
                    {
                        _isRootView = true;
                    }
                }

                while (xml.Read())
                {
                    if (xml.NodeType == XmlNodeType.Element)
                    {
                    }
                    else if (xml.NodeType == XmlNodeType.EndElement && xml.Name == "view")
                    {
                        break;
                    }
                }
            }
        }

        public int XPosition
        {
            get;
            set;
        }

        public int YPosition
        {
            get;
            set;
        }

        public INode ToNode
        {
            get
            {
                return _toNode;
            }
        }

        public INode FromNode
        {
            get
            {
                return _fromNode;
            }
        }

        public bool IsRootView
        {
            get
            {
                return _isRootView;
            }
        }

        public string ViewId
        {
            get;
            private set;
        }

        public void BuildRelationships(Dictionary<string, INode> nodeList)
        {
            bool nodeListContainsFrom = nodeList.ContainsKey(_from);
            bool nodeListContainsTo = nodeList.ContainsKey(_to);
            INode fromNode = null;
            INode toNode = null;

            if (nodeListContainsFrom && nodeListContainsTo)
            {
                fromNode = nodeList[_from];
                _fromNode = fromNode;
                IDescriptor fromDescriptor = new CompendiumRelationshipDescriptor(fromNode, this, IoCContainer.GetInjectionInstance().GetInstance<CompendiumFromDescriptor>());
                _descriptors.Add(fromDescriptor);
                fromNode.AddDescriptor(fromDescriptor);

                toNode = nodeList[_to];
                _toNode = toNode;
                IDescriptor toDescriptor = new CompendiumRelationshipDescriptor(toNode, this, IoCContainer.GetInjectionInstance().GetInstance<CompendiumToDescriptor>());
                _descriptors.Add(toDescriptor);
                toNode.AddDescriptor(toDescriptor);
            }
        }

        #region INodeRelationship Members

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

        public void AddDescriptor(IDescriptor descriptor)
        {
            _descriptors.Add(descriptor);
        }

        public void RemoveDescriptor(IDescriptor descriptor)
        {
            _descriptors.Remove(descriptor);
        }

        public void AddNote(string note)
        {
            _notes.Add(note);
        }

        public void AddAttachment(string attachment)
        {
            _attachments.Add(attachment);
        }

        public void RemoveNote(string note)
        {
            _notes.Remove(note);
        }

        public void RemoveAttachment(string attachment)
        {
            _notes.Remove(attachment);
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
