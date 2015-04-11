using System;

namespace Vexe.Runtime.Types
{
	/// <summary>
	/// Annotate vectors (2/3) with this to get a foldout besides the vector offering more functionalities (copy, paste, normalize, randomize and reset)
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
	public class BetterVectorAttribute : DrawnAttribute
	{
	}
}