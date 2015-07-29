using System.Collections.Generic;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.Mouse
{
    public class TransactionalMouseManagerEventArgs : MouseManagerEventArgs
    {
        private List<ViewModel.Node> _nodesToBeCommitted;
        private MouseManagerEventArgs _innerMouseManagerEventArgs;

        public bool IsTransactional
        {
            get;
            set;
        }

        public List<ViewModel.Node> NodesToBeCommitted
        {
            get
            {
                if (_nodesToBeCommitted == null)
                {
                    _nodesToBeCommitted = new List<ViewModel.Node>();
                }

                return _nodesToBeCommitted;
            }
            set
            {
                _nodesToBeCommitted = value;
            }
        }

        public MouseManagerEventArgs InnerMouseManagerEventArgs
        {
            get
            {
                return _innerMouseManagerEventArgs;
            }
            set
            {
                _innerMouseManagerEventArgs = value;

                Start = _innerMouseManagerEventArgs.Start;
                End = _innerMouseManagerEventArgs.End;
            }
        }
    }
}
