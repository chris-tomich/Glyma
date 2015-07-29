using System;
using System.Collections.Generic;
using SilverlightMappingToolBasic.UI.SuperGraph.Controller.Operations;
using System.Windows;
using SilverlightMappingToolBasic.UI.SuperGraph.ViewModel;
using TransactionalNodeService.Proxy;
using Node = SilverlightMappingToolBasic.UI.SuperGraph.ViewModel.Node;
using Relationship = SilverlightMappingToolBasic.UI.SuperGraph.ViewModel.Relationship;

namespace SilverlightMappingToolBasic.UI.SuperGraph.Controller
{
    public interface ISuperGraphNodeFactory
    {
        void AddCon(Point location, Dictionary<string, string> metaData = null);
        void AddIdea(Point location, Dictionary<string, string> metaData = null);
        void AddMap(Point location, Dictionary<string, string> metaData = null);
        void AddPro(Point location, Dictionary<string, string> metaData = null);
        void AddQuestion(Point location, Dictionary<string, string> metaData = null);
        void AddNote(Point location, Dictionary<string,string> metaData = null);
        void AddDecision(Point location, Dictionary<string, string> metaData = null);
        void DeleteNode(Node node);
        void UpdateNode(Node viewModelNode, ChangeNodeTypeEnum changedTo);

        
        void CloneNode(Node node);
        void CloneNodes(IEnumerable<Node> nodes, IEnumerable<Relationship> relationships);
        void AddLinkedNode(string nodeType, Point location, Node parent);
    }
}
