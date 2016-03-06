using UnityEngine;
using System.Collections.Generic;
using Vexe.Runtime.Types;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

namespace Vexe.FastSave
{
    [CreateAssetMenu(menuName = "Vexe/PrefabStorage")]
    public class PrefabStorage : BaseScriptableObject
    {
        public List<GameObject> Prefabs = new List<GameObject>();

        public GameObject Get(int id)
        {
            return Prefabs[id];
        }

        public bool IsPrefab(GameObject go)
        {
            return GetIndex(go) != -1;
        }

        public int GetIndex(GameObject prefab)
        {
            return Prefabs.IndexOf(prefab);
        }

        static PrefabStorage _Current;
        public static PrefabStorage Current
        {
            get
            {
                if (_Current == null)
                {
                    _Current = Resources.Load<PrefabStorage>(typeof(PrefabStorage).Name);
                    if (_Current == null)
                    {
                        Debug.LogError("No prefab storage was found! Please create one under Resources/Storage");
                        return null;
                    }
                }
                return _Current;
            }
        }

#if UNITY_EDITOR && !UNITY_WEBPLAYER
        [Show] void Populate()
        {
            Prefabs.Clear();
            var prefabFiles = Directory.GetFiles("Assets", "*.prefab", SearchOption.AllDirectories);
            for (int i = 0; i < prefabFiles.Length; i++)
            {
                var path = prefabFiles[i];
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                Prefabs.Add(prefab);
            }
        }
#endif
    }
}
