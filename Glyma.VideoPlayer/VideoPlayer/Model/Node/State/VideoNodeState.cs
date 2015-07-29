using System;
using System.Windows.Media;
using VideoPlayer.Model.Node.State.Interface;

namespace VideoPlayer.Model.Node.State
{
    public abstract class VideoNodeState : IVideoNodeState
    {
        protected VideoNodeStateFactory Factory;
        protected IVideoNode Context;

        protected VideoNodeState(VideoNodeStateFactory factory, IVideoNode context)
        {
            Factory = factory;
            Context = context;
        }

        public void SetState(MediaElementState mediaState)
        {
            Context.CurrentState = Factory.SetState(mediaState, Context);
        }

        

        //protected void TransitTo<T>(Func<T> newState) where T : VideoNodeState
        //{
        //    Context.CurrentState = Factory.GetOrCreate(newState);
        //}


        public virtual MediaElementState GetState()
        {
            return new MediaElementState();
        }
    }

    
}
