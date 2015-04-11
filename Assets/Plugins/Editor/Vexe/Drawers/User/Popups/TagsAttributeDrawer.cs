using Vexe.Runtime.Types;

namespace Vexe.Editor.Drawers
{
	public class TagsAttributeDrawer : AttributeDrawer<string, TagsAttribute>
	{
		public override void OnGUI()
		{
			memberValue = gui.Tag(niceName, memberValue);
		}
	}
}