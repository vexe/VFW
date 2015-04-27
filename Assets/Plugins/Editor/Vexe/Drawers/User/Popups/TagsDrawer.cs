using Vexe.Runtime.Types;

namespace Vexe.Editor.Drawers
{
	public class TagsDrawer : AttributeDrawer<string, TagsAttribute>
	{
		public override void OnGUI()
		{
			memberValue = gui.Tag(displayText, memberValue);
		}
	}
}