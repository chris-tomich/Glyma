using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glyma.Security.Model
{
    public interface IRight
    {
        int RightId { get; }
        string DisplayName { get; }
    }
}
