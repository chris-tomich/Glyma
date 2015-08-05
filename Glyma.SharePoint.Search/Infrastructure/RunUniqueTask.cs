using System;
using System.Collections.Generic;
using System.Threading;

namespace Glyma.SharePoint.Search
{
   public class RunUniqueTask<K> : IDisposable
   {
      private bool _isMasterTask = false;
      private K _taskKey;
      private ManualResetEvent _taskSignal;
      private RunningTasksQueue<K> _runningTasks;


      public RunUniqueTask(RunningTasksQueue<K> runningTasks, K key, TimeSpan slaveWaitDuration)
      {
         if (runningTasks == null)
         {
            throw new ArgumentNullException("runningTasks");
         }

         if (default(K) == null & key == null)
         {
            throw new ArgumentNullException("key");
         }

         _runningTasks = runningTasks;
         _taskKey = key;
         _taskSignal = _runningTasks.Register(_taskKey, out _isMasterTask);
         if (!_isMasterTask)
         {
            _taskSignal.WaitOne(slaveWaitDuration);
         }
      }


      public void Dispose()
      {
         SendSignalForMaster();
         GC.SuppressFinalize(this);
      }


      ~RunUniqueTask()
      {
         SendSignalForMaster();
      }


      protected void SendSignalForMaster()
      {
         if (_taskSignal != null && _isMasterTask)
         {
            _taskSignal.Set();
            if (_runningTasks != null)
            {
               _runningTasks.Unregister(_taskKey);
            }
         }
      }
   }
}
