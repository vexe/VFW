using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Vexe.Runtime.Types
{
    /// <summary>
    /// Intended to be a better replacement for PlayerPrefs/EditorPrefs
    /// Better because it gives more storage options (*), offers more flexibility (**) and faster (***)
    /// (*) There's more data types you could store other than int/float/string.
    /// You could also subclass and add support for arrays/lists of those types.
    /// (**) You could serialize the prefs data to stream, say file or memory.
    /// (***) It uses dictionaries so it's faster than writing to registry
    /// 
    /// NOTE: You could use the extension method (in Vexe.Runtime.Extensions) ValueOrDefault on the dictionaries
    /// if you're not sure whether or not there's a value registered with a certain key.
    /// There's also an overload that lets you specify the default value to use.
    /// </summary>
    public class BetterPrefs : BetterScriptableObject
    {
        public Dictionary<int, int>     Ints     = new Dictionary<int, int>();
        public Dictionary<int, string>  Strings  = new Dictionary<int, string>();
        public Dictionary<int, float>   Floats   = new Dictionary<int, float>();
        public Dictionary<int, bool>    Bools    = new Dictionary<int, bool>();
        public Dictionary<int, Vector3> Vector3s = new Dictionary<int, Vector3>();
        public Dictionary<int, Color>   Colors   = new Dictionary<int, Color>();

        [Show]
        void Clear()
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

        static BetterPrefs editorInstance;
        public static BetterPrefs GetEditorInstance()
        {
            if ( editorInstance == null )
            {
                editorInstance = AssetDatabase.LoadAssetAtPath(EditorPrefsPath, typeof(BetterPrefs)) as BetterPrefs;
                if ( editorInstance == null )
                {
                    editorInstance = ScriptableObject.CreateInstance<BetterPrefs>();
                    AssetDatabase.CreateAsset(editorInstance, EditorPrefsPath);
                    AssetDatabase.ImportAsset(EditorPrefsPath, ImportAssetOptions.ForceUpdate);
                }
            }

            AssignIfNull(ref editorInstance.Ints);
            AssignIfNull(ref editorInstance.Strings);
            AssignIfNull(ref editorInstance.Floats);
            AssignIfNull(ref editorInstance.Bools);
            AssignIfNull(ref editorInstance.Colors);
            AssignIfNull(ref editorInstance.Vector3s);

            return editorInstance;
        }

        static void AssignIfNull<T>(ref Dictionary<int, T> value)
        {
            if (value == null)
                value = new Dictionary<int, T>();
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