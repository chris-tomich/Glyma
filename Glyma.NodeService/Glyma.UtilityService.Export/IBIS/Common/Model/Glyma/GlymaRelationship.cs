using System;
using System.Collections.Generic;
using System.Linq;
using Glyma.UtilityService.Export.IBIS.Common.Model.Interface;
using TransactionalNodeService.Proxy.Universal;
using TransactionalNodeService.Proxy.Universal.Collections.NodeContainers;

namespace Glyma.UtilityService.Export.IBIS.Common.Model.Glyma
{
    public class GlymaRelationship : IGlymaRelationship
    {
        private readonly IRelationship _relationship;
        private IEnumerable<NodeTuple> _connectedNodes;
        private INode _nodeFrom;
        private INode _nodeTo;
 
        public IRelationship ProxyRelationship {
            get { return _relationship; }
        }

        public INode NodeFrom
        {
            get
            {
                if (_nodeFrom == null)
                {
                    var nodeTuple = ConnectedNodes.FirstOrDefault(q => q.ConnectionType.Name == "From");
                    _nodeFrom = nodeTuple.Node;
                }
                return _nodeFrom;
            }
        }

        public INode NodeTo
        {
            get
            {
                if (_nodeTo == null)
                {
                    var nodeTuple = ConnectedNodes.FirstOrDefault(q => q.ConnectionType.Name == "To");
                    _nodeTo = nodeTuple.Node;
                }
                return _nodeTo;
            }
        }

        public IEnumerable<NodeTuple> ConnectedNodes
        {
            get
            {
                if (_connectedNodes == null)
                {
                    _connectedNodes = ProxyRelationship.Nodes.FindNodes();
                }
                return _connectedNodes;
            }
        }

        public GlymaRelationship(IRelationship relationship)
        {
            _relationship = relationship;
            

            ///TODO: Need to get the actual value from the server
            Created = "1380778119656";
            LastModified = "1380778119656";
        }

        public Guid Id
        {
            get { return ProxyRelationship.Id; }
        }

        public string OrginalId
        {
            get { return ProxyRelationship.OriginalId; }
        }

        public Guid From
        {
            get { return NodeFrom.Id; }
        }

        public Guid To
        {
            get { return NodeTo.Id; }
        }

        public string Created { get; private set; }

        public string LastModified { get; private set; }
    }
}
