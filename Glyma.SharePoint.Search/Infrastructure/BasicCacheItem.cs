using System;
using System.Collections.Generic;

namespace Glyma.SharePoint.Search
{
   public class BasicCacheItem<T>
   {
      private T _value;
      private DateTime _dateAddedToCache;
      private DateTime _absoluteExpiration = BasicCacheConstants.NoAbsoluteExpiration;
      private TimeSpan _slidingExpiration = BasicCacheConstants.NoSlidingExpiration;
      private object _itemLock = new object();


      public BasicCacheItem(T item)
      {
         Value = item;
      }


      public T Value
      {
         get
         {
            lock (_itemLock)
            {
               return _value;
            }
         }

         set
         {
            lock (_itemLock)
            {
               _value = value;
            }
         }
      }


      public DateTime DateAddedToCache
      {
         get
         {
            lock (_itemLock)
            {
               return _dateAddedToCache;
            }
         }

         set
         {
            lock (_itemLock)
            {
               _dateAddedToCache = value;
            }
         }
      }


      public DateTime AbsoluteExpiration
      {
         get
         {
            lock (_itemLock)
            {
               return _absoluteExpiration;
            }
         }

         set
         {
            lock (_itemLock)
            {
               _absoluteExpiration = value;
               _slidingExpiration = BasicCacheConstants.NoSlidingExpiration;
            }
         }
      }


      public TimeSpan SlidingExpiration
      {
         get
         {
            lock (_itemLock)
            {
               return _slidingExpiration;
            }
         }

         set
         {
            lock (_itemLock)
            {
               if (value.TotalSeconds < 0)
               {
                  throw new ArgumentException("SlidingExpiration cannot be less than zero.");
               }
               _slidingExpiration = value;
               _absoluteExpiration = BasicCacheConstants.NoAbsoluteExpiration;
            }
         }
      }


      public bool IsExpired()
      {
         bool result = false;

         lock (_itemLock)
         {
            if (AbsoluteExpiration != BasicCacheConstants.NoAbsoluteExpiration || SlidingExpiration != BasicCacheConstants.NoSlidingExpiration)
            {
               if (AbsoluteExpiration == BasicCacheConstants.NoAbsoluteExpiration)
               {
                  result = DateTime.Now > DateAddedToCache.Add(SlidingExpiration);
               }
               else if (SlidingExpiration == BasicCacheConstants.NoSlidingExpiration)
               {
                  result = DateTime.Now > AbsoluteExpiration;
               }
            }
         }

         return result;
      }


      public void ValidateExpiration()
      {
         lock (_itemLock)
         {
            ValidateExpiration(AbsoluteExpiration, SlidingExpiration);
         }
      }


      public static bool IsValidExpiration(DateTime absoluteExpiration, TimeSpan slidingExpiration)
      {
         return absoluteExpiration == BasicCacheConstants.NoAbsoluteExpiration || slidingExpiration == BasicCacheConstants.NoSlidingExpiration;
      }


      public static void ValidateExpiration(DateTime absoluteExpiration, TimeSpan slidingExpiration)
      {
         if (!IsValidExpiration(absoluteExpiration, slidingExpiration))
         {
            throw new ArgumentException("The absoluteExpiration and slidingExpiration parameters are both set for the item you are trying to add to the Cache.");
         }
      }
   }
}
