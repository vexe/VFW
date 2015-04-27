using System;
using Vexe.Editor.Windows;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Types;

namespace Vexe.Editor.Drawers
{
	public class SelectEnumDrawer : CompositeDrawer<Enum, SelectEnumAttribute>
	{
		public override void OnRightGUI()
		{
			if (gui.SelectionButton())
			{
				string[] names = Enum.GetNames(memberType);
				int currentIndex = memberValue == null ? -1 : names.IndexOf(memberValue.ToString());
				SelectionWindow.Show(new Tab<string>(
					@getValues: () => names,
					@getCurrent: () => memberValue.ToString(),
					@setTarget: name =>
					{
						if (names[currentIndex] != name)
							memberValue = name.ParseEnum(memberType);
					},
					@getValueName: name => name,
					@title: memberTypeName + "s"
				));
			}
		}
	}
}