using System;
using System.Collections.Generic;
using System.Windows.Media;
using VideoPlayer.Model.Node.State;

namespace VideoPlayer.Model.Node
{
    public class VideoNodeStateFactory
    {
        private readonly Dictionary<string, VideoNodeState> _states = new Dictionary<string, VideoNodeState>();

        public VideoNodeState GetOrCreate<T>(Func<T> state) where T : VideoNodeState
        {
            var typeName = typeof(T).FullName;

            if (_states.ContainsKey(typeName))
                return _states[typeName];

            _states.Add(typeName, state());

            return state();
        }

        public VideoNodeState SetState(MediaElementState mediaState, IVideoNode context)
        {
            if (_states.ContainsKey(mediaState + "State"))
                return _states[mediaState + "State"];

            switch (mediaState)
            {
                case MediaElementState.Buffering:
                    var bufferingState = new BufferingState(this, context);
                    _states.Add(bufferingState.GetType().Name, bufferingState);
                    return bufferingState;
                case MediaElementState.Playing:
                    var playingState = new PlayingState(this, context);
                    _states.Add(playingState.GetType().Name, playingState);
                    return playingState;
                case MediaElementState.Stopped:
                    var stoppedState = new StoppedState(this, context);
                    _states.Add(stoppedState.GetType().Name, stoppedState);
                    return stoppedState;
                case MediaElementState.Opening:
                    var openingState = new OpeningState(this, context);
                    _states.Add(openingState.GetType().Name, openingState);
                    return openingState;
                case MediaElementState.Paused:
                    var pausedState = new PausedState(this, context);
                    _states.Add(pausedState.GetType().Name, pausedState);
                    return pausedState;
                default:
                    return null;
            }
        }
    }
}
