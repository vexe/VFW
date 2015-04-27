using UnityEngine;
using Vexe.Runtime.Types;

namespace VFWExamples
{
	public class BetterVectorExample : BetterBehaviour
	{
		[BetterV2]
		public Vector2 vector2Field;

		[BetterV2]
		public Vector2 Vector2Property { get; set; }

		[BetterV3]
		public Vector3 vector3Field;

		[BetterV3]
		public Vector3 Vector3Property { get; set; }
	}
}
