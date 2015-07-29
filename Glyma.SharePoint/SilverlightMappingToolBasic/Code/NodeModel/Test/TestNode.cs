using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SilverlightMappingToolBasic.Test
{
    public class TestNode : INode
    {
        public TestNode()
        {
        }

        #region INode Members

        public string[] Notes
        {
            get;
            set;
        }

        public string[] Attachments
        {
            get;
            set;
        }

        public INodeType NodeType
        {
            get;
            set;
        }

        public IRelationship[] Relationships
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void AddNote(string note)
        {
        }

        public void AddAttachment(string attachment)
        {
        }

        public void AddDescriptor(IDescriptor descriptor)
        {
            throw new NotImplementedException();
        }

        public void RemoveNote(string note)
        {
        }

        public void RemoveAttachment(string attachment)
        {
        }

        public void RemoveDescriptor(IDescriptor descriptor)
        {
            throw new NotImplementedException();
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
            return Id.Equals(secondElement.Id);
        }

        #endregion

        #region INode Members

        public IDescriptor[] Descriptors
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}
