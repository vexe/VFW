using UnityEditor;
using Vexe.Runtime.Types;

namespace Vexe.Editor.Drawers
{
	public class CommentAttributeDrawer : CompositeDrawer<object, CommentAttribute>
	{
		public override void OnUpperGUI()
		{
			gui.HelpBox(attribute.comment, (MessageType)attribute.type);
		}
	}
}