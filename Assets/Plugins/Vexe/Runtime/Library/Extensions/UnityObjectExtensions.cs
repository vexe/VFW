using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Vexe.Runtime.Extensions
{
    public static class UnityObjectExtensions
    {
        public static T InstantiateNew<T>(this T source, Vector3 pos, Quaternion rot) where T : UnityObject 
        {
            return UnityObject.Instantiate(source, pos, rot) as T;
        }

        public static T InstantiateNew<T>(this T source, Vector3 pos) where T : UnityObject 
        {
            return UnityObject.Instantiate(source, pos, Quaternion.identity) as T;
        }

        public static T InstantiateNew<T>(this T source) where T : UnityObject 
        {
            return UnityObject.Instantiate(source, Vector3.zero, Quaternion.identity) as T;
        }

        /// <summary>
        /// Calls Destroy on this object if we're in playmode, otherwise (edit-time) DestroyImmediate
        /// </summary>
        public static void Destroy(this UnityObject obj, bool allowDestroyingAssets)
        {
            if (obj == null) return;

            if (Application.isPlaying)
            {
                if (obj is GameObject)
                {
                    GameObject gameObject = obj as GameObject;
                    gameObject.transform.SetParent(null, false);
                }
                UnityObject.Destroy(obj);
            }
            else
            {
                UnityObject.DestroyImmediate(obj, allowDestroyingAssets);
            }
        }

        public static void Destroy(this UnityObject obj)
        {
            Destroy(obj, false);
        }
    }
}