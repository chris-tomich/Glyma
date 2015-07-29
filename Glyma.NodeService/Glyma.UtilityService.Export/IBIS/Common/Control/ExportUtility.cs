using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using Glyma.UtilityService.Export.Common;
using Glyma.UtilityService.Export.Common.Control.Interface;
using Glyma.UtilityService.Export.Common.EventArgs;
using Glyma.UtilityService.Export.IBIS.Common.Extension;
using Glyma.UtilityService.Export.IBIS.Common.Model;
using Glyma.UtilityService.Export.IBIS.Common.Model.Glyma;
using TransactionalNodeService.Proxy.Universal;
using TransactionalNodeService.Proxy.Universal.EventArgs;
using Glyma.UtilityService.Common.Model;

namespace Glyma.UtilityService.Export.IBIS.Common.Control
{
    public abstract class ExportUtility: IExportUtility
    {
        #region private 
        private string _fileName;
        private Dictionary<Guid, INode> _mapCompleted;
        private Dictionary<Guid, INode> _mapQueue;
        private Dictionary<string, string> _exportProperties;
        private IEnumerable<Guid> _selectedNodes; 
        #endregion

        #region eventhandler

        public event EventHandler<ExportCompletedEventArgs> ExportCompleted;
        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;
        public event EventHandler<ExceptionRaisedEventArgs> ExceptionRaised;

        #endregion

        public GlymaMap RootMap { get; protected set; }

        public IEnumerable<Guid> SelectedNodes {
            get
            {
                if (_selectedNodes == null)
                {
                    _selectedNodes = new List<Guid>();
                }
                return _selectedNodes;
            }
            protected set { _selectedNodes = value; } 
        } 

        public Guid RootMapId { get; protected set; }
        public Guid DomainId { get; protected set; }

        public Dictionary<string, string> ExportProperties
        {
            get
            {
                if (_exportProperties == null)
                {
                    _exportProperties = new Dictionary<string, string>();
                }
                return _exportProperties;
            }
            protected set { _exportProperties = value; }
        }

        public object UserState { get; protected set; }
        public string FileExtension { get; set; }
        public IMapManager MapManager { get; protected set; }

        public Dictionary<Guid, INode> MapQueue
        {
            get
            {
                if (_mapQueue == null)
                {
                    _mapQueue = new Dictionary<Guid, INode>();
                }
                return _mapQueue;
            }
        }

        
        public Dictionary<Guid, INode> MapCompleted
        {
            get
            {
                if (_mapCompleted == null)
                {
                    _mapCompleted = new Dictionary<Guid, INode>();
                }
                return _mapCompleted;
            }
        }

        protected string FileName
        {
            get
            {
                if (_fileName == null)
                {
                    _fileName = StringHelper.RandomString(10);
                }
                return _fileName;
            }
        }

        public string OutputFileLocation
        {
            get { return Path.GetTempPath() + @"/" + FileName +"." + FileExtension;}
        }

        protected ExportUtility(IMapManager mapmanager)
        {
            MapManager = mapmanager;
        }

        public void ExportMap(Guid domainUid, Guid rootMapUid, Dictionary<string, string> exportProperties, IEnumerable<Guid> selectedNodes, object userState)
        {
            DomainId = domainUid;
            RootMapId = rootMapUid;
            ExportProperties = exportProperties;
            SelectedNodes = selectedNodes;

            UserState = userState;
            try
            {
                MapManager.QueryMapByIdCompleted.RegisterEvent(RootMapId, OnQueryMapByIdCompleted);
                MapManager.QueryMapByIdAsync(DomainId, RootMapId);
            }
            catch (Exception ex)
            {
                OnExceptionRaised(this, "Error occurred when querying map by id", ex);
            }
        }

        protected abstract bool CreateFile();



