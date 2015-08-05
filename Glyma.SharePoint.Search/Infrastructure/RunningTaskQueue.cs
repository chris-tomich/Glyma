using System;
using System.Collections.Generic;
using System.Threading;

namespace Glyma.SharePoint.Search
{
   public class RunningTasksQueue<K> : IEnumerable<KeyValuePair<K, ManualResetEvent>>, IDisposable
   {
      private Dictionary<K, ManualResetEvent> _runningTasks = new Dictionary<K, ManualResetEvent>();
      private object _queueLock = new object();
      private bool _disposed = false;


      public int Count
      {
         get
         {
            lock (_queueLock)
            {
               return _runningTasks.Count;
            }
         }
      }


      public ManualResetEvent Register(K key, out bool isNew)
      {
         ManualResetEvent taskSignal = null;
         isNew = false;

         if (default(K) == null && key == null)
         {
            throw new ArgumentNullException("key");
         }

         lock (_queueLock)
         {
            taskSignal = Get(key);
            if (taskSignal == null)
            {
               isNew = true;
               taskSignal = new ManualResetEvent(false);
               _runningTasks.Add(key, taskSignal);
            }
         }

         return taskSignal;
      }


      public ManualResetEvent Get(K key)
      {
         ManualResetEvent taskSignal = null;

         if (default(K) == null && key == null)
         {
            throw new ArgumentNullException("key");
         }

         lock (_queueLock)
         {
            if (_runningTasks.ContainsKey(key))
            {
               taskSignal = _runningTasks[key];
            }
         }

         return taskSignal;
      }


      public void Unregister(K key)
      {
         if (default(K) == null && key == null)
         {
            throw new ArgumentNullException("key");
         }

         lock (_queueLock)
         {
            _runningTasks.Remove(key);
         }
      }


      public void Clear()
      {
         lock (_queueLock)
         {
            _runningTasks.Clear();
         }
      }


      public IEnumerator<KeyValuePair<K, ManualResetEvent>> GetEnumerator()
      {
         lock (_queueLock)
         {
            return (_runningTasks as IEnumerable<KeyValuePair<K, ManualResetEvent>>).GetEnumerator();
         }
      }


      System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
      {
         lock (_queueLock)
         {
            return _runningTasks.GetEnumerator();
         }
      }


      public void Dispose()
      {
         Dispose(true);
         GC.SuppressFinalize(this);
      }


      ~RunningTasksQueue()
      {
         Dispose(false);
      }


      protected virtual void Dispose(bool disposing)
      {
         if (disposing)
         {
            if (!_disposed)
            {
               if (_runningTasks != null)
               {
                  foreach (ManualResetEvent taskSignal in _runningTasks.Values)
                  {
                     if (taskSignal != null)
                     {
                        taskSignal.Close();
                     }
                  }
                  _runningTasks.Clear();
                  _runningTasks = null;
               }
               _disposed = true;
            }
         }
      }
   }
}
