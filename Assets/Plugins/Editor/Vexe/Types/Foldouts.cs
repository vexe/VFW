using UnityEditor;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Types;

namespace Vexe.Editor.Types
{
	public class Foldouts
	{
		public bool this[int key]
		{
			get
            {
                var prefs = BetterPrefs.GetEditorInstance();
                return prefs.Bools.ValueOrDefault(key);
            }
			set
			{
                var prefs = BetterPrefs.GetEditorInstance();
				prefs.Bools[key] = value;
				EditorUtility.SetDirty(prefs);
			}
		}
	}
}
