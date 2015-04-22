using System.Collections.Generic;
using UnityEngine;
using Vexe.Runtime.Types;

namespace VFWExamples
{
	public class ReadonlyExample : BetterBehaviour
	{
		[Comment("You can't assign it from the inspector even during edit-time (can only be assigned from code)")]
		[Readonly]
		public Transform ReadonlyAtEditAndRuntime;

		[Comment("Same for this")]
        [Readonly]
        public List<int> SomeList;

		[Comment("You can only assign this during editing")]
		[Readonly(AssignAtEditTime = true)]
        public Transform ReadonlyAtRuntime { get; set; }

        // just demonstrating that ReadonlyAttribute only works when editing in the inspector.
        // you could still modify the list from code.
        [Show] void AddToList(int value)
        {
            SomeList.Add(value);
        }
	}
}