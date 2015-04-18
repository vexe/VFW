using UnityEngine;

namespace Vexe.Runtime.Extensions
{
	public static class RigidbodyExtensions
	{
		public static void FreezeAll(this Rigidbody rb)
		{
			rb.constraints = RigidbodyConstraints.FreezeAll;
		}

		public static void FreezePosition(this Rigidbody rb)
		{
			rb.constraints = RigidbodyConstraints.FreezePosition;
		}

		public static void FreezeRotation(this Rigidbody rb)
		{
			rb.constraints = RigidbodyConstraints.FreezeRotation;
		}

		public static void FreezeAllExcept(this Rigidbody rb, RigidbodyConstraints except)
		{
			rb.constraints = RigidbodyConstraints.FreezeAll - (int)except;
		}
	}
}