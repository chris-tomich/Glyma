using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IoC;

namespace NodeService
{
    public class CompendiumTransclusionRelationship : IRelationship
    {
        private List<IDescriptor> _descriptors;
        private INode _toNode;
        private INode _fromNode;
        private INode _mapNode;
        private INode _transclusionNode;
        private List<string> _notes;
        private List<string> _attachments;

        public CompendiumTransclusionRelationship()
        {
            _descriptors = new List<IDescriptor>();
            _notes = new List<string>();
            _attachments = new List<string>();
            RelationshipType = IoCContainer.GetInjectionInstance().GetInstance<CompendiumNodeRelationshipBaseType>();
        }

        public CompendiumTransclusionRelationship(IRelationship relationship, CompendiumViewRelationship viewRelationship, INode toNode, INode fromNode, INode transclusionNode, INode mapNode)
            : this()
        {
            _toNode = toNode;
            IDescriptor toDescriptor = new CompendiumRelationshipDescriptor(toNode, this, IoCContainer.GetInjectionInstance().GetInstance<CompendiumToDescriptor>());
            _descriptors.Add(toDescriptor);
            _toNode.AddDescriptor(toDescriptor);

            _fromNode = fromNode;
            IDescriptor fromDescriptor = new CompendiumRelationshipDescriptor(fromNode, this, IoCContainer.GetInjectionInstance().GetInstance<CompendiumFromDescriptor>());
            _descriptors.Add(fromDescriptor);
            _fromNode.AddDescriptor(fromDescriptor);

            _transclusionNode = transclusionNode;
            IDescriptor transclusionNodeDescriptor = new CompendiumRelationshipDescriptor(transclusionNode, this, IoCContainer.GetInjectionInstance().GetInstance<CompendiumTransclusionNodeDescriptor>());
            _descriptors.Add(transclusionNodeDescriptor);
            _transclusionNode.AddDescriptor(transclusionNodeDescriptor);

            _mapNode = mapNode;
            IDescriptor mapDescriptor = new CompendiumRelationshipDescriptor(mapNode, this, IoCContainer.GetInjectionInstance().GetInstance<CompendiumTransclusionMapDescriptor>());
            _descriptors.Add(mapDescriptor);
            _mapNode.AddDescriptor(mapDescriptor);

            this.Id = relationship.Id;
            this.Created = relationship.Created;
            this.CreatedBy = relationship.CreatedBy;
            this.LastModified = relationship.LastModified;
            this.Name = relationship.Name;
            this.XPosition = viewRelationship.XPosition;
            this.YPosition = viewRelationship.YPosition;
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

        public INode MapNode
        {
            get
            {
                return _mapNode;
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

        #region IRelationship

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

        public IRelationshipType RelationshipType
        {
            get;
            set;
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
