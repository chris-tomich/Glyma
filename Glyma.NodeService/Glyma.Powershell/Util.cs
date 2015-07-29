using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glyma.Powershell
{
    class Util
    {
        internal static Exception FindLastException(Exception ex)
        {
            Exception lastException = ex;
            if (ex.InnerException != null)
            {
                lastException = Util.FindLastException(ex.InnerException);
            }
            return lastException;
        }
    }
}
