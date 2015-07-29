using System;

namespace NodeService
{
    public interface INodeNote : IStorageElement
    {
        string[] Pages { get; set; }

        void AddPage(string pageData);
        void InsertPage(string pageData, int page);
        void RemovePage(int page);
    }
}
