using UnityEngine;
using UnityEngine.UI;

namespace Vexe.Runtime.Types
{ 
	public class CachedBehaviour : BetterBehaviour
	{
		private Transform cachedTransform;
		new public Transform transform
		{
			get
			{
				if (!cachedTransform)
					cachedTransform = base.transform;
				return cachedTransform;
			}
		}

		public RectTransform rectTransform
		{
			get { return transform as RectTransform; }
		}

		public Transform parent
		{
			get { return transform.parent; }
		}

		public int childCount
		{
			get { return transform.childCount; }
		}

		public Vector3 forward
		{
			get { return transform.forward; }
			set { transform.forward = value; }
		}

		public Vector3 right
		{
			get { return transform.right; }
			set { transform.right = value; }
		}

		public Vector3 left
		{
			get { return -right; }
			set { right = -value; }
		}

		public Vector3 up
		{
			get { return transform.up; }
			set { transform.up = value; }
		}

		public Vector3 back
		{
			get { return -forward; }
			set { forward = -value; }
		}

		public Vector3 down
		{
			get { return -up; }
			set { up = -value; }
		}

		public Vector3 position
		{
			get { return transform.position; }
			set { transform.position = value; }
		}

		public Vector3 localPosition
		{
			get { return transform.localPosition; }
			set { transform.localPosition = value; }
		}

		public Quaternion rotation
		{
			get { return transform.rotation; }
			set { transform.rotation = value; }
		}

		public Quaternion localRotation
		{
			get { return transform.localRotation; }
			set { transform.localRotation = value; }
		}

		public Vector3 eulerAngles
		{
			get { return transform.eulerAngles; }
			set { transform.eulerAngles = value; }
		}

		public Vector3 localEulerAngles
		{
			get { return transform.localEulerAngles; }
			set { transform.localEulerAngles = value; }
		}

		public Vector3 localScale
		{
			get { return transform.localScale; }
			set { transform.localScale = value; }
		}
	}
}
