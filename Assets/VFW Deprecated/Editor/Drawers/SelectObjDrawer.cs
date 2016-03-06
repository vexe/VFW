using System;
using System.Linq;
using UnityEngine;
using Vexe.Editor.Helpers;
using Vexe.Editor.Windows;
using Vexe.Editor.Types;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Types;
using UnityObject = UnityEngine.Object;

namespace Vexe.Editor.Drawers
{
	public class SelectObjDrawer : AttributeDrawer<UnityObject, SelectObjAttribute>
	{
		public override void OnGUI()
		{
			using (gui.Horizontal())
			{
				bool isNull = member.IsNull();
				bool isObjectField = prefs[id];

				gui.Text(displayText, isNull ? "null" : memberValue.name + " (" + memberTypeName + ")");

				var fieldRect = gui.LastRect;
				{
					GUIHelper.PingField(fieldRect, memberValue, !isNull && isObjectField);
				}

				if (gui.SelectionButton("object"))
				{
					Func<UnityObject[], string, Tab> newTab = (values, title) =>
							new Tab<UnityObject>(
								@getValues    : () => values,
								@getCurrent   : member.As<UnityObject>,
								@setTarget    : member.Set,
								@getValueName : obj => obj.name,
								@title        : title
							);

					bool isGo =  memberType == typeof(GameObject);
					SelectionWindow.Show("Select a " + memberTypeName,
						newTab(UnityObject.FindObjectsOfType(memberType), "All"),
						newTab(isGo ?  (UnityObject[])gameObject.GetChildren() :
							gameObject.GetComponentsInChildren(memberType), "Children"),
						newTab(isGo ?  (UnityObject[])gameObject.GetParents() :
							gameObject.GetComponentsInParent(memberType), "Parents"),
						newTab(isGo ? PrefabHelper.GetGameObjectPrefabs().ToArray() :
								PrefabHelper.GetComponentPrefabs(memberType).Cast<UnityObject>().ToArray(), "Prefabs")
					);
				}
			}
			
		}
	}
}