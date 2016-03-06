using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using Vexe.Editor.GUIs;
using Vexe.Runtime.Types;

namespace Vexe.Editor.Windows
{
    public abstract class BetterWindow : EditorWindow
    {
        public RabbitGUI gui;
        public EditorRecord prefs;

        protected virtual int id
        {
            get { return GetType().GetHashCode(); }
        }

        protected virtual void OnDisable()
        {
            SavePrefs();
        }

        protected virtual void OnEnable()
        {
            LoadPrefs();

            if (gui == null)
            {
                gui = new RabbitGUI(prefs);
                gui.OnRepaint = Repaint;
            }
        }

        public void LoadPrefs()
        {
            if (prefs == null)
            {
                prefs = new EditorRecord();
            }
            else
            {
                foreach (var x in prefs)
                {
                    switch (x.Value.Type)
                    {
                        case RecordValueType.Int:
                        prefs[x.Key] = EditorPrefs.GetInt(x.Key.ToString(), 0);
                        break;

                        case RecordValueType.Float:
                        prefs[x.Key] = EditorPrefs.GetFloat(x.Key.ToString(), 0.0f);
                        break;

                        case RecordValueType.String:
                        prefs[x.Key] = EditorPrefs.GetString(x.Key.ToString(), string.Empty);
                        break;

                        case RecordValueType.Bool:
                        prefs[x.Key] = EditorPrefs.GetBool(x.Key.ToString(), false);
                        break;
                    }
                }
            }
        }

        public void SavePrefs()
        {
            Assert.IsNotNull(prefs);

            foreach (var x in prefs)
            {
                switch (x.Value.Type)
                {
                    case RecordValueType.Int:
                    EditorPrefs.SetInt(x.Key.ToString(), prefs[x.Key]);
                    break;

                    case RecordValueType.Float:
                    EditorPrefs.SetFloat(x.Key.ToString(), prefs[x.Key]);
                    break;

                    case RecordValueType.String:
                    EditorPrefs.SetString(x.Key.ToString(), prefs[x.Key]);
                    break;

                    case RecordValueType.Bool:
                    EditorPrefs.SetBool(x.Key.ToString(), prefs[x.Key]);
                    break;
                }
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