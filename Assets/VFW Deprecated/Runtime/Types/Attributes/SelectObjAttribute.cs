using System;

namespace Vexe.Runtime.Types
{
	[Obsolete("Dumb useless attribute")]
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
	public class SelectObjAttribute : DrawnAttribute
	{
	}
}