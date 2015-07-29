using System;
using System.Globalization;
using Glyma.UtilityService.Export.IBIS.Common.Model.Glyma;
using TransactionalNodeService.Proxy.Universal;

namespace Glyma.UtilityService.Export.IBIS.Compendium.Model
{
    public class CompendiumNode : GlymaNode
    {
        private readonly CompendiumNode _referenceNode;

        public CompendiumNode(IRelationship relationship, INode node) : base(relationship, node)
        {
            Source = FindMetadata("Link");

            Description = FindMetadata("Description");
            ExtraMetadata = FindExtraMetadata();

            

            if (!string.IsNullOrEmpty(Source))
            {
                _referenceNode = new CompendiumNode(this, relationship);
            }

            Initialise(node);
        }

        private CompendiumNode(CompendiumNode node, IRelationship orginRelationship)
        {
            Relationship = orginRelationship;
            Id = Guid.NewGuid();
            Proxy = node.Proxy;
            Name = node.Source;
            NodeType = Proxy.MapManager.NodeTypes["CompendiumReferenceNode"];

            XPosition = (XPosition + 140);
            YPosition = (YPosition + 140);

            Source = FindMetadata("Link");

            Description = string.Empty;
            ExtraMetadata = string.Empty;

            Initialise(node.Proxy);
        }

        private readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private long ConvertToTimestamp(DateTime value)
        {
            TimeSpan elapsedTime = value - _epoch;
            return (long) elapsedTime.TotalSeconds;
        }

        private void Initialise(INode node)
        {
            ///TODO: Need to get the actual value from the server
            long time = ConvertToTimestamp(DateTime.Now);
            
            Author = "Glyma";
            Created = time.ToString(CultureInfo.InvariantCulture);
            LastModified = time.ToString(CultureInfo.InvariantCulture);
            LastModificationAuthor = "Glyma";
        }

        public CompendiumNode ReferenceNode
        {
            get { return _referenceNode; }
        }

        public string Author { get; private set; }

        public string Created { get; private set; }
        public string LastModified { get; private set; }
        public string LastModificationAuthor { get; private set; }

        public string Source { get; private set; }

        public string Description { get; private set; }

        public string ExtraMetadata { get; private set; }

        private string FindExtraMetadata()
        {
            var extraMetadata = string.Empty;
            foreach (var metadata in Proxy.Metadata)
            {
                if (metadata.Name != "Link" && metadata.Name != "Description.Content"
                    && metadata.Name != "Description.Type" && metadata.Name != "CollapseState"
                    && metadata.Name != "YPosition" && metadata.Name != "Name"
                    && metadata.Name != "Visibility" && metadata.Name != "XPosition"
                    && metadata.Name != "AuthorCollapseState" && metadata.Name != "AuthorVisibility")
                {
                    extraMetadata += string.Format("'{0}'      '{1}'\r\n", metadata.Name, metadata.Value);
                }
            }
            return extraMetadata;
        }

        
    }
}
