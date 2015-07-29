using System;
using System.Windows.Controls;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Arrow;
using System.Collections.Generic;
using System.Linq;
using SilverlightMappingToolBasic.UI.SuperGraph.View.CollapseSupportClasses;
using SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses;

namespace SilverlightMappingToolBasic.UI.SuperGraph.Controller
{
    public class ArrowControlFactory
    {
        private NodeControlFactory _nodeFactory = null;
        private Dictionary<Guid, ArrowControlFactorySet> _arrows = null;
        private Dictionary<Guid, ViewModel.Relationship> _pendingRelationships = null;

        public ArrowControlFactory(NodeControlFactory nodeFactory)
        {
            _nodeFactory = nodeFactory;
        }

        private NodeControlFactory NodeFactory
        {
            get
            {
                return _nodeFactory;
            }
        }

        private Dictionary<Guid, ArrowControlFactorySet> Arrows
        {
            get
            {
                if (_arrows == null)
                {
                    _arrows = new Dictionary<Guid, ArrowControlFactorySet>();
                }

                return _arrows;
            }
        }

        private Dictionary<Guid, ViewModel.Relationship> PendingRelationships
        {
            get
            {
                if (_pendingRelationships == null)
                {
                    _pendingRelationships = new Dictionary<Guid, ViewModel.Relationship>();
                }

                return _pendingRelationships;
            }
        }

        private ArrowControlFactorySet CreateSet(ViewModel.Relationship viewModelRelationship, NodeControl fromControl, NodeControl toControl)
        {
            ArrowController arrowController = new ArrowController(viewModelRelationship, fromControl, toControl);
            
            arrowController.ViewModel.Id = viewModelRelationship.Id;
            if (toControl != null)
            {
                toControl.LinkFromNode(fromControl);
            }
            ArrowControl arrowControl = new ArrowControl(fromControl, toControl);
            Canvas.SetZIndex(arrowControl, 40);
            arrowControl.DataContext = arrowController.ViewModel;
            if (toControl != null && toControl.ViewModelNode.State == CollapseState.None)
            {
                toControl.ViewModelNode.State = CollapseState.Expanded;
            }
            ArrowControlFactorySet set = new ArrowControlFactorySet();
            set.Relationship = viewModelRelationship;
            set.Control = arrowControl;
            set.Controller = arrowController;

            return set;
        }

        public bool IsArrowExist(Guid from, Guid to)
        {
            foreach (var set  in Arrows)
            {
                var arrow = set.Value;
                if (arrow.Relationship.From == from && arrow.Relationship.To == to)
                {
                    return true;
                }
            }
            return false;
        }

        public ArrowControl CreateArrow(ViewModel.Relationship viewModelRelationship)
        {
            if (viewModelRelationship.From == Guid.Empty || viewModelRelationship.To == Guid.Empty)
            {
                return null;
            }

            

            if (Arrows.ContainsKey(viewModelRelationship.Id))
            {
                return null;
            }
            if (IsArrowExist(viewModelRelationship.From, viewModelRelationship.To))
            {
                return null;
            }
            var fromControl = NodeFactory.FindNodeControl(viewModelRelationship.From);
            var toControl = NodeFactory.FindNodeControl(viewModelRelationship.To);

            if (fromControl == null || toControl == null)
            {
                PendingRelationships[viewModelRelationship.Id] = viewModelRelationship;
                return null;
            }

                

            var set = CreateSet(viewModelRelationship, fromControl, toControl);

            Arrows[viewModelRelationship.Id] = set;
            return set.Control;
        }

        public IEnumerable<ArrowControl> ReassessPendingRelationships()
        {
            List<ViewModel.Relationship> completedViewModels = new List<ViewModel.Relationship>();

            foreach (ViewModel.Relationship pendingRelationship in PendingRelationships.Values)
            {
                NodeControl fromControl = NodeFactory.FindNodeControl(pendingRelationship.From);
                NodeControl toControl = NodeFactory.FindNodeControl(pendingRelationship.To);

                if (fromControl != null && toControl != null)
                {
                    ArrowControlFactorySet set = CreateSet(pendingRelationship, fromControl, toControl);

                    Arrows[pendingRelationship.Id] = set;

                    completedViewModels.Add(pendingRelationship);

                    yield return set.Control;
                }
            }

            foreach (ViewModel.Relationship completedViewModel in completedViewModels)
            {
                PendingRelationships.Remove(completedViewModel.Id);
            }
        }

        public ArrowControlFactorySet RemoveArrow(ArrowControl arrowControl)
        {
            ViewModel.ArrowViewModel viewModelArrow = arrowControl.DataContext as ViewModel.ArrowViewModel;

            if (viewModelArrow != null)
            {
                return RemoveArrow(viewModelArrow);
            }
            else
            {
                IEnumerable<Guid> arrowSetIds = from arrowSet in Arrows
                                                where arrowSet.Value.Control == arrowControl
                                                select arrowSet.Key;

                if (arrowSetIds.Any())
                {
                    return RemoveArrow(arrowSetIds.First());
                }
            }

            return null;
        }

        public bool IsRelationshipExist(ViewModel.Relationship viewModelRelationship)
        {
            var exist = Arrows.Values.FirstOrDefault(
                    q =>
                        q.Relationship.From == viewModelRelationship.From &&
                        q.Relationship.To == viewModelRelationship.To);
            return exist != null;
        }

        public ArrowControlFactorySet RemoveArrow(ViewModel.ArrowViewModel viewModelArrow)
        {
            return RemoveArrow(viewModelArrow.Id);
        }

        public ArrowControlFactorySet RemoveArrow(ViewModel.Relationship viewModelRelationship)
        {
            return RemoveArrow(viewModelRelationship.Id);
        }

        public ArrowControlFactorySet RemoveArrow(Guid relationshipId)
        {
            ArrowControlFactorySet set = null;

            if (Arrows.ContainsKey(relationshipId))
            {
                set = Arrows[relationshipId];
                Arrows.Remove(relationshipId);
                set.Control.UnlinkRelatedNodeControls();
            }

            return set;
        }

        public IEnumerable<ArrowControlFactorySet> RemoveArrows(ViewModel.Node node)
        {
            List<ArrowControlFactorySet> irrelevantSets = new List<ArrowControlFactorySet>();

            foreach (ArrowControlFactorySet set in Arrows.Values)
            {
                if (set.Relationship.From == node.Id || set.Relationship.To == node.Id)
                {
                    irrelevantSets.Add(set);
                }
            }

            foreach (ArrowControlFactorySet irreleventSet in irrelevantSets)
            {
                Arrows.Remove(irreleventSet.Relationship.Id);
                irreleventSet.Control.UnlinkRelatedNodeControls();
            }

            return irrelevantSets;
        }

        public IEnumerable<ArrowControlFactorySet> Clear()
        {
            foreach (ArrowControlFactorySet set in Arrows.Values)
            {
                yield return set;
            }

            Arrows.Clear();
        }
    }
}
