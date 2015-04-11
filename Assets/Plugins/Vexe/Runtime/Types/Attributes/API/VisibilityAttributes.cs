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
}
