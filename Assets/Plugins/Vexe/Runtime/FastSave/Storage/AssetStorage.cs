//#define DBG

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Vexe.Runtime.Types;
using UnityObject = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
using System.Linq;
#endif

namespace Vexe.FastSave
{
    public class AssetStorage : BaseScriptableObject
    {
        [Display(Dict.HorizontalPairs)]
        public AssetLookup Assets = new AssetLookup();

        [Hide] public static readonly List<Type> SupportedTypes = new List<Type>()
        {
            typeof(Mesh),
            typeof(AudioClip),
            typeof(Material),
            typeof(PhysicMaterial),
            typeof(PhysicsMaterial2D),
            typeof(Flare),
            typeof(GUIStyle),
            typeof(Texture),
            typeof(RuntimeAnimatorController),
            typeof(AnimationClip),
            typeof(UnityObject), // for text assets etc
        };

        static AssetStorage _Current;
        public static AssetStorage Current
        {
            get
            {
                if (_Current == null)
                    _Current = GetStore();
                return _Current;
            }
        }

        public string Store(UnityObject asset)
        {
            var name = asset.name;
            Assets[name] = asset;
            return name;
        }

        public UnityObject Get(string key)
        {
            UnityObject result;
            if (!Assets.TryGetValue(key, out result))
                return null;
            return result;
        }

#if UNITY_EDITOR
        [Show] void FilterNulls()
        {
            var cleaned = new AssetLookup();
            var iter = Assets.GetEnumerator();
            while(iter.MoveNext())
            {
                var current = iter.Current;

                var value = current.Value;
                if (value == null)
                    return;

                cleaned.Add(current.Key, value);
            }
            Assets = cleaned;
        }
#endif

        static AssetStorage GetStore()
        {
            AssetStorage store = null;
#if UNITY_EDITOR && !UNITY_WEBPLAYER
            var storeName = typeof(AssetStorage).Name + ".asset";
            var directory = Directory.GetDirectories("Assets", "FastSave", SearchOption.AllDirectories).FirstOrDefault();
            if (directory == null)
                Debug.LogError("Couldn't find FastSave directory!");
            else
            {
                var storePath = directory + "/Resources/" + storeName;
                store = AssetDatabase.LoadAssetAtPath<AssetStorage>(storePath);
                if (store == null)
                {
                    store = ScriptableObject.CreateInstance<AssetStorage>();
                    AssetDatabase.CreateAsset(store, storePath);
                    AssetDatabase.ImportAsset(storePath, ImportAssetOptions.ForceUpdate);
                    AssetDatabase.Refresh();
                }
            }
#endif
            if (store == null)
                store = Resources.Load<AssetStorage>(typeof(AssetStorage).Name);
            return store;
        }
    }

    [Serializable]
    public class AssetLookup : SerializableDictionary<string, UnityObject> { }
}
