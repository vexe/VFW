using System.Collections.Generic;
using UnityEngine;
using Vexe.Runtime.Extensions;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Vexe.Runtime.Types
{
    public class BetterPrefs : BetterScriptableObject
    {
        public Dictionary<int, int>     Ints     = new Dictionary<int, int>();
        public Dictionary<int, string>  Strings  = new Dictionary<int, string>();
        public Dictionary<int, float>   Floats   = new Dictionary<int, float>();
        public Dictionary<int, bool>    Bools    = new Dictionary<int, bool>();
        public Dictionary<int, Vector3> Vector3s = new Dictionary<int, Vector3>();
        public Dictionary<int, Color>   Colors   = new Dictionary<int, Color>();

        [Show] void Clear()
        {
            Ints.Clear();
            Strings.Clear();
            Floats.Clear();
            Bools.Clear();
            Vector3s.Clear();
            Colors.Clear();
        }

#if UNITY_EDITOR
        const string EditorPrefsPath  = "Assets/Plugins/Editor/Vexe/ScriptableAssets/BetterEditorPrefs.asset";
        const string RuntimePrefsPath = "Assets/Plugins/Vexe/Runtime/ScriptableAssets/BetterPrefs.asset";

        static BetterPrefs _editorInstance;
        public static BetterPrefs EditorInstance
        {
            get
            {
                if (_editorInstance == null)
                {
                    _editorInstance = AssetDatabase.LoadAssetAtPath(EditorPrefsPath, typeof(BetterPrefs)) as BetterPrefs;
                    if (_editorInstance == null)
                    {
                        _editorInstance = ScriptableObject.CreateInstance<BetterPrefs>();
                        AssetDatabase.CreateAsset(_editorInstance, EditorPrefsPath);
                        AssetDatabase.ImportAsset(EditorPrefsPath, ImportAssetOptions.ForceUpdate);
                    }
                }
                return _editorInstance;
            }
        }

        public static class BetterPrefsMenus
        {
            [MenuItem("Tools/Vexe/BetterPrefs/CreateAsset")]
            public static void CreateBetterPrefsAsset()
            {
                AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<BetterPrefs>(), RuntimePrefsPath);
            }
        }
#endif
    }
}