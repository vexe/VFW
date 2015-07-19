using UnityEditor;
using Vexe.Runtime.Types;

namespace Vexe.Editor.Drawers
{
	public class CommentDrawer : CompositeDrawer<object, CommentAttribute>
	{
        private string buttonLabel = "?";
        private bool showHelp;

        public override void OnRightGUI()
        {
            if (attribute.helpButton)
                showHelp = gui.ToggleButton(showHelp, buttonLabel, buttonLabel, "Help", MiniButtonStyle.Right);
        }

		public override void OnUpperGUI()
		{
            if (!attribute.helpButton)
                gui.HelpBox(attribute.comment, (MessageType)attribute.type);
		}

        public override void OnLowerGUI()
        {
            if (attribute.helpButton)
            {
                if (showHelp)
                    gui.HelpBox(attribute.comment, (MessageType)attribute.type);
            }
        }
	}
}