using UnityEditor;
using Vexe.Runtime.Types;

namespace Vexe.Editor.Drawers
{
	public class CommentDrawer : CompositeDrawer<object, CommentAttribute>
	{
		public override void OnUpperGUI()
		{
			gui.HelpBox(attribute.comment, (MessageType)attribute.type);
		}
	}
}