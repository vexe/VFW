using System;
using Vexe.Runtime.Types;

namespace Vexe.Editor.Drawers
{
	public class EnumMaskDrawer : AttributeDrawer<Enum, EnumMaskAttribute>
	{
		public override void OnGUI()
		{
			var currentValue = memberValue;
			var newMask = gui.BunnyMask(displayText, currentValue);
			{
				var newValue = Enum.ToObject(memberType, newMask) as Enum;
				if (!Equals(newValue, currentValue))
				{
					memberValue = newValue;
				}
			}
		}
	}
}