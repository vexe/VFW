using System;
using UnityEditor;
using UnityEngine;
using Vexe.Runtime.Types;

namespace Vexe.Editor.Editors
{
	[CustomEditor(typeof(BaseBehaviour), true), CanEditMultipleObjects]
	public class BaseBehaviourEditor : BaseEditor
	{
		protected override void OnBeforeInitialized()
		{
			if (target is MonoBehaviour && !(target is BaseBehaviour))
			{
				throw new InvalidOperationException("target is a MonoBehaviour but not a BaseBehaviour. Please inherit BaseBehaviour");
			}

			if ((target as BaseBehaviour) == null)
			{
				Debug.LogWarning(string.Concat(new[] {
								"Casting target object to BaseBehaviour failed! Something's wrong. ",
								"Maybe you switched back and inherited MonoBehaviour instead of BaseBehaviour ",
								"and you still had your gameObject selected? ",
								"If that's the case then the BaseBehaviourEditor is still there in memory ",
								"and so this could be resolved by reselcting your gameObject. ",
								"Destroying this BaseBehaviourEditor instance anyway..."
							}));

				DestroyImmediate(this);
			}
		}
	}
}