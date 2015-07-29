using System;
using System.Net;

namespace SilverlightMappingToolBasic.Compendium
{
    public class CompendiumNote// : INodeNote
    {
        public CompendiumNote()
        {
        }

        #region INodeNote Members

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

        public string[] Pages
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

        public void AddPage(string pageData)
        {
            throw new NotImplementedException();
        }

        public void InsertPage(string pageData, int page)
        {
            throw new NotImplementedException();
        }

        public void RemovePage(int page)
        {
            throw new NotImplementedException();
        }

        public void RemovePage(string pageData)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
