using UnityEngine;
using Vexe.Runtime.Extensions;

namespace Vexe.Runtime.Helpers
{
	public static class GOHelper
	{
		private static GameObject _emptyGO;

		/// <summary>
		/// Returns a cached reference to an empty GO (think NullObject)
		/// If none is found, a new one is created
		/// </summary>
		public static GameObject EmptyGO
		{
			get
			{
				if (_emptyGO == null)
				{
					string na = "__Empty";
					_emptyGO = GameObject.Find(na) ?? CreateGo(na, null, HideFlags.HideInHierarchy);
				}
				return _emptyGO;
			}
		}

		/// <summary>
		/// Creates and returns a GameObject with the passed name, parent and HideFlags
		/// </summary>
		public static GameObject CreateGo(string name, Transform parent = null, HideFlags flags = HideFlags.None)
		{
			var go = new GameObject(name);
			go.hideFlags = flags;
			if (parent)
			{
				go.transform.parent = parent;
				go.transform.Reset();
			}
			return go;
		}

		/// <summary>
		/// Creates a GameObject with a MonoBehaviour specified by the generic T arg - returns the MB added
		/// </summary>
		public static T CreateGoWithMb<T>(string name, out GameObject createdGo, Transform parent = null, HideFlags flags = HideFlags.None) where T : MonoBehaviour
		{
			createdGo = CreateGo(name, parent, flags);
			var comp = createdGo.AddComponent<T>();
			return comp;
		}

		/// <summary>
		/// Creates a GameObject with a MonoBehaviour specified by the generic T arg - returns the MB added
		/// </summary>
		public static T CreateGoWithMb<T>(string name, Transform parent = null, HideFlags flags = HideFlags.None) where T : MonoBehaviour
		{
			var go = new GameObject();
			return CreateGoWithMb<T>(name, out go, parent, flags);
		}
	}
}