using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace TransactionalNodeService.Proxy
{
    public class EventRegister<ContextObject, EventArguments> where EventArguments : EventRegisterEventArgs
    {
        protected struct CallbackPersistenceInfo<PersistenceInfoEventArguments> where PersistenceInfoEventArguments : EventRegisterEventArgs
        {
            public bool PersistEventSubscription;
            public object State;
            public EventHandler<PersistenceInfoEventArguments> Callback;
        }

        protected Dictionary<ContextObject, List<CallbackPersistenceInfo<EventArguments>>> _registeredEvents = null;

        public EventRegister()
        {
        }

        protected Dictionary<ContextObject, List<CallbackPersistenceInfo<EventArguments>>> RegisteredEvents
        {
            get
            {
                if (_registeredEvents == null)
                {
                    _registeredEvents = new Dictionary<ContextObject, List<CallbackPersistenceInfo<EventArguments>>>();
                }

                return _registeredEvents;
            }
        }

        public void RegisterEvent(ContextObject context, EventHandler<EventArguments> CallbackEventhandler, bool persistEventSubscription, object state)
        {
            List<CallbackPersistenceInfo<EventArguments>> callbacks;

            if (RegisteredEvents.ContainsKey(context))
            {
                callbacks = RegisteredEvents[context];
            }
            else
            {
                callbacks = new List<CallbackPersistenceInfo<EventArguments>>();

                RegisteredEvents.Add(context, callbacks);
            }

            CallbackPersistenceInfo<EventArguments> callback = new CallbackPersistenceInfo<EventArguments>();
            callback.PersistEventSubscription = persistEventSubscription;
            callback.Callback = CallbackEventhandler;
            callback.State = state;

            if (!callbacks.Contains(callback))
            {
                callbacks.Add(callback);
            }
        }

        public void RegisterEvent(ContextObject context, EventHandler<EventArguments> CallbackEventhandler, object state)
        {
            RegisterEvent(context, CallbackEventhandler, false, state);
        }

        public void RegisterEvent(ContextObject context, EventHandler<EventArguments> CallbackEventhandler, bool persistEventSubscription)
        {
            RegisterEvent(context, CallbackEventhandler, persistEventSubscription, null);
        }

        public void RegisterEvent(ContextObject context, EventHandler<EventArguments> CallbackEventhandler)
        {
            RegisterEvent(context, CallbackEventhandler, false);
        }

        public void FireEvent(ContextObject context, object sender, EventArguments eventArgs)
        {
            if (RegisteredEvents.ContainsKey(context))
            {
                List<CallbackPersistenceInfo<EventArguments>> callbacks = RegisteredEvents[context];
                List<CallbackPersistenceInfo<EventArguments>> persistingCallbacks = new List<CallbackPersistenceInfo<EventArguments>>();

                foreach (CallbackPersistenceInfo<EventArguments> callbackTuple in callbacks)
                {
                    eventArgs.Context = context;
                    eventArgs.State = callbackTuple.State;

                    callbackTuple.Callback(sender, eventArgs);

                    if (callbackTuple.PersistEventSubscription)
                    {
                        persistingCallbacks.Add(callbackTuple);
                    }
                }

                RegisteredEvents[context] = persistingCallbacks;
            }
        }
    }
}
