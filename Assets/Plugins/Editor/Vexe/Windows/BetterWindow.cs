using UnityEditor;
using UnityEngine;
using Vexe.Editor.GUIs;

namespace Vexe.Editor.Windows
{
	public abstract class BetterWindow : EditorWindow
	{
		public RabbitGUI gui = new RabbitGUI();

		public void OnGUI()
		{
			var start = new Rect(5f, 5f, EditorGUIUtility.currentViewWidth - 5f, 0f);
			using (gui.Begin(start))
				OnWindowGUI();
		}

		protected abstract void OnWindowGUI();
	}
}