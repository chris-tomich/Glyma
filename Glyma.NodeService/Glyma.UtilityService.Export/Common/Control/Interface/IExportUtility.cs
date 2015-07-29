using System;
using System.Collections.Generic;
using System.ComponentModel;
using Glyma.UtilityService.Export.Common.EventArgs;
using Glyma.UtilityService.Export.IBIS.Common.Model.Glyma;
using TransactionalNodeService.Proxy.Universal;

namespace Glyma.UtilityService.Export.Common.Control.Interface
{
    public interface IExportUtility
    {
        void ExportMap(Guid domainUid, Guid rootMapUid, Dictionary<string, string> exportProperties, IEnumerable<Guid> selectedNodes, object userState);

        event EventHandler<ExportCompletedEventArgs> ExportCompleted;
        event EventHandler<ProgressChangedEventArgs> ProgressChanged;
        event EventHandler<ExceptionRaisedEventArgs> ExceptionRaised;
        
        Guid RootMapId { get;  }

        Guid DomainId { get;  }

        GlymaMap RootMap { get; }

        Dictionary<string, string> ExportProperties { get; }

        IEnumerable<Guid> SelectedNodes { get; }

        object UserState { get; }

        string OutputFileLocation { get; }

        string FileExtension { get; set; }

        IMapManager MapManager { get; }

        Dictionary<Guid, INode> MapQueue { get; }

        Dictionary<Guid, INode> MapCompleted { get; }


        string GetExportProperty(string key);

    }
}
