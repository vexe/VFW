using UnityEngine;

namespace Vexe.Runtime.Types.Examples
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