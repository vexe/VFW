using System;
using UnityEngine;

namespace Vexe.Editor.GUIs
{
	public abstract partial class BaseGUI
	{
		/// <summary>
		/// Credits to Bunny83: http://answers.unity3d.com/questions/393992/custom-inspector-multi-select-enum-dropdown.html?sort=oldest
		/// </summary>
		public int BunnyMask(GUIContent content, int currentValue, int[] enumValues, string[] enumNames)
		{
			int maskVal = 0;
			for (int i = 0; i < enumValues.Length; i++)
			{
				if (enumValues[i] != 0)
				{
					if ((currentValue & enumValues[i]) == enumValues[i])
						maskVal |= 1 << i;
				}
				else if (currentValue == 0)
					maskVal |= 1 << i;
			}

			var newMaskVal = Mask(content, maskVal, enumNames);
			int changes = maskVal ^ newMaskVal;

			for (int i = 0; i < enumValues.Length; i++)
			{
				if ((changes & (1 << i)) != 0) // has this list item changed?
				{
					if ((newMaskVal & (1 << i)) != 0) // has it been set?
					{
						if (enumValues[i] == 0) // special case: if "0" is set, just set the val to 0
						{
							currentValue = 0;
							break;
						}
						else
						{
							currentValue |= enumValues[i];
						}
					}
					else // it has been reset
					{
						currentValue &= ~enumValues[i];
					}
				}
			}
			return currentValue;
		}

		public int BunnyMask(int currentValue, int[] enumValues, string[] enumNames, string text)
		{
			return BunnyMask(GetContent(text), currentValue, enumValues, enumNames);
		}

		public int BunnyMask(GUIContent content, Enum enumValue)
		{
			var enumType = enumValue.GetType();
			var enumNames = Enum.GetNames(enumType);
			var enumValues = Enum.GetValues(enumType) as int[];
			return BunnyMask(content, Convert.ToInt32(enumValue), enumValues, enumNames);
		}

		public int BunnyMask(string content, Enum enumValue)
		{
			return BunnyMask(GetContent(content), enumValue);
		}

		public int Mask(int mask, string[] displayedOptions, Layout option)
		{
			return Mask(string.Empty, mask, displayedOptions, option);
		}

		public int Mask(string content, int mask, string[] displayedOptions)
		{
			return Mask(GetContent(content), mask, displayedOptions);
		}

		public int Mask(GUIContent content, int mask, string[] displayedOptions)
		{
			return Mask(content, mask, displayedOptions, (Layout)null);
		}

		public int Mask(GUIContent content, int mask, string[] displayedOptions, Layout option)
		{
			return Mask(content, mask, displayedOptions, GUIStyles.Popup, option);
		}

		public int Mask(int mask, string[] displayedOptions, GUIStyle style, Layout option)
		{
			return Mask(string.Empty, mask, displayedOptions, style, option);
		}

		public int Mask(string content, int mask, string[] displayedOptions, Layout option)
		{
			return Mask(content, mask, displayedOptions, GUIStyles.Popup, option);
		}

		public int Mask(string content, int mask, string[] displayedOptions, GUIStyle style, Layout option)
		{
			return Mask(GetContent(content), mask, displayedOptions, style, option);
		}

		public abstract int Mask(GUIContent content, int mask, string[] displayedOptions, GUIStyle style, Layout option);
	}
}