using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.SharePoint.Administration;

namespace Glyma.SharePoint.Search
{
   public class BasicCache<K, T> : IEnumerable<KeyValuePair<K, BasicCacheItem<T>>>, IDisposable
   {
      private Dictionary<K, BasicCacheItem<T>> _cacheItems = new Dictionary<K, BasicCacheItem<T>>();
      private object _cacheLock = new object();
      private Timer _autoExpireItemsTimer;
      private TimerCallback _autoExpireItemsCallback;
      private TimeSpan _autoExpireItemsPeriod = BasicCacheConstants.DefaultAutoExpirePeriod;
      private int _maxItems = BasicCacheConstants.DefaultMaxItems;
      private bool _disposed = false;


      public BasicCache()
      {
         _autoExpireItemsCallback = RemoveExpiredItemsCallback;
         _autoExpireItemsTimer = new Timer(RemoveExpiredItemsCallback, null, (int)_autoExpireItemsPeriod.TotalMilliseconds, Timeout.Infinite);
      }


      public TimeSpan AutoExpireItemsPeriod
      {
         get
         {
            lock (_cacheLock)
            {
               return _autoExpireItemsPeriod; 
            }
         }

         set
         {
            lock (_cacheLock)
            {
               if (value.TotalSeconds < 0)
               {
                  throw new ArgumentException("AutoExpireItemsPeriod cannot be less than zero.");
               }
               _autoExpireItemsPeriod = value;
               _autoExpireItemsTimer.Change((int)_autoExpireItemsPeriod.TotalMilliseconds, Timeout.Infinite);
            }
         }
      }


      public int MaxItems
      {
         get
         {
            lock (_cacheLock)
            {
               return _maxItems;
            }
         }

         set
         {
            lock (_cacheLock)
            {
               if (value < 0)
               {
                  throw new ArgumentException("MaxItems cannot be less than zero.");
               }
               _maxItems = value;
            }
         }
      }


      public int Count
      {
         get
         {
            lock (_cacheLock)
            {
               return _cacheItems.Count;
            }
         }
      }


      public void Add(K key, T value, DateTime absoluteExpiration, TimeSpan slidingExpiration)
      {
         if (default(K) == null && key == null)
         {
            throw new ArgumentNullException("key");
         }

         lock (_cacheLock)
         {
            if (Count < MaxItems)
            {
               BasicCacheItem<T>.ValidateExpiration(absoluteExpiration, slidingExpiration);
               BasicCacheItem<T> cacheItem = new BasicCacheItem<T>(value);
               cacheItem.DateAddedToCache = DateTime.Now;
               cacheItem.AbsoluteExpiration = absoluteExpiration;
               cacheItem.SlidingExpiration = slidingExpiration;

               _cacheItems.Add(key, cacheItem);
            }
            else
            {
               GlymaSearchLogger.WriteTrace(LogCategoryId.Connector, TraceSeverity.Medium, "Cache item limit of " + MaxItems + " has been reached.  Item with key " + key.ToString() + " will not be added to the cache.");
            }
         }
      }


      public void Insert(K key, T value, DateTime absoluteExpiration, TimeSpan slidingExpiration)
      {
         if (default(K) == null && key == null)
         {
            throw new ArgumentNullException("key");
         }

         lock (_cacheLock)
         {
            if (_cacheItems.ContainsKey(key))
            {
               BasicCacheItem<T> cacheItem = _cacheItems[key];
               cacheItem.Value = value;
               cacheItem.DateAddedToCache = DateTime.Now;
            }
            else
            {
               Add(key, value, absoluteExpiration, slidingExpiration);
            }
         }
      }


      public T Get(K key)
      {
         T result = default(T);

         if (default(K) == null && key == null)
         {
            throw new ArgumentNullException("key");
         }

         lock (_cacheLock)
         {
            if (_cacheItems.ContainsKey(key))
            {
               BasicCacheItem<T> cacheItem = _cacheItems[key];
               if (!cacheItem.IsExpired())
               {
                  result = cacheItem.Value;
               }
               else
               {
                  _cacheItems.Remove(key);
               }
            }
         }

         return result;
      }


      public void Remove(K key)
      {
         if (default(K) == null && key == null)
         {
            throw new ArgumentNullException("key");
         }

         lock (_cacheLock)
         {
            if (_cacheItems.ContainsKey(key))
            {
               _cacheItems.Remove(key);
            }
         }
      }


      public void RemoveExpiredItems()
      {
         List<K> keysToRemove = new List<K>();

         lock (_cacheLock)
         {
            foreach (KeyValuePair<K, BasicCacheItem<T>> cacheItem in _cacheItems)
            {
               if (cacheItem.Value.IsExpired())
               {
                  keysToRemove.Add(cacheItem.Key);
               }
            }

            foreach (K key in keysToRemove)
            {
               _cacheItems.Remove(key);
            }
         }
      }


      public void RemoveExpiredItemsCallback(Object stateInfo)
      {
         lock (_cacheLock)
         {
            GlymaSearchLogger.WriteTrace(LogCategoryId.Connector, TraceSeverity.Medium, "Auto expiring items in cache.");
            RemoveExpiredItems();
            if (_autoExpireItemsTimer != null)
            {
               _autoExpireItemsTimer.Change((int)_autoExpireItemsPeriod.TotalMilliseconds, Timeout.Infinite);
            }
         }
      }


      public void Clear()
      {
         lock (_cacheLock)
         {
            _cacheItems.Clear();
         }
      }


      public IEnumerator<KeyValuePair<K, BasicCacheItem<T>>> GetEnumerator()
      {
         lock (_cacheLock)
         {
            return (_cacheItems as IEnumerable<KeyValuePair<K, BasicCacheItem<T>>>).GetEnumerator();
         }
      }


      System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
      {
         lock (_cacheLock)
         {
            return _cacheItems.GetEnumerator();
         }
      }


      public void Dispose()
      {
         Dispose(true);
         GC.SuppressFinalize(this);
      }


      ~BasicCache()
      {
         Dispose(false);
      }


      protected virtual void Dispose(bool disposing)
      {
         if (disposing)
         {
            if (!_disposed)
            {
               if (_autoExpireItemsTimer != null)
               {
                  _autoExpireItemsTimer.Dispose();
                  _autoExpireItemsTimer = null;
               }
               _disposed = true;
            }
         }
      }
   }
}
