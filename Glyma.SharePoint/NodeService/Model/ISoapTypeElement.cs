using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NodeService
{
    public interface ISoapTypeElement
    {
        Guid Id { get; set; }
        string Name { get; set; }
    }
}