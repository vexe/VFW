using UnityEngine;

namespace Vexe.Runtime.Types
{
	/// <summary>
	/// A convenient script to delegate the drawing of another gameObject
	/// Useful if you have a deep hierarchy of objects, and the object that you're most interested in
	/// is deep down, so you just attach this script to the top-most object,
	/// and have it reference the one you're interested in
	/// </summary>
	
	public class GOInliner : BetterBehaviour
	{
		[Inline]
		public GameObject target;
	}
}