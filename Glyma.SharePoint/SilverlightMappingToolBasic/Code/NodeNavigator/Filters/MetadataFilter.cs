using System;

using SilverlightMappingToolBasic.MappingService;

namespace SilverlightMappingToolBasic
{
    public enum MetadataFilterOperator
    {
        GREATER_THAN,
        GREATER_THAN_OR_EQUAL,
        LESS_THAN,
        LESS_THAN_OR_EQUAL,
        EQUAL,
        NOT_EQUAL,
        CONTAINS
    }

    public abstract class MetadataFilter
    {
        public MetadataContext Key 
        {
            get;
            set;
        }

        public MetadataFilterOperator Operator
        {
            get;
            set;
        }

        public string Value
        {
            get;
            set;
        }

        public abstract bool HasMatch(INodeProxy node);

        protected bool ContainsKey(INodeProxy node)
        {
            if (node.HasMetadata(Key) && node.GetNodeMetadata(Key) != null)
            {
                return true;
            }
            return false;
        }
    }
}
