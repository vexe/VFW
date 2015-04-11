using UnityEditor;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Types;

namespace Vexe.Editor.Types
{
	public class Foldouts
	{
		private readonly BetterPrefs prefs;

		public Foldouts(BetterPrefs prefs)
		{
			this.prefs = prefs;
		}

		public bool this[int key]
		{
			get { return prefs.Bools.ValueOrDefault(key); }
			set
			{
				prefs.Bools[key] = value;
				EditorUtility.SetDirty(prefs);
			}
		}
	}
}