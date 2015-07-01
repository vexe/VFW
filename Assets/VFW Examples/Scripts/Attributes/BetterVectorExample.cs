using UnityEngine;
using Vexe.Runtime.Types;

namespace VFWExamples
{
	public class BetterVectorExample : BaseBehaviour
	{
		[BetterV2] public Vector2 vector2Field;
		[BetterV3] public Vector3 vector3Field;
	}
}
