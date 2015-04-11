using System;

namespace Vexe.Runtime.Types
{
	/// <summary>
	/// UnityEngine.Objects annotated with this attribute will have a selection button appear beside them
	/// so that you could select a value from objects in the scene
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
	public class SelectObjAttribute : DrawnAttribute
	{
	}
}