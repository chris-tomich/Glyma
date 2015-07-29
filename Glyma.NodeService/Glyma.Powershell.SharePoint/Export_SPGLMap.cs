using Glyma.Powershell.Model;
using Glyma.Powershell.SharePoint.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace Glyma.Powershell.SharePoint
{
    [Cmdlet(VerbsData.Export, "SPGLMap")]
    public class Export_SPGLMap : PSCmdlet, IDynamicParameters
    {
        [Parameter(Position = 0, Mandatory = true)]
        public string Identity
        {
            get;
            set;
        }

        [Parameter(Position = 1, Mandatory = true)]
        public string ServiceUrl
        {
            get;
            set;
        }

        [Parameter(Position = 2, Mandatory = true)]
        public Map RootMap
        {
            get;
            set;
        }

        [Parameter(Position = 3, Mandatory = true)]
        public string Output
        {
            get;
            set;
        }

        [Parameter(Position = 4, Mandatory = true)]
        public string Username
        {
            get;
            set;
        }

        [Parameter(Position = 5, Mandatory = true)]
        public string Password
        {
            get;
            set;
        }

        private string _exportFormat;
        [Parameter(Position = 6, Mandatory = true)]
        [ValidateNotNullOrEmpty]
        [ValidateSet("Compendium", "Word", "PDF" /*, "GlymaXML"*/)]
        public string ExportFormat
        {
            get
            {
                return _exportFormat;
            }
            set
            {
                _exportFormat = value;
            }
        }

        //private string _mapSchema = "IBIS"; //default value for the non mandatory parameter is IBIS

        //[Parameter(Position = 7, Mandatory = false)]
        //[ValidateSet("IBIS")]
        //public string MapSchema
        //{
        //    get
        //    {
        //        return _mapSchema;
        //    }
        //    set
        //    {
        //        _mapSchema = value;
        //    }
        //}

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
        }

        protected override void ProcessRecord()
        {
            Export_SPGLMapBase exportSPGLMapBase = new Export_SPGLMapBase();
            exportSPGLMapBase.Identity = Identity;
            exportSPGLMapBase.ServiceUrl = ServiceUrl;
            exportSPGLMapBase.Domain = RootMap.Domain.DomainId;
            exportSPGLMapBase.Map = RootMap.NodeId;
            exportSPGLMapBase.Output = Output;
            exportSPGLMapBase.SharePointUsername = Username;
            exportSPGLMapBase.SharePointPassword = Password;
            exportSPGLMapBase.ExportFormat = ExportFormat;
            exportSPGLMapBase.MapSchema = "IBIS";  /*MapSchema*/

            //check if the export format was Word or PDF then this won't be null
            if (dynWordPdfParams != null)
            {
                exportSPGLMapBase.ShowImages = dynWordPdfParams.ShowImages.IsPresent;
                exportSPGLMapBase.ShowDescription = dynWordPdfParams.ShowDescriptions.IsPresent;
                exportSPGLMapBase.ShowVideo = dynWordPdfParams.ShowVideos.IsPresent;
                exportSPGLMapBase.ShowPages = dynWordPdfParams.ShowPages.IsPresent;
            }

            exportSPGLMapBase.ExecuteCmdletBase(this);
        }

        public object GetDynamicParameters()
        {
            if (_exportFormat == "Word" || _exportFormat == "PDF")
            {
                dynWordPdfParams = new Export_SPGLMapWordPdfDynamicParams();
                return dynWordPdfParams;
            }
            return null; //no dynamic params for other formats (yet)
        }
        private Export_SPGLMapWordPdfDynamicParams dynWordPdfParams = null;
    }

    public class Export_SPGLMapWordPdfDynamicParams
    {
        private SwitchParameter _showImages = SwitchParameter.Present;
        private SwitchParameter _showDescriptions = !SwitchParameter.Present;
        private SwitchParameter _showVideos = !SwitchParameter.Present;
        private SwitchParameter _showPages = !SwitchParameter.Present;

        [Parameter(Position = 100, Mandatory = false)]
        public SwitchParameter ShowImages
        {
            get
            {
                return _showImages;
            }
            set
            {
                _showImages = value;
            }
        }

        [Parameter(Position = 101, Mandatory = false)]
        public SwitchParameter ShowDescriptions
        {
            get
            {
                return _showDescriptions;
            }
            set
            {
                _showDescriptions = value;
            }
        }

        [Parameter(Position = 102, Mandatory = false)]
        public SwitchParameter ShowVideos
        {
            get
            {
                return _showVideos;
            }
            set
            {
                _showVideos = value;
            }
        }

        [Parameter(Position = 103, Mandatory = false)]
        public SwitchParameter ShowPages
        {
            get
            {
                return _showPages;
            }
            set
            {
                _showPages = value;
            }
        }
    }
}
