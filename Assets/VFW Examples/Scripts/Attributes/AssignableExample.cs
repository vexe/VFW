using UnityEngine;
using Vexe.Runtime.Types;

namespace VFWExamples
{
	public class AssignableExample : BaseBehaviour
	{
		[Assignable] public string SomeString;
		[Assignable] public int SomeInt;
		[Assignable] public GameObject SomeGameObject;
	}
}