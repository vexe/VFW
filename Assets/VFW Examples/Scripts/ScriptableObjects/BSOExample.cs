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
			UnityEditor.AssetDatabase.CreateAsset(ex, "Assets/Plugins/Vexe/Runtime/Examples/ExampleAssets/BSO.asset");
		}
	}
#endif

	public class BSOExample : BetterScriptableObject
	{
		[Serialize, Tags, PerItem]
		private string[] tags;

		[SelectEnum]
		public KeyCode Jump { get; set; }

		[BetterV3]
		public Vector3 Target { get; set; }

		[Show]
		private void method()
		{
			Debug.Log("method");
		}

		[Inline]
		public Object obj;
	}
}