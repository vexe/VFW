using System;
using UnityEngine;

namespace Vexe.Editor.GUIs
{
	public abstract partial class BaseGUI
	{
		public Enum EnumPopup(Enum selected, Layout option)
		{
			return EnumPopup(selected, GUIStyles.Popup, option);
		}

		public Enum EnumPopup(Enum selected, GUIStyle style, Layout option)
		{
			return EnumPopup(GUIContent.none, selected, style, option);
		}

		public Enum EnumPopup(GUIContent content, Enum selected, Layout option)
		{
			return EnumPopup(content, selected, GUIStyles.Popup, option);
		}

		public T EnumPopup<T>(T selected)
        {
            return EnumPopup<T>(string.Empty, selected);
        }

		public T EnumPopup<T>(string content, T selected)
		{
			return (T)(object)EnumPopup(content, (Enum)(object)selected);
		}

		public Enum EnumPopup(string content, Enum selected)
		{
			return EnumPopup(content, selected, (Layout)null);
		}

		public Enum EnumPopup(string content, Enum selected, Layout option)
		{
			return EnumPopup(content, selected, GUIStyles.Popup, option);
		}

		public Enum EnumPopup(string content, Enum selected, GUIStyle style, Layout option)
		{
			return EnumPopup(GetContent(content), selected, style, option);
		}

		public abstract Enum EnumPopup(GUIContent content, Enum selected, GUIStyle style, Layout option);
	}
}
