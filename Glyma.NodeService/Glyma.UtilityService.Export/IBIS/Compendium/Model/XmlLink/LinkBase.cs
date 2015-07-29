using System.Xml;
using Glyma.UtilityService.Export.IBIS.Common.Model.Interface;
using Glyma.UtilityService.Export.IBIS.Common.Model.Xml;
using TransactionalNodeService.Proxy.Universal.Collections.MetadataCollection;

namespace Glyma.UtilityService.Export.IBIS.Compendium.Model.XmlLink
{
    public class LinkBase : XmlElementBase
    {
        public LinkType LinkType
        {
            get;
            set;
        }

        public LinkBase(XmlDocument doc, IGlymaRelationship relationship, XmlElement parent) : base(doc, parent)
        {
            if (this is ReferenceLink)
            {
                LinkType = LinkType.About; //reference links are About, note this is different to a CompendiumReferenceNode
            }
            else
            {
                LinkType = GetLinkType(relationship);
            }
            int linkTypeNum = (int)LinkType;
            AddAttributeByKeyValue("type", linkTypeNum.ToString());
            AddAttributeByKeyValue("label", "");
            AddAttributeByKeyValue("originalid", "");
        }

        private LinkType GetLinkType(IGlymaRelationship relationship)
        {
            LinkType linkType = LinkType.RespondsTo;
            if (relationship != null && relationship.NodeFrom != null && relationship.NodeFrom.NodeType != null)
            {
                switch (relationship.NodeFrom.NodeType.Name)
                {
                    case "CompendiumDecisionNode":
                        linkType = LinkType.Resolves;
                        break;
                    case "CompendiumReferenceNode":
                        // not currently part of our schema of node types, but this is the correct LinkType
                        linkType = LinkType.Specializes;
                        break;
                    case "CompendiumNoteNode":
                        linkType = LinkType.ExpandsOn;
                        break;
                    case "CompendiumConNode":
                        linkType = LinkType.ObjectsTo;
                        break;
                    case "CompendiumProNode":
                        linkType = LinkType.Supports;
                        break;
                    case "CompendiumArgumentNode":
                        //not currently part of our schema of node types, but this is the correct LinkType
                        linkType = LinkType.Challenges;
                        break;
                    case "CompendiumIdeaNode":
                        linkType = LinkType.RespondsTo;
                        break;
                    case "CompendiumMapNode":
                        linkType = LinkType.RespondsTo;
                        break;
                    case "CompendiumQuestionNode":
                        linkType = LinkType.RespondsTo;
                        break;
                    case "CompendiumListNode":
                        // not currently part of our schema of node types, but this is the correct LinkType
                        linkType = LinkType.RespondsTo;
                        break;
                    default:
                        linkType = LinkType.RespondsTo;
                        break;
                }
            }
            return linkType;
        }
    }
}
