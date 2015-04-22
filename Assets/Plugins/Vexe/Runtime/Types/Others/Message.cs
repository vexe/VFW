using System;

namespace Vexe.Runtime.Types
{
    /// <summary>
    /// A global generic event/messaging system based on C#'s delegates
    /// Subscribe/Add, Unsubscribe/Remove and Raise/Fire/Post game events/messages
    /// Events/messages are objects (classes|structs) that contain information relavent to that event/message (think EventArgs)
    /// See MessageExample.cs for sample usage
    /// </summary>
    public class Message<T>
    {
        public static readonly Message<T> Instance = new Message<T>();

        /// <summary>
        /// Lets the specified handler/listener 'h' handle the event/message specified by the generic argument T
        /// Note: this will set the delegate directly to the handler so any previous subscribers will be unsubbed
        /// so you might want to use this if you want your event/message to be handled by a single handler
        /// </summary>
        public static void Set(Action<T> handler)
        {
            Instance.set(handler);
        }

        /// <summary>
        /// Subscribes/Adds a handler/listener to the event/message specified by the generic argument `T`
        /// </summary>
        public static void Add(Action<T> handler)
        {
            Instance.add(handler);
        }

        /// <summary>
        /// Unubscribes/Remove a handler/listener from the event/message specified by the generic argument `T`
        /// </summary>
        public static void Remove(Action<T> handler)
        {
            Instance.remove(handler);
        }

        /// <summary>
        /// Raises/Fires/Posts the specified event/message
        /// </summary>
        public static void Post(T e)
        {
            Instance.post(e);
        }

        /// <summary>
        /// Removes all the subscribers/listeners of the event/message specified by the generic argument T
        /// </summary>
        public static void Clear()
        {
            Instance.clear();
        }

        /// <summary>
        /// Returns true if the specified handler/listener is subscribed to the event/message specified by the generic argument T
        /// </summary>
        public static bool Contains(Action<T> handler)
        {
            return Instance.contains(handler);
        }

        private Action<T> _delegate = delegate { };

        public void set(Action<T> handler)
        {
            _delegate = handler;
        }

        public void add(Action<T> handler)
        {
            _delegate += handler;
        }

        public void remove(Action<T> handler)
        {
            _delegate -= handler;
        }

        public void post(T e)
        {
            _delegate(e);
        }

        public void clear()
        {
            _delegate = null;
        }

        public bool contains(Action<T> handler)
        {
            if (_delegate == null)
                return false;

            var list = _delegate.GetInvocationList();
            if (list.Length == 1 && list[0].Target == null)
                return false;

            for (int i = 0; i < list.Length; i++)
                if ((Action<T>)list[i] == handler)
                    return true;
            return false;
        }
    }

    /// <summary>
    /// Exists for pure conveneince.
    /// You might be like me and don't like the repetitiveness in Message<SomeMessage>.Post(new SomeMessage...)
    /// So you could just say Message.Post(new SomeMessage...)
    /// </summary>
    public static class Message
    {
        public static void Post<T>(T e)
        {
            Message<T>.Post(e);
        }
    }
}
