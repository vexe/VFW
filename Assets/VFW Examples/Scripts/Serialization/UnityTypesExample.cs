using System.Collections.Generic;
using UnityEngine;
using Vexe.Runtime.Types;

namespace VFWExamples
{
	public class UnityTypesExample : BetterBehaviour
	{
		public Vector3 vector3;
		public Color color { get; set; }
		public Quaternion quaternion;

		[Serialize]
		private Vector2 vector2 { get; set; }

		[Serialize]
		protected Bounds bounds;

		[Serialize]
		private LayerMask mask;
	}
}
