using System;

namespace Vexe.Runtime.Types
{
	/// <summary>
	/// Annotate a Vector2 with this to get a foldout offering more functionalities (copy, paste, normalize, randomize and reset)
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
	public class BetterV2Attribute : DrawnAttribute
	{
	}

	/// <summary>
	/// Annotate a Vector3 with this to get a foldout offering more functionalities (copy, paste, normalize, randomize and reset)
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
	public class BetterV3Attribute : DrawnAttribute
	{
	}
}