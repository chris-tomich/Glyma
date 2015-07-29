using System;
using System.Collections.Generic;
using System.Windows;
using SilverlightMappingToolBasic.UI.SuperGraph.ViewModel;
using SilverlightMappingToolBasic.UI.ViewModel;

namespace SilverlightMappingToolBasic.UI.SuperGraph.Controller
{
    public interface ISuperGraphRelationshipFactory
    {
        void ConnectNodes(Node from, Node to);
        void TranscludeNode(List<Node> nodes, INode map, Point newLocation);
        void DeleteTranscludedNode(List<Node> nodes, INode map, bool isQuiet);
        void ConnectTranscludedNodes(Node from, Node to);
        void DeleteRelationship(Relationship relationship);
        void DeleteRelationships(IEnumerable<Relationship> relationships);
        void TranscludeNodes(List<Node> nodes, INode map, Point location, Point? oldLocation = null);
        void ConnectMultipleTranscludedNodes(List<Relationship> relationships, List<Node> fullNodes, INode map);
        void DeleteNodes(List<Node> nodes, INode map, bool isSamePage);

        Guid DomainId { get; }

        bool IsSameDomain { get; set; }
    }
}
