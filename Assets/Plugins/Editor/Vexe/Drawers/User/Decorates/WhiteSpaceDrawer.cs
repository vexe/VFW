using Vexe.Runtime.Types;

namespace Vexe.Editor.Drawers
{
	public class WhiteSpaceDrawer : CompositeDrawer<object, WhitespaceAttribute>
	{
		public override void OnUpperGUI()
		{
			gui.Space(attribute.Top);
		}
		public override void OnLowerGUI()
		{
			gui.Space(attribute.Bottom);
		}
		public override void OnRightGUI()
		{
			gui.Space(attribute.Right);
		}
		public override void OnLeftGUI()
		{
			gui.Space(attribute.Left);
		}
	}
}