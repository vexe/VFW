using System;
using UnityEngine;

namespace Vexe.Runtime.Types
{
	/// <summary>
	/// Annotate a string with this to constrain it to match a specific regular expression pattern
	/// If the input doesn't match the pattern, the string field value will fallback to the most recent valid value
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
	public class RegexAttribute : CompositeAttribute
	{
		/// <summary>
		/// The pattern the string must match
		/// </summary>
		public readonly string pattern;

		public RegexAttribute(int id, string pattern) : base(id)
		{
			this.pattern = pattern;
		}

		public RegexAttribute(string pattern) : this(-1, pattern)
		{
		}
	}
}