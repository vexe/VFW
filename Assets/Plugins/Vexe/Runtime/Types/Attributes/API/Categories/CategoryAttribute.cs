using System;
using UnityEngine;

namespace Vexe.Runtime.Types
{
	/// <summary>
	/// Annotate a member with this to include it in a certain category with a certain display order
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
	public class CategoryAttribute : Attribute
	{
		/// <summary>
		/// The name of the category to add the member to
		/// </summary>
		public readonly string name;

		public CategoryAttribute(string name)
		{
			this.name = name;
		}
	}
}