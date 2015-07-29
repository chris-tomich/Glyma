using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Browser;
using SilverlightMappingToolBasic.UI.SuperGraph.View.CollapseSupportClasses;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Mouse;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Realign;
using SilverlightMappingToolBasic.UI.SuperGraph.ViewModel;

namespace SilverlightMappingToolBasic.UI.Extensions.CookieManagement
{
    public class MapInformation
    {
        private Dictionary<string, string> _cookieItem;
        private Dictionary<Guid, Point> _nodeLocations;

        public bool IsMapMoved { get; private set; }
        public Guid MapId { get; private set; }

        private Dictionary<string, string> CookieItem
        {
            get
            {
                if (_cookieItem == null)
                {
                    _cookieItem = new Dictionary<string, string>();
                }
                return _cookieItem;
            }
        }

        public double Zoom
        {
            get
            {
                if (CookieItem.ContainsKey("Zoom"))
                {
                    return double.Parse(CookieItem["Zoom"]);
                }
                return 1;
            }
            set
            {
                if (CookieItem.ContainsKey("Zoom"))
                {
                    CookieItem["Zoom"] = value.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    CookieItem.Add("Zoom", value.ToString(CultureInfo.InvariantCulture));
                    Save();
                }
            }
        }

        private void Save()
        {
            CookieManager.Write(MapId.ToString(), ToString(), -1);
        }

        public Dictionary<Guid, Point> NodeLocations
        {
            get
            {
                if (_nodeLocations == null)
                {
                    _nodeLocations = new Dictionary<Guid, Point>();
                }
                return _nodeLocations;
            }
            set { _nodeLocations = value; }
        }
       

        public Point MapLocation
        {
            get
            {
                if (CookieItem.ContainsKey("MapLocation"))
                {
                    var pointvalue = CookieItem["MapLocation"].Replace("(", "").Replace(")", "").Split(',');
                    return new Point(double.Parse(pointvalue[0]), double.Parse(pointvalue[1]));
                }
                return new Point();
            }
            set
            {
                if (!IsMapMoved)
                {
                    IsMapMoved = true;
                }
                if (CookieItem.ContainsKey("MapLocation"))
                {
                    CookieItem["MapLocation"] = string.Format("({0},{1})", value.X, value.Y);
                }
                else
                {
                    CookieItem.Add("MapLocation", string.Format("({0},{1})", value.X, value.Y));
                    Save();
                }
            }
        }

        public RealignStyle AutoRealignStyle
        {
            get
            {
                if (CookieItem.ContainsKey("AutoRealignStyle"))
                {
                    return (RealignStyle)Enum.Parse(typeof(RealignStyle), CookieItem["AutoRealignStyle"], true);
                }
                return RealignStyle.Horizontal;
            }
            set
            {
                if (CookieItem.ContainsKey("AutoRealignStyle"))
                {
                    CookieItem["AutoRealignStyle"] = value.ToString();
                }
                else
                {
                    CookieItem.Add("AutoRealignStyle", value.ToString());
                    Save();
                }
            }
        }

        public Dictionary<Guid, Visibility> Visibilitys
        {
            get
            {
                if (App.UserStyle == UserStyle.Reader)
                {
                    if (_viewerVisibilitys == null)
                    {
                        _viewerVisibilitys = new Dictionary<Guid, Visibility>();
                    }
                    return _viewerVisibilitys;
                }
                else
                {
                    if (_authorVisibilitys == null)
                    {
                        _authorVisibilitys = new Dictionary<Guid, Visibility>();
                    }
                    return _authorVisibilitys;
                }
                
            }
        }

        public Dictionary<Guid, CollapseState> States
        {
            get
            {
                if (App.UserStyle == UserStyle.Reader)
                {
                    if (_viewerStates == null)
                    {
                        _viewerStates = new Dictionary<Guid, CollapseState>();
                    }
                    return _viewerStates;
                }
                else
                {
                    if (_authorStates == null)
                    {
                        _authorStates = new Dictionary<Guid, CollapseState>();
                    }
                    return _authorStates;
                }
            }
        }

        private Dictionary<Guid, CollapseState> _viewerStates;
        private Dictionary<Guid, CollapseState> _authorStates;

        private Dictionary<Guid, Visibility> _viewerVisibilitys;
        private Dictionary<Guid, Visibility> _authorVisibilitys; 

        public void SetVisibility(Guid nodeId, Visibility visibility)
        {
            if (Visibilitys.ContainsKey(nodeId))
            {
                Visibilitys[nodeId] = visibility;
            }
            else
            {
                Visibilitys.Add(nodeId, visibility);
            }
            
        }

        public void SetCollapseState(Guid nodeId, CollapseState collapseState)
        {
            if (States.ContainsKey(nodeId))
            {
                States[nodeId] = collapseState;
            }
            else
            {
                States.Add(nodeId, collapseState);
            }
            
        }

        public MapInformation(Guid mapId, string str = null)
        {
            IsMapMoved = false;
            MapId = mapId;
            if (str != null)
            {
                str = HttpUtility.UrlDecode(str);
                var items = str.Split(';');
                foreach (var pair in items)
                {
                    string[] keyValuePair = pair.Split('=');
                    if (keyValuePair.Length == 2 && !string.IsNullOrWhiteSpace(keyValuePair[1]))
                    {
                        CookieItem.Add(keyValuePair[0], keyValuePair[1]);
                    }
                }
            }
        }

        public override string ToString()
        {
            string output = string.Empty;
            foreach (var item in CookieItem)
            {
                if (output == string.Empty)
                {
                    output += string.Format("{0}={1}", item.Key, item.Value);
                }
                else
                {
                    output += string.Format(";{0}={1}", item.Key, item.Value);
                }
            }
            return HttpUtility.UrlEncode(output);
        }

        public void Clear()
        {
            if (_authorStates != null)
            {
                _authorStates.Clear();
            }

            if (_authorVisibilitys != null)
            {
                _authorVisibilitys.Clear();
            }

            if (_viewerStates != null)
            {
                _viewerStates.Clear();
            }

            if (_viewerVisibilitys != null)
            {
                _viewerVisibilitys.Clear();
            }

        }
    }
}
