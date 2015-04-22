using UnityEngine;
using Vexe.Runtime.Types;

namespace VFWExamples
{
	
	public class SelectionsExample : BetterBehaviour
	{
		[SelectObj]
		public BoxCollider boxCollider;

		[SelectObj]
		public GameObject GO { get; set; }

		[SelectEnum]
		public KeyCode jumpKey;
	}
}