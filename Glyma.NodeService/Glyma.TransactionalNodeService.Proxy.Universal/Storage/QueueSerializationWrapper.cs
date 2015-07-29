using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace TransactionalNodeService.Proxy.Universal.Storage
{
    [DataContract]
    public class QueueSerializationWrapper<T>
    {
        private List<KeyValuePair<int, T>> _wrappedQueue;

        public QueueSerializationWrapper(Queue<T> queueToWrap)
            : this(queueToWrap, null)
        {
        }

        public QueueSerializationWrapper(Queue<T> queueToWrap, object lastItem)
        {
            int counter = 0;

            if (lastItem != null && lastItem is T)
            {
                T lastItemAsT = (T)lastItem;
                WrappedQueue.Add(new KeyValuePair<int, T>(counter, lastItemAsT));

                counter++;
            }

            if (queueToWrap != null)
            {
                foreach (T objectToStore in queueToWrap)
                {
                    WrappedQueue.Add(new KeyValuePair<int, T>(counter, objectToStore));

                    counter++;
                }
            }
        }

        [DataMember]
        public List<KeyValuePair<int, T>> WrappedQueue
        {
            get
            {
                if (_wrappedQueue == null)
                {
                    _wrappedQueue = new List<KeyValuePair<int, T>>();
                }

                return _wrappedQueue;
            }
            set
            {
                _wrappedQueue = value;
            }
        }

        public Queue<T> DeserializeQueue()
        {
            var orderedQueue = WrappedQueue.OrderBy(keyValuePair => keyValuePair.Key);

            Queue<T> queue = new Queue<T>();

            foreach (var keyValuePair in orderedQueue)
            {
                queue.Enqueue(keyValuePair.Value);
            }

            return queue;
        }
    }
}
