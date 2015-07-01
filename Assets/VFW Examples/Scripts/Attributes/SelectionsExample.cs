using UnityEngine;
using Vexe.Runtime.Types;

namespace VFWExamples
{
	public class SelectionsExample : BaseBehaviour
	{
		[SelectObj] public BoxCollider boxCollider;
		[SelectObj] public GameObject GO;
		[SelectEnum] public KeyCode jumpKey;
	}
}