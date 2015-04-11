using UnityEngine;

namespace Vexe.Runtime.Types.Examples
{
	
	public class AssignableExample : BetterBehaviour
	{
		[Assignable] public string SomeString         { get; set; }
		[Assignable] public int SomeInt               { get; set; }
		[Assignable] public GameObject SomeGameObject { get; set; }
	}
}