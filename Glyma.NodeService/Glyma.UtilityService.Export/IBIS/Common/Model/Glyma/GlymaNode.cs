using System;
using System.Collections.Generic;
using Glyma.UtilityService.Export.Common.Model;
using Glyma.UtilityService.Export.IBIS.Common.Model.Interface;
using TransactionalNodeService.Proxy.Universal;
using TransactionalNodeService.Proxy.Universal.Types;

namespace Glyma.UtilityService.Export.IBIS.Common.Model.Glyma
{
    public class GlymaNode : IGlymaNode
    {
        private string _name;
        private List<IGlymaNode> _nodes;
        private NodeType _nodeType;
        private Guid? _id;

        private List<IGlymaNode> _childNodes;
        public IRelationship Relationship { get; protected set; }

        public INode Proxy { get; protected set; }

        public Guid Id
        {
            get
            {
                if (!_id.HasValue)
                {
                    _id = Proxy.Id;
                }
                return _id.Value;
            }

            set { _id = value; }
        }

        public string OrginalId
        {
            get { return Proxy.OriginalId; }
        }

        public NodeType NodeType
        {
            get
            {
                if (_nodeType == null)
                {
                    _nodeType = Proxy.NodeType;
                }
                return _nodeType;
            }
            set { _nodeType = value; }
        }


        public string Name {
            get
            {
                if (_name == null)
                {
                    _name = FindMetadata("Name");
                    if (string.IsNullOrEmpty(_name))
                    {
                        _name = "(No Name)";
                    }
                }
                return _name;
            }
            protected set { _name = value; } 
        }

        public double XPosition { get; protected set; }
        public double YPosition { get; protected set; }

        public int Depth { get; set; }
        public NodeVideo NodeVideo { get; private set; }
        public NodeDescription NodeDescription { get; private set; }

        public List<IGlymaNode> Nodes
        {
            get
            {
                if (_nodes == null)
                {
                    _nodes = new List<IGlymaNode>();
                }
                return _nodes;
            }
        }

        public List<IGlymaNode> ChildNodes
        {
            get
            {
                if (_childNodes == null)
                {
                    _childNodes = new List<IGlymaNode>();
                }
                return _childNodes;
            }
        }

        

        public IEnumerable<IGlymaNode> GetAllChildNodes()
        {
            var output = new List<IGlymaNode>();
            foreach (var node in ChildNodes)
            {
                if (node.ChildNodes.Count > 0)
                    output.AddRange(node.GetAllChildNodes());
                else
                    output.Add(node);
            }
            return output;
        }

        public GlymaNode()
        {
            
        }

        protected GlymaNode(INode node)
        {
            Proxy = node;
        }

        public GlymaNode(IRelationship relationship, INode node)
        {
            Relationship = relationship;
            Proxy = node;

            double x, y;
            if (!double.TryParse(FindRelationshipBindedMetadata("XPosition"), out x))
            {
                x = 0;
            }
            XPosition = x;
            if (!double.TryParse(FindRelationshipBindedMetadata("YPosition"), out y))
            {
                y = 0;
            }
            YPosition = y;

            NodeVideo = new NodeVideo(FindMetadata("Video.Source"), FindMetadata("Video.StartPosition"), FindMetadata("Video.EndPosition"));

            if (FindMetadata("DescriptionType") != string.Empty)
            {
                NodeDescription = new NodeDescription(FindMetadata("DescriptionType"), FindMetadata("Description"));
            }
            else if (FindMetadata("Description.Type") != string.Empty)
            {
                NodeDescription = new NodeDescription(FindMetadata("Description.Type"), FindMetadata("Description.Content"), FindMetadata("Description.Url"));
            }
            else
            {
                NodeDescription = new NodeDescription(null, null);
            }
            
            
        }

        protected string FindMetadata(string key)
        {
            var metadataSet = Proxy.Metadata.FindMetadata(key);
            if (metadataSet != null)
            {
                return metadataSet.Value;
            }
            return string.Empty;
        }

        protected string FindRelationshipBindedMetadata(string key)
        {
            var metadataSet = Proxy.Metadata.FindMetadata(Relationship, key);
            if (metadataSet != null)
            {
                return metadataSet.Value;
            }
            return string.Empty;
        }
    }
}
