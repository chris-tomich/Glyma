using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SevenSigma.MappingTool
{
    public interface INodeControlConnector
    {
        void AddListener(string clientID);
    }
}
