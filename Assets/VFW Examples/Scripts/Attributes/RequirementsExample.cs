using UnityEngine;

namespace Vexe.Runtime.Types.Examples
{
	/// <summary>
	/// A demo for the requirements attributes.
	/// They're useful when you have a field that's a must for things to work, and so you want to emphasise that it must be assigned
	/// The requirements resolver could help you automatically assign your members if you tell it where to look (in children, in parent, etc)
	/// </summary>
	[HasRequirements]
	public class RequirementsExample : BetterBehaviour
	{
		// This will try a GetComponent on this gameObject for Rigidbody and display a notification if it wasn't there
		[RequiredFromThis]
		public Rigidbody rb;

		// This will add the component to this gameobject if it's not there
		[RequiredFromThis(true)]
		public BoxCollider box;

		// This will try a GetComponentInChildren for the ParticleSystem and display a notification if there was none
		[RequiredFromChildren]
		public ParticleSystem Particles { get; set; }

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
		public GameObject GO { get; set; }


		public ReqTest test;
	}

	[HasRequirements]
	public class ReqTest
	{
		// this will get the transform component from the gameObject that the owning script of this object is attached to
		// in our example, ReqTest is a member inside the script RequirementsExample.cs, which is attached to some gameObject
		// the field ReqTest.transform will be assigned to the transform of that gameObject
		[RequiredFromThis]
		public Transform transform;
	}
}