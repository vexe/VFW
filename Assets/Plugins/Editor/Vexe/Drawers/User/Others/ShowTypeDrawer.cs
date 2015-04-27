using System;
using System.Linq;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Helpers;
using Vexe.Runtime.Types;
using Vexe.Editor.Types;

namespace Vexe.Editor.Drawers
{
	public class ShowTypeDrawer : AttributeDrawer<Type, ShowTypeAttribute>
	{
		private Type[] availableTypes;
		private string[] typesNames;
		private int index;

		protected override void Initialize()
		{
			if (attribute.FromThisGo)
			{
				availableTypes = gameObject.GetAllComponents()
										   .Select(x => x.GetType())
										   .Where(x => x.IsA(attribute.baseType))
										   .ToArray();
			}
			else
			{
				availableTypes = ReflectionHelper.GetAllTypesOf(attribute.baseType)
												 .Where(t => !t.IsAbstract)
												 .ToArray();
			}

			typesNames = availableTypes.Select(t => t.Name)
									   .ToArray();
		}

		public override void OnGUI()
		{
			index = member.IsNull() ? -1 : availableTypes.IndexOf(memberValue);
			var selection = gui.Popup(displayText, index, typesNames);
			{
				if (index != selection)
				{
					memberValue = availableTypes[selection];
					index = selection;
				}
			}
		}
	}
}