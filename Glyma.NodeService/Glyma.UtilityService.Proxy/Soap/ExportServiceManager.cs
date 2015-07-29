using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using Glyma.UtilityService.Proxy.Service;
using System.Collections;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Glyma.UtilityService.Proxy
{
    public class ExportServiceManager : IExportServiceManager
    {
        private Proxy.DeleteExportJobEventRegister _deleteExportJobEventRegister;
        private Proxy.CreateExportJobEventRegister _createExportJobEventRegister;
        private Proxy.GetExportJobsEventRegister _getExportJobsEventRegister;
        private Proxy.GetExportJobsForMapTypeEventRegister _getExportJobsForMapTypeEventRegister;
        private Proxy.IsExportingAvailableEventRegister _isExportingAvailableEventRegister;

        private Service.UtilityServiceManagerClient _utilityService = null;

        public ExportServiceManager(Service.UtilityServiceManagerClient serviceProxy)
        {
            if (serviceProxy != null)
            {
                ServiceProxy = serviceProxy;
                ServiceEndpoint = ServiceProxy.Endpoint; //record the 

                RegisterServiceEventHandlers();
            }
        }

        private void RegisterServiceEventHandlers()
        {
            ServiceProxy.GetExportJobsCompleted += OnGetExportJobsCompleted;
            ServiceProxy.GetExportJobsForMapTypeCompleted += OnGetExportJobsForMapTypeCompleted;
            ServiceProxy.CreateExportJobCompleted += OnCreateExportJobCompleted;
            ServiceProxy.DeleteExportJobCompleted += OnDeleteExportJobCompleted;
            ServiceProxy.IsExportingAvailableCompleted += OnIsExportingAvailableCompleted;
        }

        protected Service.UtilityServiceManagerClient ServiceProxy
        {
            get
            {
                if (_utilityService != null && _utilityService.State != System.ServiceModel.CommunicationState.Faulted)
                {
                    return _utilityService;
                }
                else
                {
                    if (ServiceEndpoint != null)
                    {
                        //If the client is in a faulted state recreate it with the original binding and address information.
                        _utilityService = new UtilityServiceManagerClient(ServiceEndpoint.Binding, ServiceEndpoint.Address);
                    }
                }
                return _utilityService;
            }
            set
            {
                _utilityService = value;
            }
        }

        private ServiceEndpoint ServiceEndpoint
        {
            get;
            set;
        }

        public Proxy.GetExportJobsEventRegister GetExportJobsCompleted
        {
            get
            {
                if (_getExportJobsEventRegister == null)
                {
                    _getExportJobsEventRegister = new GetExportJobsEventRegister();
                }
                return _getExportJobsEventRegister;
            }
        }

        public Proxy.GetExportJobsForMapTypeEventRegister GetExportJobsForMapTypeCompleted
        {
            get
            {
                if (_getExportJobsForMapTypeEventRegister == null)
                {
                    _getExportJobsForMapTypeEventRegister = new GetExportJobsForMapTypeEventRegister();
                }
                return _getExportJobsForMapTypeEventRegister;
            }
        }

        public Proxy.CreateExportJobEventRegister CreateExportJobCompleted
        {
            get
            {
                if (_createExportJobEventRegister == null)
                {
                    _createExportJobEventRegister = new CreateExportJobEventRegister();
                }
                return _createExportJobEventRegister;
            }
        }

        public Proxy.DeleteExportJobEventRegister DeleteExportJobCompleted
        {
            get
            {
                if (_deleteExportJobEventRegister == null)
                {
                    _deleteExportJobEventRegister = new DeleteExportJobEventRegister();
                }
                return _deleteExportJobEventRegister;
            }
        }

        public Proxy.IsExportingAvailableEventRegister IsExportingAvailableCompleted
        {
            get
            {
                if (_isExportingAvailableEventRegister == null)
                {
                    _isExportingAvailableEventRegister = new IsExportingAvailableEventRegister();
                }
                return _isExportingAvailableEventRegister;
            }
        }

        public void GetExportJobsAsync(Guid domainId, Guid rootMapId)
        {
            if (ServiceProxy != null)
            {
                ServiceProxy.GetExportJobsAsync(domainId, rootMapId, rootMapId);
            }
        }

        public void GetExportJobsForMapTypeAsync(MapType mapType, Guid domainId, Guid rootMapId)
        {
            if (ServiceProxy != null)
            {
                Service.MapType serviceMapType = ClientObjectConverter.ConvertClientMapType(mapType);
                ServiceProxy.GetExportJobsForMapTypeAsync(serviceMapType, domainId, rootMapId, rootMapId);
            }
        }

        public void CreateExportJobAsync(Guid domainId, Guid rootMapId, Dictionary<string, string> exportProperties, Proxy.MapType mapType, Proxy.ExportType exportType)
        {
            if (ServiceProxy != null)
            {
                Service.ExportType serviceExportType = ClientObjectConverter.ConvertClientExportType(exportType);
                Service.MapType serviceMapType = ClientObjectConverter.ConvertClientMapType(mapType);
                ServiceProxy.CreateExportJobAsync(domainId, rootMapId, exportProperties, serviceMapType, serviceExportType, rootMapId);
            }
        }

        public void DeleteExportJobAsync(Proxy.IExportJob exportJob)
        {
            if (ServiceProxy != null)
            {
                Service.ExportJob convertedExportJob = ClientObjectConverter.ConvertClientExportJob(exportJob);
                ServiceProxy.DeleteExportJobAsync(convertedExportJob, exportJob.Id);
            }
        }

        public void IsExportingAvailableAsync()
        {
            if (ServiceProxy != null)
            {
                ServiceProxy.IsExportingAvailableAsync();
            }
        }

        private void OnGetExportJobsCompleted(object sender, GetExportJobsCompletedEventArgs e)
        {
            if (GetExportJobsCompleted != null)
            {
                Guid context = Guid.Empty;
                if (e.UserState != null)
                {
                    context = (Guid)e.UserState; //should be the root map id
                }

                Proxy.ResultEventArgs<ExportJobCollection> eventArgs = new Proxy.ResultEventArgs<ExportJobCollection>();
                if (e.Error != null)
                {
                    eventArgs.HasError = true;
                    eventArgs.ErrorMessage = e.Error.Message;
                }
                else
                {
                    IDictionary<Guid, Proxy.IExportJob> exportJobs = ServerObjectConverter.ConvertExportJobsCollection(e.Result.ExportJobs);
                    eventArgs.Result = new ExportJobCollection(exportJobs);
                }

                GetExportJobsCompleted.FireEvent(context, this, eventArgs);
            }
        }

        private void OnGetExportJobsForMapTypeCompleted(object sender, GetExportJobsForMapTypeCompletedEventArgs e)
        {
            if (GetExportJobsForMapTypeCompleted != null)
            {
                Guid context = Guid.Empty;
                if (e.UserState != null)
                {
                    context = (Guid)e.UserState; //should be the root map id
                }

                Proxy.ResultEventArgs<ExportJobCollection> eventArgs = new Proxy.ResultEventArgs<ExportJobCollection>();
                if (e.Error != null)
                {
                    eventArgs.HasError = true;
                    eventArgs.ErrorMessage = e.Error.Message;
                }
                else
                {
                    IDictionary<Guid, Proxy.IExportJob> exportJobs = ServerObjectConverter.ConvertExportJobsCollection(e.Result.ExportJobs);
                    eventArgs.Result = new ExportJobCollection(exportJobs);
                }

                GetExportJobsForMapTypeCompleted.FireEvent(context, this, eventArgs);
            }
        }

        private void OnCreateExportJobCompleted(object sender, CreateExportJobCompletedEventArgs e)
        {
            if (CreateExportJobCompleted != null)
            {
                Guid context = Guid.Empty;
                if (e.UserState != null)
                {
                    context = (Guid)e.UserState; //should be the root map id
                }

                Proxy.ResultEventArgs<IExportJob> eventArgs = new Proxy.ResultEventArgs<IExportJob>();
                if (e.Error != null)
                {
                    eventArgs.HasError = true;
                    eventArgs.ErrorMessage = e.Error.Message;
                }
                else
                {
                    eventArgs.Result = ServerObjectConverter.ConvertServerExportJob(e.Result.ExportJob);
                }

                CreateExportJobCompleted.FireEvent(context, this, eventArgs);
            }
        }

        private void OnDeleteExportJobCompleted(object sender, DeleteExportJobCompletedEventArgs e)
        {
            if (DeleteExportJobCompleted != null)
            {
                Guid context = Guid.Empty;
                if (e.UserState != null)
                {
                    context = (Guid)e.UserState;  //should be the export job id
                }

                Proxy.ResultEventArgs<IExportJob> eventArgs = new Proxy.ResultEventArgs<IExportJob>();
                if (e.Error != null)
                {
                    eventArgs.HasError = true;
                    eventArgs.ErrorMessage = e.Error.Message;
                }
                else
                {
                    if (e.Result.ExportJob != null)
                    {
                        eventArgs.Result = ServerObjectConverter.ConvertServerExportJob(e.Result.ExportJob);
                    }
                    else
                    {
                        eventArgs.HasError = true;
                        eventArgs.ErrorMessage = "The details of ExportJob that was deleted was not returned with the result of the delete operation.";
                    }
                }

                DeleteExportJobCompleted.FireEvent(context, this, eventArgs);
            }
        }

        private void OnIsExportingAvailableCompleted(object sender, IsExportingAvailableCompletedEventArgs e)
        {
            if (IsExportingAvailableCompleted != null)
            {
                Proxy.ResultEventArgs<bool> eventArgs = new Proxy.ResultEventArgs<bool>();
                if (e.Error != null)
                {
                    eventArgs.HasError = true;
                    eventArgs.ErrorMessage = e.Error.Message;
                }
                else
                {
                    eventArgs.Result = e.Result.IsAvailable;
                }

                IsExportingAvailableCompleted.FireEvent(this, eventArgs);
            }
        }
    }
}
