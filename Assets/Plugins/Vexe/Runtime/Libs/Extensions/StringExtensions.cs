using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Vexe.Runtime.Extensions
{
	public static class StringExtensions
	{
        /// <summary>
        /// Eg MY_INT_VALUE => MyIntValue
        /// </summary>
        public static string ToTitleCase(this string input)
        {
            var builder = new StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {
                var current = input[i];
                if (current == '_' && i + 1 < input.Length)
                {
                    var next = input[i + 1];
                    if (char.IsLower(next))
                        next = char.ToUpper(next);
                    builder.Append(next);
                    i++;
                }
                else
                    builder.Append(current);
            }
            return builder.ToString();
        }

        /// <summary>
        /// Performs a simple char-by-char comparison to see if input ends with postfix
        /// </summary>
        /// <returns></returns>
        public static bool IsPostfix(this string input, string postfix)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            if (postfix == null)
                throw new ArgumentNullException("postfix");

            if (input.Length < postfix.Length)
                return false;

            for (int i = input.Length - 1, j = postfix.Length - 1; j >= 0; i--, j--)
                if (input[i] != postfix[j])
                    return false;
            return true;
        }

        /// <summary>
        /// Performs a simple char-by-char comparison to see if input starts with prefix
        /// </summary>
        /// <returns></returns>
        public static bool IsPrefix(this string input, string prefix)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            if (prefix == null)
                throw new ArgumentNullException("prefix");

            if (input.Length < prefix.Length)
                return false;

            for (int i = 0; i < prefix.Length; i++)
                if (input[i] != prefix[i])
                    return false;
            return true;
        }

		public static Enum ToEnum(this string str, Type enumType)
		{
			return Enum.Parse(enumType, str) as Enum;
		}

		public static T ToEnum<T>(this string str)
		{
			return (T)Enum.Parse(typeof(T), str);
		}

		public static string FormatWith(this string str, params object[] args)
		{
			return string.Format(str, args);
		}

		/// <summary>
		/// Parses the specified string to the enum value of type T
		/// </summary>
		public static T ParseEnum<T>(this string value)
		{
			return (T)Enum.Parse(typeof(T), value, false);
		}

		/// <summary>
		/// Parses the specified string to the enum whose type is specified by enumType
		/// </summary>
		public static Enum ParseEnum(this string value, Type enumType)
		{
			return (Enum)Enum.Parse(enumType, value, false);
		}

		/// <summary>
		/// Returns the Nth index of the specified character in this string
		/// </summary>
		public static int IndexOfNth(this string str, char c, int n)
		{
			int s = -1;

			for (int i = 0; i < n; i++)
			{
				s = str.IndexOf(c, s + 1);
				if (s == -1) break;
			}
			return s;
		}

		/// <summary>
		/// Removes the last occurance of the specified string from this string.
		/// Returns the modified version.
		/// </summary>
		public static string RemoveLastOccurance(this string s, string what)
		{
			return s.Substring(0, s.LastIndexOf(what));
		}

		/// <summary>
		/// Removes the type extension. ex "Medusa.mp3" => "Medusa"
		/// </summary>
		public static string RemoveExtension(this string s)
		{
			return s.Substring(0, s.LastIndexOf('.'));
		}

		/// <summary>
		/// Returns whether or not the specified string is contained with this string
		/// Credits to JaredPar http://stackoverflow.com/questions/444798/case-insensitive-containsstring/444818#444818
		/// </summary>
		public static bool Contains(this string source, string toCheck, StringComparison comp)
		{
			return source.IndexOf(toCheck, comp) >= 0;
		}

		/// <summary>
		/// "tHiS is a sTring TesT" -> "This Is A String Test"
		/// Credits: http://extensionmethod.net/csharp/string/topropercase 
		/// </summary>
		public static string ToProperCase(this string text)
		{
			System.Globalization.CultureInfo cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
			System.Globalization.TextInfo textInfo = cultureInfo.TextInfo;
			return textInfo.ToTitleCase(text);
		}

		/// <summary>
		/// Ex: "thisIsCamelCase" -> "this Is Camel Case"
		/// Credits: http://stackoverflow.com/questions/155303/net-how-can-you-split-a-caps-delimited-string-into-an-array
		/// </summary>
		public static string SplitCamelCase(this string input)
		{
			return Regex.Replace(input, "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");
		}

		/// <summary>
		/// Ex: "thisIsCamelCase" -> "This Is Camel Case"
		/// </summary>
		public static string SplitPascalCase(this string input)
		{
			return string.IsNullOrEmpty(input) ? input : input.SplitCamelCase().ToUpperAt(0);
		}

		/// <summary>
		/// Nomalizes this string by replacing all '/' with '\' and returns the normalized string instance
		/// </summary>
		public static string NormalizePath(this string input)
		{
			return input.NormalizePath('/', '\\');
		}

		/// <summary>
		/// Normalizes this string by replacing all 'from's by 'to's and returns the normalized instance
		/// Ex: "path/to\dir".NormalizePath('/', '\\') => "path\\to\\dir"
		/// </summary>
		public static string NormalizePath(this string input, char from, char to)
		{
			return input.Replace(from, to);
		}

		/// <summary>
		/// Replaces the character specified by the passed index with newChar and returns the new string instance
		/// </summary>
		public static string ReplaceAt(this string input, int index, char newChar)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			var builder = new StringBuilder(input);
			builder[index] = newChar;
			return builder.ToString();
		}

		/// <summary>
		/// Uppers the character specified by the passed index and returns the new string instance
		/// </summary>
		public static string ToUpperAt(this string input, int index)
		{
			return input.ReplaceAt(index, char.ToUpper(input[index]));
		}

		/// <summary>
		/// Returns true if this string is null or empty
		/// </summary>
		public static bool IsNullOrEmpty(this string str)
		{
			return string.IsNullOrEmpty(str);
		}
	}
}
