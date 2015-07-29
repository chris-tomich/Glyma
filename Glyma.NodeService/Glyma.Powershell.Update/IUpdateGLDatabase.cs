using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Glyma.Powershell.Base;

namespace Glyma.Powershell.Update
{
    interface IUpdateGLDatabase : IGLCmdletBase
    {
        string DatabaseServer { get; set; }
        string MapDatabaseName { get; set; }
        string TransactionDatabaseServer { get; set; }
        string TransactionDatabaseName { get; set; }
    }
}
