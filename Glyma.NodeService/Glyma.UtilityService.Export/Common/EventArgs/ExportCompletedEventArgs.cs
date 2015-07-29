using System;
using Glyma.UtilityService.Common.Model;

namespace Glyma.UtilityService.Export.Common.EventArgs
{
    public class ExportCompletedEventArgs : System.EventArgs
    {
        public string FileLocation { get; private set; }

        public ExportStatus Status { get; private set; }

        public string ErrorMessage { get; private set; }

        public object UserState { get; private set; }

        public string MapName { get; private set; }


        public ExportCompletedEventArgs(string fileLocation, object userState, string mapname, ExportStatus? status = null, string errorMessage = null)
        {
            UserState = userState;
            FileLocation = fileLocation;
            MapName = mapname;

            if (status.HasValue)
            {
                Status = status.Value;
            }
            else
            {
                Status = ExportStatus.Completed;
            }
            ErrorMessage = errorMessage;
        }

    }
}
