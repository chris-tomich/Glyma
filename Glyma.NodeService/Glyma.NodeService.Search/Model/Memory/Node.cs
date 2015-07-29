using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Glyma.NodeService.Search;
using Sql = Glyma.NodeService.Search.Model.Sql;
using System.Text;

namespace Glyma.NodeService.Search.Model.Memory
{
    public class Node
    {
        private Dictionary<string, string> _metadata;

        public Node(Sql.Node sqlNode)
        {
            Id = sqlNode.NodeUid;
            NodeType = sqlNode.NodeTypeUid.ToString();
        }

        public Guid Id
        {
            get;
            set;
        }

        public string NodeType
        {
            get;
            set;
        }

        public Dictionary<string, string> Metadata
        {
            get
            {
                if (_metadata == null)
                {
                    _metadata = new Dictionary<string, string>();
                }

                return _metadata;
            }
        }

        public void FillMetadata(IEnumerable<Sql.Metadata> allMetadata)
        {
            IEnumerable<Sql.Metadata> metadataForThisNode = allMetadata.Where(x => x.NodeUid == Id);

            foreach (Sql.Metadata metadatum in metadataForThisNode)
            {
                Metadata[metadatum.MetadataName] = metadatum.MetadataValue;
            }
        }

        public override string ToString()
        {
            StringBuilder nodeHtmlBuilder = new StringBuilder();

            nodeHtmlBuilder.AppendLine("<div>");

            foreach (KeyValuePair<string, string> metadatum in Metadata)
            {
                nodeHtmlBuilder.AppendLine(string.Format("<div><span>{0}:&nbsp;</span><span>{1}</span></div>", metadatum.Key, metadatum.Value));
            }

            nodeHtmlBuilder.AppendLine("</div>");

            return nodeHtmlBuilder.ToString();
        }
    }
}