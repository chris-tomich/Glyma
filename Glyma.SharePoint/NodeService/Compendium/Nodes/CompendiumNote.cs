using System;
using System.Collections.Generic;
using System.Net;

namespace NodeService
{
    public class CompendiumNote : INodeNote
    {
        public List<string> _pages;

        public CompendiumNote()
        {
            _pages = new List<string>();
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
                return _pages.ToArray();
            }
            set
            {
                _pages.Clear();
                _pages.AddRange(value);
            }
        }

        public void AddPage(string pageData)
        {
            _pages.Add(pageData);
        }

        public void InsertPage(string pageData, int page)
        {
            int index = page - 1;
            if (index < 0)
            {
                index = 0;
            }
            if (index > _pages.Count)
            {
                //fill with empty pages
                int initialSize = _pages.Count;
                for (int i = 0; i < (index - initialSize); i++)
                {
                    _pages.Add(string.Empty);
                }
            }
            _pages.Insert(index, pageData);
        }

        public void RemovePage(int page)
        {
            int index = page - 1;
            if (index < 0) 
            {
                index = 0;
            }
            if (index < _pages.Count)
            {
                _pages.RemoveAt(index);
            }
        }

        public bool Equals(IStorageElement secondElement)
        {
            return Id.Equals(secondElement.Id);
        }

        #endregion
    }
}
