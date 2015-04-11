using UnityEngine;

namespace Vexe.Runtime.Types.Examples
{
	// only the types in this example are currently supported for inlining.
	// if you want to add support to more built-in types, have a look at InlineAttributeDrawer,
	// it's very simple to do so...
	public class InlineExample : BetterBehaviour
	{
		[Inline] public Transform trans;
		[Inline] public Rigidbody rigid;
		[Inline] public Camera cam;
		[Inline] public MeshRenderer meshrend;
		[Inline] public SphereCollider sphereCol;
		[Inline] public Animator anim;

		// This is a very common combination to use
		// We require the component (in this case from this gameObject),
		// if it doesn't exist it gets added
		// And then we inline it, and hide it from the gameObject
		// NOTE: Both of these are composite attributes,
		// meaning they have an order in which they are drawn
		// RequiredFromThis should be drawn before Inline
		// Otherwise Inline will try to hide a target that's not there yet!
		// So we use an id of 0 to the first attribute, and 1 to the second.
		// 0 comes before 1 :)
		[RequiredFromThis(0, true), Inline(1, HideTarget = true)]
		public BoxCollider col;

		// inlining a game object will draw the editor for all its components
		[Inline]
		public GameObject GO { get; set; }
	}
}
