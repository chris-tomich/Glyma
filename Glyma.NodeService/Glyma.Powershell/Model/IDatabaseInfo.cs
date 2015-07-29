using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glyma.Powershell.Model
{
    public interface IDatabaseInfo
    {
        string DatabaseServer { get; set; }
        string DatabaseName { get; set; }
        string ConnectionString { get; }
    }
}
