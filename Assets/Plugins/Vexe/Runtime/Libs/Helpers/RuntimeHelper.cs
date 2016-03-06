using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Types;
using UnityObject = UnityEngine.Object;

namespace Vexe.Runtime.Helpers
{
    public static class RuntimeHelper
    {
        private static GameObject _emptyGO;

        /// <summary>
        /// Returns a cached reference to an empty GO (think NullObject)
        /// If none is found, a new one is created
        /// </summary>
        public static GameObject EmptyGO
        {
            get
            {
                if (_emptyGO == null)
                {
                    string na = "__Empty";
                    _emptyGO = GameObject.Find(na) ?? CreateGo(na, null, HideFlags.HideInHierarchy);
                }
                return _emptyGO;
            }
        }

        /// <summary>
        /// Creates and returns a GameObject with the passed name, parent and HideFlags
        /// </summary>
        public static GameObject CreateGo(string name, Transform parent = null, HideFlags flags = HideFlags.None)
        {
            var go = new GameObject(name);
            go.hideFlags = flags;
            if (parent)
            {
                go.transform.parent = parent;
                go.transform.Reset();
            }
            return go;
        }

        /// <summary>
        /// Creates a GameObject with a MonoBehaviour specified by the generic T arg - returns the MB added
        /// </summary>
        public static T CreateGoWithMb<T>(string name, out GameObject createdGo, Transform parent = null, HideFlags flags = HideFlags.None) where T : MonoBehaviour
        {
            createdGo = CreateGo(name, parent, flags);
            var comp = createdGo.AddComponent<T>();
            return comp;
        }

        /// <summary>
        /// Creates a GameObject with a MonoBehaviour specified by the generic T arg - returns the MB added
        /// </summary>
        public static T CreateGoWithMb<T>(string name, Transform parent = null, HideFlags flags = HideFlags.None) where T : MonoBehaviour
        {
            var go = new GameObject();
            return CreateGoWithMb<T>(name, out go, parent, flags);
        }

        public static string GetCallStack()
        {
            return GetCallStack(false);
        }

        public static string GetCallStack(bool verbose)
        {
            if (verbose)
            {
                return string.Join(" -> \r\n", new StackTrace()
                             .GetFrames()
                             .Select(x =>
                             {
                                 var method = x.GetMethod();
                                 return string.Format("{0}.{1}",
                                     method.DeclaringType.GetNiceName(),
                                     x.GetMethod().GetNiceName());
                             })
                             .ToArray());
            }

            return string.Join(" -> ", new StackTrace()
                         .GetFrames()
                         .Select(x => x.GetMethod().Name)
                         .ToArray());
        }

        public static void Swap<T>(ref T value0, ref T value1)
        {
            T tmp = value0;
            value0 = value1;
            value1 = tmp;
        }

        public static string GetCurrentSceneName()
        {
            string sceneName;

#if UNITY_EDITOR
            sceneName = Application.isPlaying ?
                SceneManager.GetActiveScene().name :
                System.IO.Path.GetFileNameWithoutExtension(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name);
            if (string.IsNullOrEmpty(sceneName))
            {
                sceneName = "Unnamed";
            }
#else
            sceneName = SceneManager.GetActiveScene().name;
#endif

            return sceneName;
        }

        public static int GetTargetID(object target)
        {
            var beh = target as BaseBehaviour;
            if (beh != null) return beh.GetPersistentId();

            var obj = target as BaseScriptableObject;
            if (obj != null) return obj.GetPersistentId();

            return RuntimeHelpers.GetHashCode(target);
        }

        public static int CombineHashCodes<Item0, Item1>(Item0 item0, Item1 item1)
        {
            int hash = 17;
            hash = hash * 31 + item0.GetHashCode();
            hash = hash * 31 + item1.GetHashCode();
            return hash;
        }

        public static int CombineHashCodes<Item0, Item1, Item2>(Item0 item0, Item1 item1, Item2 item2)
        {
            int hash = 17;
            hash = hash * 31 + item0.GetHashCode();
            hash = hash * 31 + item1.GetHashCode();
            hash = hash * 31 + item2.GetHashCode();
            return hash;
        }

        public static void Measure(string msg, int nTimes, Action<string> log, Action code)
        {
            log(string.Format("Time took to `{0}` is `{1}` ms", msg, Measure(nTimes, code)));
        }

        public static float Measure(int nTimes, Action code)
        {
            var w = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < nTimes; i++)
                code();
            return w.ElapsedMilliseconds;
        }

        public static Dictionary<T, U> CreateDictionary<T, U>(IEnumerable<T> keys, IList<U> values)
        {
            return keys.Select((k, i) => new { k, v = values[i] }).ToDictionary(x => x.k, x => x.v);
        }

        public static Texture2D GetTexture(byte r, byte g, byte b, byte a, HideFlags flags)
        {
            var texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, new Color32(r, g, b, a));
            texture.Apply();
            texture.hideFlags = flags;
            return texture;
        }

        public static Texture2D GetTexture(Color32 c, HideFlags flags)
        {
            return GetTexture(c.r, c.g, c.b, c.a, flags);
        }

        public static Texture2D GetTexture(Color32 c)
        {
            return GetTexture(c, HideFlags.None);
        }

        public static Color HexToColor(string hex)
        {
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            return new Color32(r, g, b, 255);
        }

        public static string ColorToHex(Color32 color)
        {
            string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
            return hex;
        }
    }
}
