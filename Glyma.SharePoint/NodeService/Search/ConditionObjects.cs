using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace NodeService
{
    public class ConditionHelper
    {
        public static Node GetAlternateNode(Descriptor descriptor)
        {
            Node result = null;
            if (descriptor.Relationship != null && descriptor.Relationship.Descriptors != null)
            {
                foreach (Descriptor desc in descriptor.Relationship.Descriptors)
                {
                    if (desc.DescriptorUid == descriptor.DescriptorUid)
                    {
                        continue;
                    }
                    else
                    {
                        result = desc.Node;
                    }
                }
            }
            return result;
        }

        public static Descriptor GetAlternateDescriptor(Descriptor descriptor)
        {
            Descriptor result = null;
            if (descriptor.Relationship != null && descriptor.Relationship.Descriptors != null)
            {
                foreach (Descriptor desc in descriptor.Relationship.Descriptors)
                {
                    if (desc.DescriptorUid == descriptor.DescriptorUid)
                    {
                        continue;
                    }
                    else
                    {
                        result = desc;
                    }
                }
            }
            return result;
        }

        public static bool IsParentNode(Descriptor descriptor)
        {
            bool result = false;
            if (descriptor.DescriptorType.DescriptorTypeName == "To")
            {
                result = true;
            }
            return result;
        }

        public static Node GetMapNode(Node initialNode)
        {
            Node result = null;
            if (initialNode.NodeType.NodeTypeName == "CompendiumMapNode")
            {
                result = initialNode;
            }
            else
            {
                foreach (Descriptor descriptor in initialNode.Descriptors)
                {
                    if (descriptor.Relationship.RelationshipType.RelationshipTypeName == "MapContainerRelationship")
                    {
                        result = GetAlternateNode(descriptor);
                        break;
                    }
                }
            }
            return result;
        }

        public static Guid GetMapId(Node node, Descriptor descriptor)
        {
            Node altNode = ConditionHelper.GetAlternateNode(descriptor);
            bool isNodeParent = ConditionHelper.IsParentNode(descriptor);

            Guid mapId = Guid.Empty;
            if (!isNodeParent)
            {
                mapId = altNode.NodeUid;
            }
            else
            {
                mapId = node.NodeUid;
            }
            return mapId;
        }
    }

    public enum Action
    {
        Stop,
        Continue
    }

    public enum ComparisonOperators
    {
        Equal,
        NotEqual
    }

    public enum ConditionContext
    {
        Node,
        Relationship,
        Descriptor,
        NodeType,
        RelationshipType,
        DescriptorType,
        MapMatch
    }

    public class ConditionResult
    {
        public bool IncludeNode
        {
            get;
            set;
        }

        public Action Action
        {
            get;
            set;
        }

        public bool Value
        {
            get;
            set;
        }
    }

    [XmlTypeAttribute(AnonymousType = true)]
    [XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class Condition : ISearchCondition
    {
        private Match[] matchField;
        private string actionField;

        /// <remarks/>
        [XmlElementAttribute("Match")]
        public Match[] Matches
        {
            get
            {
                return this.matchField;
            }
            set
            {
                this.matchField = value;
            }
        }

        [XmlAttribute]
        public string Action
        {
            get
            {
                return actionField;
            }
            set
            {
                actionField = value;
                try
                {
                    Action action = (Action)Enum.Parse(typeof(Action), value, true);
                    {
                        ActionValue = action;
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        [XmlIgnore]
        private Action ActionValue
        {
            get;
            set;
        }

        #region ISearchCondition Members

        public ConditionResult Evaluate(Node initialSearchNode, Node node, Relationship relationship, Descriptor descriptor)
        {
            ConditionResult result = new ConditionResult() { Action = ActionValue, IncludeNode = true, Value = true };
            foreach (Match match in Matches)
            {
                ConditionResult matchResult = match.Evaluate(initialSearchNode, node, relationship, descriptor);
                if (matchResult.IncludeNode == false)
                {
                    result.IncludeNode = false;
                }
                if (!matchResult.Value)
                {
                    result.Value = false;
                    break;
                }
            }
            return result;
        }

        #endregion
    }


    [XmlTypeAttribute(AnonymousType = true)]
    public partial class Match : ISearchCondition
    {
        private MetadataMatch[] metadataField;
        private TypeMatch[] typeField;
        private bool isIncludedField;

        /// <remarks/>
        [XmlElementAttribute("Metadata")]
        public MetadataMatch[] MetadataConditions
        {
            get
            {
                return this.metadataField;
            }
            set
            {
                this.metadataField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute("Type")]
        public TypeMatch[] TypeConditions
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        [XmlAttributeAttribute]
        public bool IsIncluded
        {
            get
            {
                return this.isIncludedField;
            }
            set
            {
                this.isIncludedField = value;
            }
        }

        #region ISearchCondition Members

        public ConditionResult Evaluate(Node initialSearchNode, Node node, Relationship relationship, Descriptor descriptor)
        {
            ConditionResult result = new ConditionResult() { IncludeNode = this.IsIncluded, Value = true };
            if (TypeConditions != null)
            {
                foreach (TypeMatch typeMatch in TypeConditions)
                {
                    bool isMatch = typeMatch.Evaluate(initialSearchNode, node, relationship, descriptor);
                    if (!isMatch)
                    {
                        //as soon as it doesn't evaluate to true we can break we have our result
                        //we are ANDing all the TypeConditions
                        result.Value = false;
                        break;
                    }
                }
            }
            if (result.Value) 
            {
                //if we still have a true result check the metadata next
                if (MetadataConditions != null)
                {
                    foreach (MetadataMatch metaMatch in MetadataConditions)
                    {
                        bool isMatch = metaMatch.Evaluate(initialSearchNode, node, relationship, descriptor);
                        if (!isMatch)
                        {
                            //as soon as it doesn't evaluate to true we can break we have our result
                            //we are ANDing all the TypeConditions
                            result.Value = false;
                            break;
                        }
                    }
                }
            }
            return result;
        }

        #endregion
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true)]
    public partial class MetadataMatch
    {
        private string nameField;
        private string metadataTypeField;
        private Guid typeIdField;
        private string valueField;

        /// <remarks/>
        [XmlAttributeAttribute]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [XmlAttributeAttribute]
        public string MetadataType
        {
            get
            {
                return this.metadataTypeField;
            }
            set
            {
                this.metadataTypeField = value;
            }
        }

        /// <remarks/>
        [XmlAttributeAttribute]
        public Guid TypeId
        {
            get
            {
                return this.typeIdField;
            }
            set
            {
                this.typeIdField = value;
            }
        }

        /// <remarks/>
        [XmlTextAttribute]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }

        #region ISearchCondition Members

        public bool Evaluate(Node initialSearchNode, Node node, Relationship relationship, Descriptor descriptor)
        {
            bool result = false;
            var metadata = from dbDatum in node.Metadatas select dbDatum;

            foreach (var datum in metadata.Where(y => y.MetadataName == Name).OrderBy(x => (x.RelationshipUid != null) ? x.RelationshipUid : Guid.Empty).OrderBy(x => (x.DescriptorTypeUid != null) ? x.DescriptorTypeUid : Guid.Empty))
            {
                if (datum.MetadataTypeUid == TypeId)
                {
                    if (datum.MetadataValue == Value)
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        #endregion
    }

    public partial class MapEqualityMatch
    {
        public bool Evaluate(Node initialSearchNode, Node node, Relationship relationship, Descriptor descriptor)
        {
            Guid searchId = Guid.Empty;
            bool? evaluation = new bool?();

            if (relationship.RelationshipType.RelationshipTypeName == "MapContainerRelationship")
            {
                Node mapNode = ConditionHelper.GetMapNode(initialSearchNode);

                Node altNode = ConditionHelper.GetAlternateNode(descriptor);
                bool isNodeParent = ConditionHelper.IsParentNode(descriptor);

                Guid mapId = Guid.Empty;
                if (!isNodeParent && altNode.NodeType.NodeTypeName != "DomainNode")
                {
                    mapId = altNode.NodeUid;
                }
                else
                {
                    mapId = node.NodeUid;
                }

                if (mapNode.NodeUid == mapId)
                {
                    evaluation = true;
                }
                else
                {
                    evaluation = false;
                }
            }
            else
            {
                evaluation = false;
            }

            

            return evaluation.Value;
        }
    }


    [XmlTypeAttribute(AnonymousType = true)]
    public partial class TypeMatch
    {
        private string baseTypeField;
        private string idField;
        private string nameField;
        private string operatorField;

        /// <remarks/>
        [XmlAttributeAttribute]
        public string BaseType
        {
            get
            {
                return this.baseTypeField;
            }
            set
            {
                this.baseTypeField = value;
                try
                {
                    ConditionContext context = (ConditionContext)Enum.Parse(typeof(ConditionContext), value, true);
                    Context = context;
                }
                catch (Exception)
                {
                }
            }
        }

        [XmlAttribute]
        private string Operator
        {
            get
            {
                return operatorField;
            }
            set
            {
                operatorField = value;
                try
                {
                    ComparisonOperators op = (ComparisonOperators)Enum.Parse(typeof(ComparisonOperators), value, true);
                    OperatorValue = op;
                }
                catch (Exception)
                {
                    OperatorValue = ComparisonOperators.Equal;
                }
            }
        }

        /// <remarks/>
        [XmlAttributeAttribute]
        public string Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
                Guid id;
                try
                {
                    id = new Guid(value);
                    SearchValue = id;
                }
                catch (Exception)
                {
                    SearchValue = Guid.Empty;
                }
            }
        }

        /// <remarks/>
        [XmlAttributeAttribute]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        [XmlIgnore]
        public ConditionContext Context
        {
            get;
            set;
        }

        [XmlIgnore]
        public Guid SearchValue
        {
            get;
            set;
        }

        [XmlIgnore]
        public ComparisonOperators OperatorValue
        {
            get;
            set;
        }

        public bool Evaluate(Node initialSearchNode, Node node, Relationship relationship, Descriptor descriptor)
        {
            Guid searchId = Guid.Empty;
            bool? evaluation = new bool?();
            
            switch (Context)
            {
                case ConditionContext.Node:
                    searchId = node.NodeUid;
                    break;
                case ConditionContext.Relationship:
                    searchId = relationship.RelationshipUid;
                    break;
                case ConditionContext.Descriptor:
                    searchId = descriptor.DescriptorUid;
                    break;
                case ConditionContext.NodeType:
                    searchId = node.NodeTypeUid.Value;
                    break;
                case ConditionContext.RelationshipType:
                    searchId = relationship.RelationshipTypeUid.Value;
                    break;
                case ConditionContext.DescriptorType:
                    searchId = descriptor.DescriptorTypeUid.Value;
                    break;
                case ConditionContext.MapMatch:
                    searchId = ConditionHelper.GetMapNode(initialSearchNode).NodeUid;
                    SearchValue = ConditionHelper.GetMapId(node, descriptor);
                    break;
                default:
                    break;
            }

            switch (OperatorValue)
            {
                case ComparisonOperators.Equal:
                    evaluation = SearchValue == searchId;
                    break;
                case ComparisonOperators.NotEqual:
                    evaluation = SearchValue != searchId;
                    break;
                default:
                    evaluation = false;
                    break;
            }

            return evaluation.Value;
        }
    }

}