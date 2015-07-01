using System;
using UnityEngine;
using Vexe.Runtime.Types;

namespace VFWExamples
{
	/// <summary>
	/// A demo for the requirements attributes.
	/// They're useful when you have a field that's a must for things to work, and so you want to emphasise that it must be assigned
	/// The requirements resolver could help you automatically assign your members if you tell it where to look (in children, in parent, etc)
	/// </summary>
	[HasRequirements]
	public class RequirementsExample : BaseBehaviour
	{
		// This will try a GetComponent on this gameObject for Rigidbody and display a notification if it wasn't there
		[RequiredFromThis]
		public Rigidbody rb;

		// This will add the component to this gameobject if it's not there
		[RequiredFromThis(true)]
		public BoxCollider box;

		// This will try a GetComponentInChildren for the ParticleSystem and display a notification if there was none
		[RequiredFromChildren]
		public ParticleSystem particles;

		// This will attempt to find a BoxCollider at "Child1/Child2"
		// If the children are not there they will get created
		// If the component wasn't found at this path it will get added
		[RequiredFromChildren(Path = "Child1/Child2", Add = true, Create = true)]
		public BoxCollider2D box2d;

		// Similar thing to what we did with the particles, except this time it's from parents and not children
		[RequiredFromParents]
		public SphereCollider sphereCollider;

		// This will attempt to find the parent GameObject named at the path "Parent1"
		// If the parent wasn't found it gets created
		// Note that there's no meaning for "Add" here, cause we're dealing with a GameObject
		[RequiredFromParents(Path = "Parent1", Create = true)]
		public GameObject GO;

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
		[RequiredFromThis(0, true), Inline(1, HideButton = true)]
		public BoxCollider col;

		public ReqTest test;
	}

	[HasRequirements, Serializable]
	public class ReqTest
	{
		// this will get the transform component from the gameObject that the owning script of this object is attached to
		// in our example, ReqTest is a member inside the script RequirementsExample.cs, which is attached to some gameObject
		// the field ReqTest.transform will be assigned to the transform of that gameObject
		[RequiredFromThis]
		public Transform transform;
	}
}