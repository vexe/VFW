using UnityEngine;

namespace Vexe.Editor.GUIs
{
	public abstract partial class BaseGUI
	{
		public AnimationCurve Curve(AnimationCurve value)
		{
			return Curve(string.Empty, value);
		}
		
		public AnimationCurve Curve(string label, AnimationCurve value)
		{
			return Curve(label, value, null);
		}
		
		public AnimationCurve Curve(string label, string tooltip, AnimationCurve value)
		{
			return Curve(label, tooltip, value, null);
		}
		
		public AnimationCurve Curve(string label, AnimationCurve value, Layout option)
		{
			return Curve(label, string.Empty, value, option);
		}
		
		public AnimationCurve Curve(string label, string tooltip, AnimationCurve value, Layout option)
		{
			return Curve(GetContent(label, tooltip), value, option);
		}
		
		public abstract AnimationCurve Curve(GUIContent content, AnimationCurve value, Layout option);
	}
}