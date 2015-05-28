using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Vexe.Runtime.Extensions;
using UnityObject = UnityEngine.Object;

namespace Vexe.Editor.Helpers
{
	public static class PrefabHelper
	{
		public static IEnumerable<T> GetComponentPrefabs<T>() where T : Component
		{
			return Resources.FindObjectsOfTypeAll<UnityObject>()
							.OfType<T>()
							.Where(x => PrefabHelper.IsPrefab(x.gameObject));
		}

		public static IEnumerable<Component> GetComponentPrefabs(Type componentType)
		{
			return Resources.FindObjectsOfTypeAll<UnityObject>()
							.OfType<Component>()
							.Where(c => c.GetType().IsA(componentType))
							.Where(x => PrefabHelper.IsPrefab(x.gameObject));
		}

		public static IEnumerable<GameObject> GetGameObjectPrefabs()
		{
			return Resources.FindObjectsOfTypeAll<GameObject>()
							.Where(PrefabHelper.IsPrefab);
		}

		/// <summary>
		/// Returns true if the specified gameObject is a prefab instance (connected to a prefab)
		/// </summary>
		public static bool IsPrefabInstance(GameObject gameObject)
		{
			return PrefabUtility.GetPrefabType(gameObject) == PrefabType.PrefabInstance;
		}

		/// <summary>
		/// Returns true if the speicified gameObject is a prefab
		/// </summary>
		public static bool IsPrefab(GameObject gameObject)
		{
			return PrefabUtility.GetPrefabType(gameObject) == PrefabType.Prefab;
		}

		/// <summary>
		/// Connects the specified prefab instance to the prefab (equivalent of pressing apply from the prefab instance)
		/// </summary>
		public static void UpdatePrefab(GameObject prefabInstance)
		{
			PrefabUtility.ReplacePrefab(
				prefabInstance,
				PrefabUtility.GetPrefabParent(prefabInstance),
				ReplacePrefabOptions.ConnectToPrefab);
		}
	}
}
