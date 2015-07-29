using System;

namespace Glyma.UtilityService.Export.Common.EventArgs
{
    public class ExceptionRaisedEventArgs : System.EventArgs
    {
        public string ErrorMessage { get; set; }

        public Exception InnerException { get; set; }

        public object UserState { get; set; }
    }
}
