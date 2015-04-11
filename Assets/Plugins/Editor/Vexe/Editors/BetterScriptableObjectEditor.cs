using System;
using UnityEditor;
using UnityEngine;
using Vexe.Runtime.Types;

namespace Vexe.Editor.Editors
{
	[CustomEditor(typeof(BetterScriptableObject), true), CanEditMultipleObjects]
	public class BetterScriptableObjectEditor : BaseEditor
	{
		protected override void OnBeforeInitialized()
		{
			if (target is ScriptableObject && !(target is BetterScriptableObject))
			{
				throw new InvalidOperationException("target is a ScriptableObject but not a BetterScriptableObject. Please inherit BetterScriptableObject");
			}

			if ((target as BetterScriptableObject) == null)
			{
				Debug.LogWarning(string.Concat(new[] {
								"Casting target object to BetterScriptableObject failed! Something's wrong. ",
								"Maybe you switched back and inherited ScriptableObject instead of BetterScriptableObject ",
								"and you still had your gameObject selected? ",
								"If that's the case then the BetterScriptableObjectEditor is still there in memory ",
								"and so this could be resolved by reselcting your gameObject. ",
								"Destroying this BetterScriptableObjectEditor instance anyway..."
							}));

				DestroyImmediate(this);
			}
		}
	}
}