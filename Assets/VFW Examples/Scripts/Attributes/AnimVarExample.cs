using System.Collections.Generic;
using UnityEngine;
using Vexe.Runtime.Types;

namespace VFWExamples
{
	public class AnimVarExample : BaseBehaviour
	{
		[AnimVar(AutoMatch = "Var")] // this will try to auto-assign "Is Dead" to this field
		public string isDeadVar;

        [PerItem, AnimVar]
		public string[] Variables;

		[AnimVar(GetAnimatorMethod = "GetAnim")]
		public string anotherVar;

		private Animator GetAnim()
		{
			// well this is pointless because it would attempt to get the animator from this gameObject
			// by default without having us to specify a method.
			// but imagine we're getting an animtor from somewhere else...
			return GetComponent<Animator>();
		}
	}
}
