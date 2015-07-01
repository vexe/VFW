using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System;
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
    [CreateAssetMenu]
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
        const string EditorPrefsPath  = "Assets/Plugins/Editor/Vexe/ScriptableAssets/BetterEditorPrefs.asset";
        const string RuntimePrefsPath = "Assets/Plugins/Vexe/Runtime/ScriptableAssets/BetterPrefs.asset";

        static BetterPrefs instance;
        public static BetterPrefs GetEditorInstance()
        {
            if (instance == null)
            {
                instance = AssetDatabase.LoadAssetAtPath<BetterPrefs>(EditorPrefsPath);
                if (instance == null)
                {
                    instance = ScriptableObject.CreateInstance<BetterPrefs>();
                    AssetDatabase.CreateAsset(instance, EditorPrefsPath);
                    AssetDatabase.ImportAsset(EditorPrefsPath, ImportAssetOptions.ForceUpdate);
                }
            }

            if (instance.Ints == null) instance.Ints = new LookupIntInt();
            if (instance.Strings == null) instance.Strings = new LookupIntString();
            if (instance.Floats == null) instance.Floats = new LookupIntFloat();
            if (instance.Bools == null) instance.Bools = new LookupIntBool();
            if (instance.Colors == null) instance.Colors = new LookupIntColor();
            if (instance.Vector3s == null) instance.Vector3s = new LookupIntVector3();

            return instance;
        }
#endif
    }
}