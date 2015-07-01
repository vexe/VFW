using System;
using UnityEditor;
using UnityEngine;
using Vexe.Runtime.Types;

namespace Vexe.Editor.Editors
{
	[CustomEditor(typeof(BaseScriptableObject), true), CanEditMultipleObjects]
	public class BaseScriptableObjectEditor : BaseEditor
	{
		protected override void OnBeforeInitialized()
		{
			if (target is ScriptableObject && !(target is BaseScriptableObject))
			{
				throw new InvalidOperationException("target is a ScriptableObject but not a BaseScriptableObject. Please inherit BaseScriptableObject");
			}

			if ((target as BaseScriptableObject) == null)
			{
				Debug.LogWarning(string.Concat(new[] {
								"Casting target object to BaseScriptableObject failed! Something's wrong. ",
								"Maybe you switched back and inherited ScriptableObject instead of BaseScriptableObject ",
								"and you still had your gameObject selected? ",
								"If that's the case then the BaseScriptableObjectEditor is still there in memory ",
								"and so this could be resolved by reselcting your gameObject. ",
								"Destroying this BaseScriptableObjectEditor instance anyway..."
							}));

				DestroyImmediate(this);
			}
		}
	}
}