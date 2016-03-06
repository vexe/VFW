//#define PROFILE
//#define DBG

using System;
using System.Collections.Generic;
using System.IO;
using BX20Serializer;
using UnityEngine;
using Vexe.FastSave.Serializers;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Helpers;
using Vexe.Runtime.Types;
using UnityObject = UnityEngine.Object;
using UnityEngine.Assertions;

namespace Vexe.FastSave
{
    public static class Load
    {
        public static event Action<Dictionary<int, string>> OnBeganLoadingMarked;

        public static event Action<Dictionary<int, string>> OnFinishedLoadingMarked;

        // Stream
        #region
        public static bool MarkedFromStream(Stream stream)
        {
#if DBG
            string status = "Began";
#endif

            try
            {
                if (stream.Length == 0)
                {
#if DBG
                    Log("Save stream is empty");
#endif
                    return false;
                }

                var data = Load.ObjectFromStream<Dictionary<int, string>>(stream);
#if DBG
                status = "Loaded marked from stream";
#endif
                var markers = UnityObject.FindObjectsOfType<FSMarker>();

                if (OnBeganLoadingMarked != null)
                    OnBeganLoadingMarked(data);

                for (int i = 0; i < markers.Length; i++)
                {
                    var marked = markers[i];
                    int id = marked.GetPersistentId();
                    string serializedState;
                    if (!data.TryGetValue(id, out serializedState))
                        continue;
#if DBG
                    status = "Loading " + marked.name;
#endif
                    var bytes = serializedState.GetBytes();
                    using (var memory = new MemoryStream(bytes))
                        Load.GameObjectFromStream(memory, marked.gameObject);
                }

                if (OnFinishedLoadingMarked != null)
                    OnFinishedLoadingMarked(data);

                return true;
            }
            catch (Exception e)
            {
#if DBG
                Log(string.Format("Error ({0}) {1}", status, e.Message));
#else
                Log("Error loading: " + e.Message);
#endif
                return false;
            }
        }

        public static void GameObjectFromStream(Stream stream, GameObject into)
        {
            GameObjectSerializer.Read(stream, into);

            int numSavedComponents = stream.ReadInt();
            for (int i = 0; i < numSavedComponents; i++)
            {
                var type = TypeSerializer.Read(stream);
                var component = into.GetOrAddComponent(type);
                ComponentFromStream(stream, type, component);
            }
        }

        public static void HierarchyFromStream(Stream stream, GameObject root)
        {
            var numSavedChildren = stream.ReadInt();
            var children = root.GetComponentsInChildren<Transform>();
            var newHierarchy = numSavedChildren != children.Length;

            var list = new List<ItemTuple<GameObject, int>>(numSavedChildren);

            if (newHierarchy)
            {
                GameObjectFromStream(stream, root);
                list.Add(ItemTuple.Create(root, 0));
                for (int i = 0; i < numSavedChildren - 1; i++)
                {
                    var depth = stream.ReadInt();
                    var go = new GameObject();
                    GameObjectFromStream(stream, go);
                    list.Add(ItemTuple.Create(go, depth));
                }
            }
            else
            {
                GameObjectFromStream(stream, root);
                list.Add(ItemTuple.Create(root, 0));
                for (int i = 0; i < numSavedChildren - 1; i++)
                {
                    var depth = stream.ReadInt();
                    var go = children[i + 1].gameObject;
                    GameObjectFromStream(stream, go);
                    list.Add(ItemTuple.Create(go, depth));
                }
            }

            // traverse forward
            for (int i = 0; i < list.Count; i++)
            {
                // foreach item going forward, we traverse backwards
                // to find the first parent item
                var forward = list[i];
                for (int j = i - 1; j >= 0; j--)
                {
                    var backward = list[j];
                    if (forward.Item2 == backward.Item2 + 1)
                    {
                        //Debug.Log("parenting: " + forward.Item1 + " to: " + backward.Item1);
                        forward.Item1.transform.SetParent(backward.Item1.transform, true);
                        break;
                    }
                }
            }
        }

        public static void ComponentFromStream(Stream stream, Component into)
        {
            ComponentFromStream(stream, into.GetType(), into);
        }

        public static void ComponentFromStream(Stream stream, Type type, Component into)
        {
            FSCommon.Serializer.Deserialize(stream, type, ref into);
        }

        public static T ObjectFromStream<T>(Stream stream)
        {
            T instance = default(T);
            FSCommon.Serializer.Deserialize(stream, typeof(T), ref instance);
            return instance;
        }
        #endregion

        // File
        #region
        public static bool MarkedFromFile(string path)
        {
            using (var file = File.OpenRead(path))
                return MarkedFromStream(file);
        }

        public static void GameObjectFromFile(string path, GameObject into)
        {
            using (var file = File.OpenRead(path))
                GameObjectFromStream(file, into);
        }

        public static void ComponentFromFile(string path, Component into)
        {
            using (var file = File.OpenRead(path))
                ComponentFromStream(file, into);
        }

        public static void HierarchyFromFile(string path, GameObject root)
        {
            using (var file = File.OpenRead(path))
                HierarchyFromStream(file, root);
        }
        #endregion

        // Memory
        #region
        public static bool MarkedFromMemory(byte[] bytes)
        {
            using (var memory = new MemoryStream(bytes))
                return MarkedFromStream(memory);
        }

        public static void GameObjectFromMemory(byte[] bytes, GameObject into)
        {
            using (var ms = new MemoryStream(bytes))
                GameObjectFromStream(ms, into);
        }

        public static void HierarchyFromMemory(byte[] bytes, GameObject into)
        {
            using (var ms = new MemoryStream(bytes))
                HierarchyFromStream(ms, into);
        }

        public static void ComponentFromMemory(byte[] bytes, Component into)
        {
            using (var ms = new MemoryStream(bytes))
                ComponentFromStream(ms, into);
        }

        public static T ObjectFromMemory<T>(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
                return ObjectFromStream<T>(ms);
        }
        #endregion

        static void Log(string msg)
        {
#if DBG
            Debug.Log(msg);
#endif
        }
    }
}
