using UnityEngine;
using Vexe.Editor.Helpers;
using Vexe.Runtime.Types;
using UnityObject = UnityEngine.Object;

namespace Vexe.Editor.Drawers
{
	public class DraggableDrawer : CompositeDrawer<UnityObject, DraggableAttribute>
	{
		public override void OnMemberDrawn(Rect rect)
		{
			gui.RegisterFieldForDrag(rect, memberValue);
		}
	}
}