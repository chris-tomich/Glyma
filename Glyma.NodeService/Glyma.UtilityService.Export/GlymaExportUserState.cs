using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Glyma.UtilityService.Export
{
    public class GlymaExportUserState
    {
        public GlymaExportUserState(object stateObject)
        {
            Completed = new ManualResetEvent(false);
            StateObject = stateObject;
        }

        public ManualResetEvent Completed
        {
            get;
            private set;
        }

        public object StateObject
        {
            get;
            private set;
        }

        public bool UseVerboseLogging
        {
            get;
            set;
        }
    }
}
