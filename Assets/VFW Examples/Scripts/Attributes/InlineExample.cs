using UnityEngine;
using Vexe.Runtime.Types;

namespace VFWExamples
{
	// only the types in this example are currently supported for inlining.
	// if you want to add support to more built-in types, have a look at InlineDrawer,
	// it's very simple to do so...
    [HasRequirements]
	public class InlineExample : BaseBehaviour
	{
		[Inline] public Transform trans;
		[Inline] public Rigidbody rigid;
		[Inline] public Camera cam;
		[Inline] public MeshRenderer meshrend;
		[Inline] public SphereCollider sphereCol;
		[Inline] public Animator anim;

		// inlining a game object will draw the editor for all its components
		[Inline] public GameObject go;
	}
}
