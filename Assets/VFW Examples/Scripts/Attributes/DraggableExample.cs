using UnityEngine;
using Vexe.Runtime.Types;

namespace VFWExamples
{
	
	public class DraggableExample : BetterBehaviour
	{
		[Comment("Try and drag these two fields around")]
		[Draggable]
		public GameObject dragMe1;

		[Draggable]
		public GameObject DragMe2 { get; set; }
	}
}