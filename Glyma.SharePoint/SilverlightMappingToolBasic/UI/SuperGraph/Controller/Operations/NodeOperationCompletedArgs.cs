using System;
using System.Collections.Generic;
using SilverlightMappingToolBasic.UI.SuperGraph.ViewModel;
using TransactionalNodeService.Soap.TransactionFramework;

namespace SilverlightMappingToolBasic.UI.SuperGraph.Controller.Operations
{
    public class NodeOperationCompletedArgs : EventArgs
    {
        public InProcessTransactionResponse Response
        {
            get; 
            set;
        }

        public Node ViewModeNode
        {
            get;
            set;
        }

        public Relationship ViewModeRelationship
        {
            get;
            set;
        }

        public List<KeyValuePair<Node, ModelOperationType>> Nodes
        {
            get;
            set;
        }

        public List<KeyValuePair<Relationship, ModelOperationType>> Relationships
        {
            get;
            set;
        }
    } 
}
