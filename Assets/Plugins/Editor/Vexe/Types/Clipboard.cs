using UnityEngine;
using Vexe.Editor.Helpers;
using Vexe.Runtime.Types;

namespace Vexe.Editor.Types
{
	public class Clipboard : BetterScriptableObject
	{
		private static string instancePath;

		static Clipboard()
		{
			instancePath = EditorHelper.GetScriptableAssetsPath() + "/Clipboard.asset";
		}

		private static Clipboard _instance;
		public static Clipboard GetInstance()
		{
			return EditorHelper.LazyLoadScriptableAsset<Clipboard>(ref _instance, instancePath, true);
		}

		public Vector3 V3Value;
		public Vector2 V2Value;
		public int IntValue;
		public float FloatValue;
		public string StringValue;
		public bool BoolValue;
		public Color ColorValue;
		public Quaternion QuaternionValue;
	}
}