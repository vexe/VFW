using UnityEngine;
using Vexe.Runtime.Types;

namespace VFWExamples
{
#if UNITY_EDITOR
	public static class BSOMenuItem
	{
		[UnityEditor.MenuItem("Tools/Vexe/Examples/Create BSO Asset")]
		public static void CreateAsset()
		{
			var ex = ScriptableObject.CreateInstance<BSOExample>();
			UnityEditor.AssetDatabase.CreateAsset(ex, "Assets/BSO.asset");
		}
	}
#endif

	public class BSOExample : BaseScriptableObject
	{
		[Tags, PerItem]
		public string[] tags;

		[SelectEnum]
		public KeyCode Jump;

		[BetterV3]
		public Vector3 Target;

		[Show]
		void method()
		{
			Debug.Log("method");
		}

		[Inline]
		public Object obj;
	}
}