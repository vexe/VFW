using UnityEditor;
using UnityEngine;
using Vexe.Editor.GUIs;
using Vexe.Runtime.Types;

namespace Vexe.Editor.Drawers
{
	public class ParagraphDrawer : AttributeDrawer<string, ParagraphAttribute>
	{
		private Vector2 _scrollPos;
		private float _prevHeight;

		static GUIContent content = new GUIContent();

		protected override void Initialize()
		{
			if (memberValue == null)
				memberValue = string.Empty;
		}

		public override void OnGUI()
        {
            memberValue = OnGUI(displayText, memberValue, attribute.minNumLines, attribute.maxNumLines, gui as RabbitGUI);
        }

		public string OnGUI(string label, string text, int minLines, int maxLines, RabbitGUI gui)
		{
            if (!string.IsNullOrEmpty(label))
			    gui.Label(label);

			content.text = text;
			float height = GUIStyles.TextArea.CalcHeight(content, gui.Width);
			int numLines = (int)(height / 13f);
			numLines = Mathf.Clamp(numLines, minLines, maxLines);
			height = 20 + ((numLines - 1) * 13f);

			if (_prevHeight != height)
			{
				_prevHeight = height;
				gui.RequestResetIfRabbit();
			}

			var layout = Layout.sHeight(height);
			var result = gui.ScrollableTextArea(text, ref _scrollPos, layout);
            return result;
		}
	}
}