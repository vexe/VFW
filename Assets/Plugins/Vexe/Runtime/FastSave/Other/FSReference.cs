using System;
using UnityEngine;
using Vexe.Runtime.Types;

namespace Vexe.FastSave
{
    [ExecuteInEditMode]
    public class FSReference : BaseBehaviour
    {
        [Display(Dict.HorizontalPairs | Dict.Readonly)]
        static readonly ReferenceLookup Lookup = new ReferenceLookup();

        void OnEnable()
        {
            Lookup[GetPersistentId()] = gameObject;
        }

        void OnDestroy()
        {
            Lookup.Remove(GetPersistentId());
        }

        public static GameObject Get(int id)
        {
            return Lookup.ValueOrDefault(id);
        }

        public static T Get<T>(int id) where T : Component
        {
            var go = Get(id);
            if (go == null)
                return null;
            return go.GetComponent<T>();
        }

        [Serializable]
        public class ReferenceLookup : SerializableDictionary<int, GameObject> { }

#if UNITY_EDITOR
        [Show] int Id { get { return GetPersistentId(); } }

        [Show] void Populate()
        {
            Lookup.Clear();
            var refs = FindObjectsOfType<FSReference>();
            for(int i = 0; i < refs.Length; i++)
                Lookup[refs[i].GetPersistentId()] = refs[i].gameObject;
        }
#endif
    }
}
