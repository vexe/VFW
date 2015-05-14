using UnityEditor;
using UnityEngine;
using Vexe.Editor.GUIs;

namespace Vexe.Editor.Windows
{
	public abstract class BetterWindow : EditorWindow
	{
		public RabbitGUI gui;

        protected virtual int id
        {
            get { return GetType().GetHashCode(); }
        }

        protected virtual void OnEnable()
        {
            if (gui == null)
            { 
                gui = new RabbitGUI();
                gui.OnRepaint = Repaint;
            }
        }

		public void OnGUI()
		{
			var start = new Rect(5f, 5f, EditorGUIUtility.currentViewWidth - 5f, 0f);
			using (gui.Begin(start))
				OnWindowGUI();
		}

		protected abstract void OnWindowGUI();
	}
}