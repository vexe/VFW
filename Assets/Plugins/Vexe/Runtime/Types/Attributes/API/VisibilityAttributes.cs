using System;

namespace Vexe.Runtime.Types
{
	/// <summary>
	/// Annotate any member with this attribute to expose it even if it wasn't serializable
	/// This is the only way to expose method and properties with side effects (i.e. not auto-properties)
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
	public class ShowAttribute : Attribute
	{
	}

	/// <summary>
	/// A shorter alternative to HideInInspector - applicable to fields and properties
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
	public class HideAttribute : Attribute
	{
	}

	/// <summary>
	/// Annotate members with this attribute to make then visible only when a certain condition is met
	/// The name of the condition method must be passed as an argument to the attribute constructor
	/// The method should return a boolean, and take no parameters
	/// </summary>
	public class VisibleWhenAttribute : Attribute
	{
		public readonly string ConditionMethod;

		public VisibleWhenAttribute(string conditionMethod)
		{
			this.ConditionMethod = conditionMethod;
		}
	}
}
