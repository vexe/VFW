using UnityEngine;

namespace Vexe.Runtime.Types.Examples
{
	
	public class ReadonlyExample : BetterBehaviour
	{
		[Comment("You can't assign it from the inspector even during edit-time (can only be assigned from code)")]
		[Readonly]
		public Transform ReadonlyAtEditAndRuntime { get; set; }

		[Comment("You can only assign this during editing")]
		[Readonly(AssignAtEditTime = true)]
		public Transform readonlyAtRuntime;
	}
}