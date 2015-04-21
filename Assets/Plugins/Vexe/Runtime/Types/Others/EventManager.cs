using System;
using System.Linq;
using System.Reflection;

namespace Vexe.Runtime.Types
{
	/// <summary>
	/// A global generic event system based on C#'s delegates
	/// Subscribe (add), unsubscribe (remove) and raise (fire) game events
    /// Events are objects (classes|structs) that contain information relavent to that event (think EventArgs)
	/// </summary>
	public static class EventManager
	{
		/// <summary>
		/// Lets the specified handler 'handler' handle the event specified by the generic argument T
		/// Note: this will set the delegate directly to the handler so any previous subscribers will be unsubbed
		/// so you might want to use this if you want your event to be handled by a single handler
		/// </summary>
		public static void HandleEvent<T>(Action<T> handler)
		{
			EventManagerInternal<T>.HandleEvent(handler);
		}

		/// <summary>
		/// Subscribes (add) a handler to the event specified by the generic argument `T`
		/// </summary>
		public static void Subscribe<T>(Action<T> handler)
		{
			EventManagerInternal<T>.Subscribe(handler);
		}

		/// <summary>
		/// Unubscribes (remove) a handler from the event specified by the generic argument `T`
		/// </summary>
		public static void Unsubscribe<T>(Action<T> handler)
		{
			EventManagerInternal<T>.Unsubscribe(handler);
		}

		/// <summary>
		/// Raises (fires) the event specified by the generic argument `T`
		/// </summary>
		public static void Raise<T>(T e)
		{
			EventManagerInternal<T>.Raise(e);
		}

		/// <summary>
		/// Removes all the subscribers of the event specified by the generic argument T
		/// </summary>
		public static void Clear<T>()
		{
			EventManagerInternal<T>.Clear();
		}

		/// <summary>
		/// Returns true if the specified handler is subscribed to the event specified by the generic argument T
		/// </summary>
		public static bool Contains<T>(Action<T> handler)
		{
			return EventManagerInternal<T>.Contains(handler);
		}

		/// <summary>
		/// Rasies the game event 'e' to all handlers except the ones specified
		/// </summary>
		public static void RaiseToAllExcept<T>(T e, params Action<T>[] handlers)
		{
			EventManagerInternal<T>.RaiseToAllExcept(e, handlers);
		}

		/// <summary>
		/// Raises the game event 'e' only to the specified handlers
		/// </summary>
		public static void RaiseToOnly<T>(T e, params Action<T>[] handlers)
		{
			EventManagerInternal<T>.RaiseToOnly(e, handlers);
		}

		/// <summary>
		/// Raises the specified event - Resolves the event type at runtime (uses reflection)
		/// </summary>
		public static void DynamicRaise<T>(T e)
		{
			var type = typeof(EventManagerInternal<>);
			var genType = type.MakeGenericType(e.GetType());
			var raise = genType.GetMethod("Raise");
			raise.Invoke(null, new object[] { e });
		}

        /// <summary>
        /// Dynamically subscribes the handler 'handler' to the event whose type is 'eventType'
        /// </summary>
        public static void DynamicSubscribe(Type eventType, Delegate handler)
        {
            var type = typeof(EventManagerInternal<>);
			var genType = type.MakeGenericType(eventType);
            var subscribe = genType.GetMethod("Subscribe");
            subscribe.Invoke(null, new object[] { handler });
        }

        /// <summary>
        /// Dynamically unsubscribes the handler 'handler' from the event whose type is 'eventType'
        /// </summary>
        public static void DynamicUnsubscribe(Type eventType, Delegate handler)
        {
            var type = typeof(EventManagerInternal<>);
			var genType = type.MakeGenericType(eventType);
            var subscribe = genType.GetMethod("Unsubscribe");
            subscribe.Invoke(null, new object[] { handler });
        }

		private static class EventManagerInternal<T>
		{
			private static Action<T> _event;

			public static void HandleEvent(Action<T> handler)
			{
				_event = handler;
			}

			public static void Subscribe(Action<T> handler)
			{
				_event += handler;
			}

			public static void Unsubscribe(Action<T> handler)
			{
				_event -= handler;
			}

			public static void Raise(T e)
			{
                if (_event != null)
                    _event(e);
			}

			public static void RaiseToAllExcept(T e, Action<T>[] handlers)
			{
                var list = _event.GetInvocationList();
                for (int i = 0; i < list.Length; i++)
                {
                    var d = (Action<T>)list[i];
                    if (!handlers.Contains(d))
                        d(e);
                }
			}

			public static void RaiseToOnly(T e, Action<T>[] handlers)
			{
                for (int i = 0; i < handlers.Length; i++)
                    handlers[i](e);
			}

			public static void Clear()
			{
				_event = null;
			}

			public static bool Contains(Action<T> handler)
			{
                if (_event == null)
                    return false;

                var list = _event.GetInvocationList();
                if (list.Length == 1 && list[0].Target == null)
                    return false;

				return list.Contains(handler);
			}
		}
    }
}