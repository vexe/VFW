using System;
using UnityEditor;
using UnityEngine;
using Vexe.Editor.Helpers;
using Vexe.Editor.Windows;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Helpers;
using Vexe.Runtime.Types;
using UnityObject = UnityEngine.Object;

namespace Vexe.Editor.Drawers
{
	public class PathDrawer : CompositeDrawer<string, PathAttribute>
	{
		public override void OnMemberDrawn(Rect rect)
		{
			var drop = gui.RegisterFieldForDrop<UnityObject>(rect);
			if (drop != null)
			{
				memberValue = GetPath(drop);
			}

			var e = Event.current;
			if (e != null && rect.Contains(e.mousePosition))
			{
				if (Event.current.control && EventsHelper.IsMMBMouseDown())
				{
					SelectionWindow.Show(new Tab<GameObject>(
						@getValues   : Resources.FindObjectsOfTypeAll<GameObject>,
						@getCurrent  : () => null,
						@setTarget   : input => memberValue = GetPath(input),
						@getValueName: target => target.name,
						@title       : "Objects"
					));
				}
			}
		}

		private string GetPath(UnityObject input)
		{
			if (attribute.UseFullPath)
			{
				if (EditorHelper.IsSceneObject(input))
				{
					var comp = input as Component;
					var go = comp != null ? comp.gameObject : input as GameObject;
					if (go != null)
					{
						var parents = go.GetParents();
						if (parents.IsNullOrEmpty())
							return go.name;
						Array.Reverse(parents);
						var result = parents.JoinString("/", p => p.name) + "/" + go.name;
						return result;
					}
				}
				else
				{
					var fullPath = AssetDatabase.GetAssetPath(input);
					var result = attribute.AbsoluteAssetPath ? fullPath : fullPath.Remove(0, fullPath.IndexOf('/') + 1);
					return result;
				}
			}
			return input.name;
		}
	}
}