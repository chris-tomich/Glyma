using System;
using System.Collections.Generic;
using System.Linq;
using Glyma.UtilityService.Proxy;
using SilverlightMappingToolBasic.UI.Extensions.Json;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.ManagementConsole.ViewModel
{
    public class ExportOption: ViewModelBase
    {

        private Dictionary<string, string> _value;
        private readonly bool _isInitialised;
        private bool _showDescription;

        private bool _showImage;

        private bool _showVideo;

        private bool _showPages;

        public ExportType ExportType { get; set; }

        public MapType MapType { get; set; }

        public Dictionary<string, string> Value
        {
            get
            {
                if (!_isInitialised)
                {
                    if (_value == null)
                    {
                        _value = new Dictionary<string, string>();
                    }
                }
                else
                {
                    _value = new Dictionary<string, string>();
                    if (ShowDescription)
                    {
                        _value.Add("ShowDescription","True");
                    }

                    if (ShowImage)
                    {
                        _value.Add("ShowImage", "True");
                    }

                    if (ShowVideo)
                    {
                        _value.Add("ShowVideo", "True");
                    }

                    if (ShowPages)
                    {
                        _value.Add("ShowPages", "True");
                    }

                    if (SelectedNodes.Any())
                    {
                        _value.Add("SelectedNodes", SelectedNodes.ToJson());
                    }
                }
                return _value;
            }
            set { _value = value; }
        }

        private IEnumerable<Guid> _selectedNodes;

        public IEnumerable<Guid> SelectedNodes
        {
            get
            {
                if (_selectedNodes == null)
                {
                    _selectedNodes = new List<Guid>();
                }
                return _selectedNodes;
            }

            set { _selectedNodes = value; }
        }

        public bool ShowDescription
        {
            get { return _showDescription; }
            set
            {
                if (_showDescription != value)
                {
                    _showDescription = value;
                    OnNotifyPropertyChanged("ShowDescription");
                }
            }
        }

        public bool ShowPages
        {
            get { return _showPages; }
            set
            {
                if (_showPages != value)
                {
                    _showPages = value;
                    OnNotifyPropertyChanged("ShowPages");
                }
            }
        }

        public bool ShowImage
        {
            get { return _showImage; }
            set
            {
                if (_showImage != value)
                {
                    _showImage = value;
                    OnNotifyPropertyChanged("ShowImage");
                }
            }
        }

        public bool ShowVideo
        {
            get { return _showVideo; }
            set
            {
                if (_showVideo != value)
                {
                    _showVideo = value;
                    OnNotifyPropertyChanged("ShowVideo");
                }
            }
        }

        public ExportOption(Dictionary<string, string> value, ExportType exportType, MapType mapType)
            : this(exportType, mapType)
        {
            Value = value;
            ShowDescription = value.GetPropertyBooleanValue("ShowDescription");
            ShowImage = value.GetPropertyBooleanValue("ShowImage");
            ShowVideo = value.GetPropertyBooleanValue("ShowVideo");
            ShowPages = value.GetPropertyBooleanValue("ShowPages");

            var selectedNodes = value.GetPropertyStringValue("SelectedNodes");
            if (!string.IsNullOrEmpty(selectedNodes))
            {
                SelectedNodes = selectedNodes.ToGuidList();
            }
            
        }

        public ExportOption(ExportType exportType, MapType mapType)
        {
            _isInitialised = true;
            ExportType = exportType;
            MapType = mapType;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ExportOption) obj);
        }

        public bool Equals(ExportOption p)
        {
            // If parameter is null return false:
            if (p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return ((ExportType == p.ExportType) && (MapType == p.MapType) && 
                (ShowDescription == p.ShowDescription) && (ShowPages == p.ShowPages) && 
                (ShowImage == p.ShowImage) && (ShowVideo == p.ShowVideo) && 
                (SelectedNodes.ToJson() == p.SelectedNodes.ToJson()));
        }
    }
}
