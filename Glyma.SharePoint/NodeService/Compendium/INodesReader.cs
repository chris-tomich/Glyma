using System;
using System.Net;

namespace NodeService
{
    public interface INodesReader
    {
        INode[] GetAllNodes();
    }
}
