using System.Collections.Generic;
using UnityEngine;
using System;
using Vexe.Runtime.Extensions;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
using System.Linq;
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
    /// NOTE: You could use ValueOrDefault on the dictionaries
    /// if you're not sure whether or not there's a value registered with a certain key.
    /// There's also an overload that lets you specify the default value to use.
    /// </summary>
    [CreateAssetMenu(menuName = "Vexe/BetterPrefs")]
    public class BetterPrefs : BaseScriptableObject
    {
        [Serializable] public class LookupIntInt : SerializableDictionary<int, int> { }
        [Serializable] public class LookupIntString : SerializableDictionary<int, string> { }
        [Serializable] public class LookupIntFloat : SerializableDictionary<int, float> { }
        [Serializable] public class LookupIntBool : SerializableDictionary<int, bool> { }
        [Serializable] public class LookupIntVector3 : SerializableDictionary<int, Vector3> { }
        [Serializable] public class LookupIntColor : SerializableDictionary<int, Color> { }

        public LookupIntInt     Ints     = new LookupIntInt();
        public LookupIntString  Strings  = new LookupIntString();
        public LookupIntFloat   Floats   = new LookupIntFloat();
        public LookupIntBool    Bools    = new LookupIntBool();
        public LookupIntVector3 Vector3s = new LookupIntVector3();
        public LookupIntColor   Colors   = new LookupIntColor();

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
        static BetterPrefs instance;
        public static BetterPrefs GetEditorInstance()
        {
            if (instance == null) {
                // First attempt is to find the pref instance anywhere in the AssetDatabase
                var prefPath = AssetDatabase.FindAssets("t:BetterPrefs BetterEditorPrefs").Select(x => AssetDatabase.GUIDToAssetPath(x)).FirstOrDefault();
                if (prefPath != null) {
                    instance = AssetDatabase.LoadAssetAtPath<BetterPrefs>(prefPath);
                } else {
                    // Otherwise create one on the spot
                    instance = CreateInstance<BetterPrefs>();
                }
            }

            if (instance.Ints == null) instance.Ints = new LookupIntInt();
            if (instance.Strings == null) instance.Strings = new LookupIntString();
            if (instance.Floats == null) instance.Floats = new LookupIntFloat();
            if (instance.Bools == null) instance.Bools = new LookupIntBool();
            if (instance.Colors == null) instance.Colors = new LookupIntColor();
            if (instance.Vector3s == null) instance.Vector3s = new LookupIntVector3();

            // Add instance to the Asset Database if it isn't there already.
            if (AssetDatabase.Contains(instance))
                return instance;

            const string root = "Assets";
            var dirs = Directory.GetDirectories(root, "Vexe", SearchOption.AllDirectories);
            var editorDir = dirs.FirstOrDefault(x => Directory.GetParent(x).Name == "Editor") ?? string.Empty;

            var prefsDir = Path.Combine(editorDir, "ScriptableAssets");
            if (editorDir.IsNullOrEmpty() || !Directory.Exists(prefsDir)) {
                Debug.LogError("Unable to create editor prefs asset at Editor/Vexe/ScriptableAssets (couldn't find folder). Creating in project root instead...");
                prefsDir = root;
            }

            var path = Path.Combine(prefsDir, "BetterEditorPrefs.asset");
            AssetDatabase.CreateAsset(instance, path);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            AssetDatabase.Refresh();
            return instance;
        }
#endif
    }
}
