using System;
using System.Collections.Generic;
using System.Windows.Browser;

namespace SilverlightMappingToolBasic.UI.Extensions.CookieManagement
{
    public enum MapLoadType
    {
        QueryString,
        InitParams,
        Cookie,
        None,
    }

    public class MapLoadParamsManager
    {
        private Guid _mapUid;
        private Guid _domainUid;
        private Guid _nodeUid;
        private string _videoSource;
        //private bool _explorerOnly;


        public Guid MapUid
        {
            get { return _mapUid; }
        }

        public Guid DomainUid
        {
            get { return _domainUid; }
        }

        public Guid NodeUid
        {
            get { return _nodeUid; }
        }

        public string VideoSource
        {
            get { return _videoSource; }
        }

        //public bool ExplorerOnly
        //{
        //    get { return _explorerOnly; }
        //}

        public bool IsLoadMapByGuid 
        {
            get { return DomainUid != Guid.Empty && (NodeUid != Guid.Empty || MapUid != Guid.Empty); }
        }

        public bool IsValid(IDictionary<string, string> @params, MapLoadType type)
        {
            _mapUid = Guid.Empty;
            _nodeUid = Guid.Empty;
            _domainUid = Guid.Empty;
            if (@params.ContainsKey("DomainUid"))
            {
                var domainUid = @params["DomainUid"];
                Guid.TryParse(domainUid, out _domainUid);
            }

            if (@params.ContainsKey("MapUid"))
            {
                var mapUid = @params["MapUid"];
                Guid.TryParse(mapUid, out _mapUid);
            }


            if (@params.ContainsKey("NodeUid"))
            {
                var nodeUid = @params["NodeUid"];
                Guid.TryParse(nodeUid, out _nodeUid);
            }

            if (@params.ContainsKey("VideoSource"))
            {
                _videoSource = @params["VideoSource"];
            }

            //if (@params.ContainsKey("E"))
            //{
            //    _explorerOnly = @params["E"].ToLower() == "true";
            //}
            //else if (@params.ContainsKey("e"))
            //{
            //    _explorerOnly = @params["e"].ToLower() == "true";
            //}

            switch (type)
            {
                case MapLoadType.Cookie:
                    CookieManager.Delete("MapUid");
                    CookieManager.Delete("NodeUid");
                    CookieManager.Delete("DomainUid");
                    break;
                case MapLoadType.QueryString:
                    HtmlPage.Document.QueryString.Clear();
                    break;
            }
            return DomainUid != Guid.Empty && (NodeUid != Guid.Empty || MapUid != Guid.Empty);
        }
    }
}
