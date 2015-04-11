using Vexe.Runtime.Types;

namespace Vexe.Editor.Drawers
{
	public class IntSliderAttributeDrawer : AttributeDrawer<int, IntSliderAttribute>
	{
		public override void OnGUI()
		{
			memberValue = gui.IntSlider(niceName, memberValue, attribute.left, attribute.right);
		}
	}

	public class FloatSliderAttributeDrawer : AttributeDrawer<float, FloatSliderAttribute>
	{
		public override void OnGUI()
		{
			memberValue = gui.FloatSlider(niceName, memberValue, attribute.left, attribute.right);
		}
	}
}