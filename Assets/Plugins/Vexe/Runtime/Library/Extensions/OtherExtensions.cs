using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Vexe.Runtime.Extensions
{
	public static class OtherExtensions
	{
        /// <summary>
        /// http://stackoverflow.com/questions/4108828/generic-extension-method-to-see-if-an-enum-contains-a-flag
        /// </summary>
        public static bool HasFlag(this Enum variable, Enum value)
        {
            if (variable == null)
                return false;

            if (value == null)
                throw new ArgumentNullException("value");

            // Not as good as the .NET 4 version of this function, but should be good enough
            if (!Enum.IsDefined(variable.GetType(), value))
            {
                throw new ArgumentException(string.Format(
                    "Enumeration type mismatch.  The flag is of type '{0}', was expecting '{1}'.",
                    value.GetType(), variable.GetType()));
            }

            ulong num = Convert.ToUInt64(value);
            return ((Convert.ToUInt64(variable) & num) == num);
        }

        public static bool TryReadLine(this StreamReader reader, out string line)
        {
            line = reader.ReadLine();
            return line != null;
		}

		/// <summary>
		/// Returns the value at the specified key in the dictionary if the key exists,
		/// otherwise the default value of the type 'V'
		/// </summary>
		public static V ValueOrDefault<K, V>(this Dictionary<K, V> dictionary, K key)
		{
			return ValueOrDefault(dictionary, key, default(V));
		}

		/// <summary>
		/// Returns the value at the specified key in the dictionary if the key exists,
		/// otherwise the specified default value
		/// </summary>
		public static V ValueOrDefault<K, V>(this Dictionary<K, V> dictionary, K key, V defaultValue)
		{
			V result;
			if (dictionary.TryGetValue(key, out result))
				return result;
			return defaultValue;
		}

		public static void WriteAllLines(this StreamWriter writer, string[] lines)
		{
			for (int i = 0; i < lines.Length; i++)
				writer.WriteLine(lines[i]);
		}

		public static void WriteAtLine(this StreamWriter writer, string[] lines, int lineIdx, string value)
		{
			for (int i = 0; i < lines.Length; i++)
			{
				if (i == lineIdx)
					writer.WriteLine(value);
				else
					writer.WriteLine(lines[i]);
			}
		}

		public static float Sqr(this float value)
		{
			return value * value;
		}

		public static string GetString(this byte[] bytes)
		{
			return Convert.ToBase64String(bytes);
		}

		/// <summary>
		/// Ex: [true, true, false, false] ->
		///	  [1, 1, 0, 0] ->
		///	  12 (in decimal)
		/// </summary>
		public static int ToInt(this IEnumerable<bool> input)
		{
			var builder = new StringBuilder();
			foreach (var x in input)
				builder.Append(Convert.ToByte(x)); // true -> 1, false -> 0
			return Convert.ToInt32(builder.ToString(), 2); // 2 is the base - cause we're converting from base 2 (binary)
		}

		/// <summary>
		/// Credits: http://stackoverflow.com/questions/4448063/how-can-i-convert-an-int-to-an-array-of-bool
		/// </summary>
		public static bool[] ToBooleanArray(this int input)
		{
			return Convert.ToString(input, 2).Select(s => s.Equals('1')).ToArray();
		}

		/// <summary>
		/// Returns true if this object's current value is between (greater or equal to) 'from' and (less than or equal to) 'to'
		/// Credits: http://extensionmethod.net/csharp/type/between
		/// </summary>
		public static bool Between<T>(this T value, T from, T to) where T : IComparable<T>
		{
			return value.CompareTo(from) >= 0 && value.CompareTo(to) <= 0;
		}

		/// <summary>
		/// Sets the builder's length to zero and returns it
		/// </summary>
		public static StringBuilder Clear(this StringBuilder builder)
		{
			builder.Length = 0;
			return builder;
		}
	}
}