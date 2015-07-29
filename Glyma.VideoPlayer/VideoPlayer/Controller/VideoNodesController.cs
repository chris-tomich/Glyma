using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using VideoPlayer.Model.Node;
using VideoPlayerSharedLib;

namespace VideoPlayer.Controller
{
    internal class VideoNodesController
    {
        private readonly List<VideoNode> _nodes;

        public VideoNode CurrentNode { get; private set; }

        //Add new node to controller
        public void AddNode(VideoNode node)
        {
            _nodes.Add(node);
            SetCurrentNode(node);
        }

        //Construction
        public VideoNodesController()
        {
            _nodes = new List<VideoNode>();
            var defaultNode = new VideoNode(new Command
            {
                Name = "Play",
                Params = new List<Param>()
                {
                    {new Param() { Name="NodeId", Value = "00000000-0000-0000-0000-000000000000"}},
                    {new Param() { Name="Source", Value=""}},
                    {new Param() { Name="StartTimeCode", Value="00:00:00"}},
                    {new Param() { Name="AutoPlay", Value="false"}},
                }
            });
            AddNode(defaultNode);
        }

        public void ResetToDefaultNode()
        {
            var defaultNode = _nodes.FirstOrDefault();
            if (defaultNode != null && CurrentNode != defaultNode)
            {
                OnNodeChanged(new NodeChangedEventArgs(CurrentNode.NodeId, defaultNode.NodeId));
                CurrentNode = defaultNode;
            }
        }

        //Set Current Node
        public void SetCurrentNode(VideoNode node)
        {
            if (CurrentNode != node && node != null && CurrentNode != null)
            {
                OnNodeChanged(new NodeChangedEventArgs(CurrentNode.NodeId, node.NodeId));
            }
            CurrentNode = node;
        }

        //Retreive Node by NodeId
        public VideoNode GetVideoNodeById(Guid id)
        {
            return _nodes.FirstOrDefault(q => q.NodeId == id);
        }


        public delegate void NodeChangedEventHandler(Object sender, NodeChangedEventArgs e);
        public event NodeChangedEventHandler NodeChanged;
        protected virtual void OnNodeChanged(NodeChangedEventArgs e)
        {
            var handler = NodeChanged;
            if (handler != null)NodeChanged(this, e); 
        }
    }

    public class NodeChangedEventArgs : EventArgs
    {
        public readonly Guid Old;
        public readonly Guid New;
        public NodeChangedEventArgs(Guid old, Guid @new)
        {
            New = @new;
            Old = old;
        }
    }
}
