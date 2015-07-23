using UnityEngine;

namespace Vexe.Editor.GUIs
{
	public abstract partial class BaseGUI
	{
		public float FloatSlider(float value, float leftValue, float rightValue)
		{
			return FloatSlider(value, leftValue, rightValue, null);
		}

		public float FloatSlider(float value, float leftValue, float rightValue, Layout option)
		{
			return FloatSlider(string.Empty, value, leftValue, rightValue, option);
		}

		public float FloatSlider(string content, float value, float leftValue, float rightValue)
		{
			return FloatSlider(content, value, leftValue, rightValue, null);
		}

		public float FloatSlider(string content, float value, float leftValue, float rightValue, Layout option)
		{
			return FloatSlider(new GUIContent(content), value, leftValue, rightValue, option);
		}

		public abstract float FloatSlider(GUIContent content, float value, float leftValue, float rightValue, Layout option);

		public int IntSlider(int value, int leftValue, int rightValue)
		{
			return IntSlider(value, leftValue, rightValue, null);
		}

		public int IntSlider(int value, int leftValue, int rightValue, Layout option)
		{
			return IntSlider(string.Empty, value, leftValue, rightValue, option);
		}

		public int IntSlider(string content, int value, int leftValue, int rightValue)
		{
			return IntSlider(content, value, leftValue, rightValue, null);
		}

		public int IntSlider(string content, int value, int leftValue, int rightValue, Layout option)
		{
			return IntSlider(new GUIContent(content), value, leftValue, rightValue, option);
		}

		public int IntSlider(GUIContent content, int value, int leftValue, int rightValue, Layout option)
		{
			return Mathf.RoundToInt(FloatSlider(content, (float)value, (float)leftValue, (float)rightValue, option));
		}

        public void MinMaxSlider(ref float minValue, ref float maxValue, float minLimit, float maxLimit)
        {
            MinMaxSlider(string.Empty, ref minValue, ref maxValue, minLimit, maxLimit);
        }

        public void MinMaxSlider(string label, ref float minValue, ref float maxValue, float minLimit, float maxLimit)
        {
            MinMaxSlider(GetContent(label), ref minValue, ref maxValue, minLimit, maxLimit, Layout.None);
        }

        public abstract void MinMaxSlider(GUIContent content, ref float minValue, ref float maxValue, float minLimit, float maxLimit, Layout option);
	}
}
