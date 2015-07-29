using System;
using System.Net;
using System.Collections.Generic;
using IoC;
using WebSvcNs = SilverlightMappingToolBasic.CompendiumSharePointService;

namespace SilverlightMappingToolBasic.Compendium
{
    public class CompendiumSharePointServiceNavigator// : INodeNavigator
    {
        private List<INode> _allNodes;
        private List<INode> _currentNodes;
        private WebSvcNs.IMappingToolWebService _compendiumSharePointSvc;

        public CompendiumSharePointServiceNavigator()
        {
            _currentNodes = new List<INode>();
            _compendiumSharePointSvc = new WebSvcNs.MappingToolWebServiceClient();
        }

        public void GetNodesCallback(IAsyncResult result)
        {
            Dictionary<string, WebSvcNs.Node> nodes = _compendiumSharePointSvc.EndGetAllNodes(result);

            foreach (KeyValuePair<string, WebSvcNs.Node> nodeDetails in nodes)
            {
                string nodeId = nodeDetails.Key;
                WebSvcNs.Node node = nodeDetails.Value;
                WebServiceCompendiumNode compendiumNode = new WebServiceCompendiumNode();

                compendiumNode.ConsumeWebServiceNode(node);

                _currentNodes.Add(compendiumNode);
            }
        }

        /*#region INodeNavigator Members

        public void FocusNode(INode node)
        {
            throw new NotImplementedException();
        }

        public INode[] GetCurrentNodes()
        {
            _currentNodes.Clear();

            IAsyncResult result = _compendiumSharePointSvc.BeginGetAllNodes("MyMap", new AsyncCallback(GetNodesCallback), new object());
            result.AsyncWaitHandle.WaitOne();

            foreach (INode compendiumNode in _currentNodes)
            {
            }
        }

        #endregion*/
    }
}
