using UnityEngine;

namespace Vexe.Runtime.Extensions
{
	public static class VectorExtensions
	{
		public static float SqrDistanceToV3(this Vector3 v, Vector3 to)
		{
			return (v - to).sqrMagnitude;
		}

		public static float SqrDistanceToV2(this Vector2 v, Vector2 to)
		{
			return (v - to).sqrMagnitude;
		}

		public static Vector2 Add(this Vector2 v, float f)
		{
			return v + new Vector2(f, f);
		}

		public static Vector3 Add(this Vector3 v, float f)
		{
			return v + new Vector3(f, f);
		}

		public static Vector2 Subtract(this Vector2 v, float f)
		{
			return v - new Vector2(f, f);
		}

		public static Vector3 Subtract(this Vector3 v, float f)
		{
			return v - new Vector3(f, f);
		}

		public static bool ApproxEqual(this Vector2 a, Vector2 b)
		{
			return ApproxEqual(new Vector3(a.x, a.y), new Vector3(b.x, b.y));
		}

		public static bool ApproxEqual(this Vector3 a, Vector3 b)
		{
			return a.ApproxEqual(b, 0.001f);
		}

		// http://answers.unity3d.com/questions/131624/vector3-comparison.html#answer-131672
		public static bool ApproxEqual(this Vector3 a, Vector3 b, float angleError)
		{
			//if they aren't the same length, don't bother checking the rest.
			if (!Mathf.Approximately(a.magnitude, b.magnitude))
				return false;

			var cosAngleError = Mathf.Cos(angleError * Mathf.Deg2Rad);

			//A value between -1 and 1 corresponding to the angle.
			//The dot product of normalized Vectors is equal to the cosine of the angle between them.
			//So the closer they are, the closer the value will be to 1. Opposite Vectors will be -1
			//and orthogonal Vectors will be 0.
			var cosAngle = Vector3.Dot(a.normalized, b.normalized);

			//If angle is greater, that means that the angle between the two vectors is less than the error allowed.
			return cosAngle >= cosAngleError;
		}
	}
}
