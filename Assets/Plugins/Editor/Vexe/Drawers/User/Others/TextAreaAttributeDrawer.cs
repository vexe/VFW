using UnityEditor;
using UnityEngine;
using Vexe.Editor.GUIs;
using Vexe.Runtime.Types;

namespace Vexe.Editor.Drawers
{
	public class ParagraphAttributeDrawer : AttributeDrawer<string, ParagraphAttribute>
	{
		private Vector2 _scrollPos;
		private float _prevHeight;

		static GUIContent content = new GUIContent();

		protected override void OnSingleInitialization()
		{
			if (memberValue == null)
				memberValue = string.Empty;
		}

		public override void OnGUI()
		{
			gui.Label(niceName);

			content.text = memberValue;
			float height = EditorStyles.textArea.CalcHeight(content, (gui as RabbitGUI).Width);
			int numLines = (int)(height / 13f);
			numLines = Mathf.Clamp(numLines, attribute.minNumLines, attribute.maxNumLines);
			height = 20 + ((numLines - 1) * 13f);

			if (_prevHeight != height)
			{
				_prevHeight = height;
				gui.RequestResetIfRabbit();
			}

			var layout = Layout.sHeight(height);
			memberValue = gui.ScrollableTextArea(memberValue, ref _scrollPos, layout);
		}
	}
}