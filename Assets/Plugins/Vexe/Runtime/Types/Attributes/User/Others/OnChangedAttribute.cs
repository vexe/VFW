using UnityEngine;
using System;

namespace Vexe.Runtime.Types
{
	/// <summary>
	/// Annotate a field/property with this attribute if you're interested of when the value of the field/property changes
	/// You can setup a method to call passing the new value, or a field/property to set the new value to.
    /// Note that when applying it on collections (list, array, dictionary) it will give you a callback when the collection count changes
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
	public class OnChangedAttribute : CompositeAttribute
	{
		/// <summary>
		/// The name of a method to be called when the value changes
		/// It has to have a void return, and only one argument of the same field type
		/// It doesn't matter what the access modifier on the method is
		/// It could be instance, or static
		/// </summary>
		public string Call { get; set; }

		/// <summary>
		/// The name of a field/property to set the changed value to
		/// </summary>
		public string Set  { get; set; }

		public OnChangedAttribute(int id, string call) : base(id)
		{
			Call = call;
		}

		public OnChangedAttribute(string call) : this(-1, call)
		{
		}

		public OnChangedAttribute() : this(string.Empty)
		{
		}
	}
}
