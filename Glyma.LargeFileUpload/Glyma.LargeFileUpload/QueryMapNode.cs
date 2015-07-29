using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glyma.HttpHandlers
{
    public partial class QueryMapNode
    {
        private Guid _defaultViewRelationshipUid = Guid.Empty;
        private Dictionary<string, Dictionary<Guid, QueryMapMetadata>> _metadata = null;

        public QueryMapNode(QueryMapMultiDepthResult result)
        {
            RootMapUid = result.RootMapUid.Value;
            NodeUid = result.NodeUid.Value;
            DomainUid = result.DomainUid.Value;
            NodeTypeUid = result.NodeTypeUid;
        }

        private Dictionary<string, Dictionary<Guid, QueryMapMetadata>> Metadata
        {
            get
            {
                if (_metadata == null)
                {
                    _metadata = new Dictionary<string, Dictionary<Guid, QueryMapMetadata>>();
                }

                return _metadata;
            }
        }

        public string NodeType
        {
            get
            {
                /// The code developed by Kashif is case sensitive.
                switch (NodeTypeUid.ToString().ToUpperInvariant())
                {
                    case "3B53600F-39EC-42FB-B08A-325062037130":
                        return "Idea";//"CompendiumIdeaNode";
                    case "042E7E3B-8A5F-4A52-B1DD-3361A3ACD72A":
                        return "Idea";//"CompendiumArgumentNode";
                    case "263754C2-2F31-4D21-B9C4-6509E00A5E94":
                        return "Note";//"DomainNode";
                    case "B8C354CB-C7D0-4982-9A0F-6C4368FAB749":
                        return "Map";//"CompendiumMapNode";
                    case "47B75628-7FDF-4440-BF35-8506D3FE6F2A":
                        return "Note";//"CompendiumGenericNode";
                    case "53EC78E3-F189-4340-B251-AAF9D78CF56D":
                        return "Decision";//"CompendiumDecisionNode";
                    case "DA66B603-F6B3-4ECF-8ED0-AB34A288CF08":
                        return "Con";//"CompendiumConNode";
                    case "7D3C9B87-F31D-400F-A375-ABC0D1888625":
                        return "Note";//"CompendiumListNode";
                    case "8F3DA942-06C4-4075-AD8B-B51361ABC900":
                        return "Idea";//"CompendiumReferenceNode";
                    case "99FD1475-8099-45D3-BEDF-BEC396CCB4DD":
                        return "Question";//"CompendiumQuestionNode";
                    case "84B7634B-DB8D-449B-B8CE-D3F3F80E95DD":
                        return "Note";//"CompendiumNoteNode";
                    case "084F38B7-115F-4AF6-9E30-D9D91226F86B":
                        return "Pro";//"CompendiumProNode";
                    default:
                        return "Idea";
                }
            }
        }

        public void SetMetadataView(Guid relationshipUid)
        {
            _defaultViewRelationshipUid = relationshipUid;
        }

        public void AddMetadata(QueryMapMetadata metadata)
        {
            Dictionary<Guid, QueryMapMetadata> metadataByRelationshipId;

            if (Metadata.ContainsKey(metadata.MetadataName))
            {
                metadataByRelationshipId = Metadata[metadata.MetadataName];
            }
            else
            {
                metadataByRelationshipId = new Dictionary<Guid, QueryMapMetadata>();
                Metadata[metadata.MetadataName] = metadataByRelationshipId;
            }

            if (metadata.RelationshipUid.HasValue)
            {
                metadataByRelationshipId[metadata.RelationshipUid.Value] = metadata;
            }
            else
            {
                metadataByRelationshipId[Guid.Empty] = metadata;
            }
        }

        public IEnumerable<QueryMapMetadata> FindAllMetadata()
        {
            foreach (Dictionary<Guid, QueryMapMetadata> metadataByView in Metadata.Values)
            {
                if (metadataByView.ContainsKey(Guid.Empty))
                {
                    yield return metadataByView[Guid.Empty];
                }

                if (metadataByView.ContainsKey(_defaultViewRelationshipUid))
                {
                    yield return metadataByView[_defaultViewRelationshipUid];
                }
            }
        }

        public IEnumerable<QueryMapMetadata> FindAllMetadata(string name)
        {
            if (Metadata.ContainsKey(name))
            {
                foreach (QueryMapMetadata metadata in Metadata[name].Values)
                {
                    yield return metadata;
                }
            }
        }

        public QueryMapMetadata FindSingleMetadata(string name)
        {
            if (Metadata.ContainsKey(name))
            {
                Dictionary<Guid, QueryMapMetadata> matchingMetadata = Metadata[name];

                return matchingMetadata.FirstOrDefault().Value;
            }

            return null;
        }

        public QueryMapMetadata FindSingleMetadataDefaultView(string name)
        {
            if (_defaultViewRelationshipUid == Guid.Empty)
            {
                return FindSingleMetadata(name);
            }
            else
            {
                return FindSingleMetadata(_defaultViewRelationshipUid, name);
            }
        }

        public QueryMapMetadata FindSingleMetadata(Guid relationshipUid, string name)
        {
            if (Metadata.ContainsKey(name))
            {
                Dictionary<Guid, QueryMapMetadata> matchingMetadata = Metadata[name];

                if (matchingMetadata.ContainsKey(relationshipUid))
                {
                    return matchingMetadata[relationshipUid];
                }
            }

            return null;
        }
    }
}
