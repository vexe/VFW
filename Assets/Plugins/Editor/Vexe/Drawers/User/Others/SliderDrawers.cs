using UnityEngine;
using Vexe.Runtime.Types;

namespace Vexe.Editor.Drawers
{
	public class iSliderDrawer : AttributeDrawer<int, iSliderAttribute>
	{
		public override void OnGUI()
		{
			memberValue = gui.IntSlider(displayText, memberValue, attribute.left, attribute.right);
		}
	}

	public class fSliderDrawer : AttributeDrawer<float, fSliderAttribute>
	{
		public override void OnGUI()
		{
			memberValue = gui.FloatSlider(displayText, memberValue, attribute.left, attribute.right);
		}
	}

    public class vSliderDrawer : AttributeDrawer<Vector2, vSliderAttribute>
    {
        public override void OnGUI()
        {
            using(gui.Horizontal())
            {
                gui.Prefix(displayText);
                float x = gui.Float(memberValue.x);
			    float y = memberValue.y;
                gui.MinMaxSlider(ref x, ref y, attribute.left, attribute.right);
                y = gui.Float(y);
                memberValue = new Vector2(x, y);
            }
        }
    }
}
