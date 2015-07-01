using System;
using System.Collections.Generic;
using System.IO;
using Vexe.Runtime.Extensions;

namespace Vexe.Runtime.Helpers
{
	public static class Assert
	{
		/// <summary>
		/// If the value is null, it gets assigned to the specified getter
		/// If it's still null afterwards, a NullReferenceException is thrown with the specified msg
		/// </summary>
		public static void NotNullAfterAssignment<T>(ref T value, Func<T> get, string msg) where T : class
		{
			if (value == null || value.Equals(null))
				value = get();
			if (value == null || value.Equals(null))
				throw new NullReferenceException(msg);
		}

		/// <summary>
		/// Throws a NullReferenceException if obj was null
		/// </summary>
		public static void NotNull(object obj, string msg)
		{
			if (obj.IsObjectNull())
				throw new NullReferenceException(msg);
		}

		public static void NotNull(object obj)
		{
			NotNull(obj, string.Empty);
		}

		public static void PathExists(string path)
		{
			if (!File.Exists(path))
				throw new InvalidOperationException("Path doesn't exist: " + path);
		}

		/// <summary>
		/// Throws an ArgumentNullException if arg was null
		/// </summary>
		public static void ArgumentNotNull(object arg, string msg)
		{
			if (arg.IsObjectNull())
				throw new ArgumentNullException(msg);
		}

		/// <summary>
		/// Throws an IndexOutOfRangeException if the specified index was out of the specified list's bounds
		/// (less than 0 or greater than or equal to its length)
		/// </summary>
		public static void InBounds<T>(IList<T> list, int index, string msg)
		{
			if (!list.InBounds(index))
				throw new IndexOutOfRangeException(msg);
		}

		public static void True(bool condition, string msg)
		{
			if (!condition)
				throw new AssertionFailure(msg);
		}

		public static void True(bool condition)
		{
			True(condition, string.Empty);
		}

        public static void False(bool condition, string msg)
        {
            True(!condition, msg);
        }

        public static void False(bool condition)
        {
            False(condition, string.Empty);
        }

		public class AssertionFailure : Exception
		{
			public AssertionFailure(string msg) : base(msg)
			{
			}
		}
    }
}