        protected virtual void OnExportCompleted(object sender, ExportCompletedEventArgs e)
        {
            try
            {
                bool fileCreated = CreateFile();

                string safeFileName = CreateSafeFileName(RootMap.Name);
                if (fileCreated)
                {
                    var eventArgs = new ExportCompletedEventArgs(e.FileLocation, UserState, safeFileName, e.Status, e.ErrorMessage);
                    if (ExportCompleted != null)
                    {
                        ExportCompleted(sender, eventArgs);
                    }
                }
                else
                {
                    var eventArgs = new ExportCompletedEventArgs(e.FileLocation, UserState, safeFileName, ExportStatus.Error, "Error occurred when creating the export file.");
                    if (ExportCompleted != null)
                    {
                        ExportCompleted(sender, eventArgs);
                    }
                }
            }
            catch (Exception ex)
            {
                OnExceptionRaised(sender, "Error occurred when creating the export file.", ex);
            }
        }

        private string CreateSafeFileName(string mapName)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                mapName = mapName.Replace(c, '_');
            }
            return mapName;
        }

        protected virtual void OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var eventArgs = new ProgressChangedEventArgs(e.ProgressPercentage, UserState);
            if (ProgressChanged != null)
            {
                ProgressChanged(sender, eventArgs);
            }
        }

        protected virtual void OnContainerMapLoaded(INode node)
        {
            RootMap = new GlymaMap(node);
        }

        private void OnQueryMapByIdCompleted(object sender, NodesEventArgs e)
        {
            try
            {
                var context = (Guid)e.Context;
                if (e.Nodes != null && context != null)
                {
                    foreach (INode node in e.Nodes.Values)
                    {
                        if (node.Id == context)
                        {
                            OnContainerMapLoaded(node);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                OnExceptionRaised(sender, "Error occurred when processing the query map by id result", ex);
            }
        }


        protected void MapQuerierOnMapAdded(object sender, MapEventArgs e)
        {
            if (!MapQueue.ContainsKey(e.Map.Id) && !MapCompleted.ContainsKey(e.Map.Id))
            {
                MapQueue.Add(e.Map.Id, e.Map);
                if (ProgressChanged != null)
                {
                    ProgressChanged(this, new ProgressChangedEventArgs(MapCompleted.Count * 100 / (MapQueue.Count + MapCompleted.Count), null));
                }
            }
        }

        protected void MapQuerierOnQueryCompleted(object sender, MapEventArgs e)
        {
            try
            {
                if (MapQueue.ContainsKey(e.Map.Id))
                {
                    MapQueue.Remove(e.Map.Id);
                }

                if (!MapCompleted.ContainsKey(e.Map.Id))
                {
                    MapCompleted.Add(e.Map.Id, e.Map);
                }

                if (MapQueue.Count == 0)
                {
                    OnProgressChanged(this, new ProgressChangedEventArgs(100, null));
                    OnExportCompleted(this, new ExportCompletedEventArgs(OutputFileLocation, UserState, RootMap.Name));
                }
                else
                {
                    OnProgressChanged(this, new ProgressChangedEventArgs(MapCompleted.Count * 100 / (MapQueue.Count + MapCompleted.Count), null));

                    var nextMap = MapQueue.Values.FirstOrDefault();
                    if (nextMap != null)
                    {
                        if (!MapCompleted.ContainsKey(nextMap.Id))
                        {
                            ReadNextMap(nextMap);
                        }
                        else
                        {
                            var mapArgs = new MapEventArgs { Container = e.Container, Map = nextMap };
                            MapQuerierOnQueryCompleted(sender, mapArgs);
                        }
                    }
                    else
                    {
                        throw new NullReferenceException("Map in queue is null");
                    }
                }
            }
            catch (Exception ex)
            {
                OnExceptionRaised(sender, "Error occurred when querying map data", ex);
            }
            
        }


        protected abstract void ReadNextMap(INode nextMap);

        public string GetExportProperty(string key)
        {
            if (ExportProperties.ContainsKey(key))
            {
                return ExportProperties[key];
            }
            return string.Empty;
        }

        protected void OnExceptionRaised(object sender, string errorMessage, Exception ex)
        {
            if (ExceptionRaised != null)
            {
                var eventArgs = new ExceptionRaisedEventArgs {InnerException = ex, UserState = UserState, ErrorMessage = errorMessage};
                ExceptionRaised(sender, eventArgs);
            }
        }
    }
}
