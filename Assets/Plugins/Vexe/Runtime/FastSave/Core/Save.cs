//#define PROFILE
//#define DBG

using System;
using System.Collections.Generic;
using System.IO;
using BX20Serializer;
using UnityEngine;
using Vexe.FastSave.Serializers;
using Vexe.Runtime.Extensions;
using UnityObject = UnityEngine.Object;

namespace Vexe.FastSave
{
    public static class Save
    {
        public static event Action<Dictionary<int, string>> OnBeganSavingMarked;

        public static event Action<Dictionary<int, string>> OnFinishedSavingMarked;

        // Stream
        #region
        public static bool MarkedToStream(Stream stream)
        {
#if DBG
            string status = "Began saving";
#endif
            try
            {
                var markers = UnityObject.FindObjectsOfType<FSMarker>();
                var data = new Dictionary<int, string>(markers.Length);

                if (OnBeganSavingMarked != null)
                    OnBeganSavingMarked(data);

                using (var memory = new MemoryStream())
                {
                    for (int i = 0; i < markers.Length; i++)
                    {
                        var marker = markers[i];
#if DBG
                        status = "Saving " + marker.name;
#endif
                        Save.GameObjectToStream(memory, marker.gameObject, marker);

                        string serializedState = memory.ToArray().GetString();
                        int id = marker.GetPersistentId();
                        data[id] = serializedState;

                        memory.Reset();
                    }
                }
#if DBG
                status = "Saving to stream (" + stream.GetType() + ")";
#endif
                Save.ObjectToStream(stream, data);

                if (OnFinishedSavingMarked != null)
                    OnFinishedSavingMarked(data);

                return true;
            }
            catch (Exception e)
            {
#if DBG
                Log(string.Format("Error ({0}): {1}", status, e.Message));
#else
                Log("Error saving: " + e.Message);
#endif
                return false;
            }
        }

        public static void GameObjectToStream(Stream stream, GameObject value, FSMarker marker)
        {
            GameObjectSerializer.Write(stream, value);

            var marked = marker.ComponentsToSave;
            stream.WriteInt(marked.Count);
            for (int i = 0; i < marked.Count; i++)
            {
                var type = marked[i];
                var component = marker.GetComponent(type);
                TypeSerializer.Write(stream, component.GetType());
                ComponentToStream(stream, type, component);
            }
        }

        public static void GameObjectToStream(Stream stream, GameObject value)
        {
            var marker = value.GetComponent<FSMarker>();
            if (marker != null)
            {
                GameObjectToStream(stream, value, marker);
            }
            else
            {
                GameObjectSerializer.Write(stream, value);

                var components = value.GetAllComponents();
                stream.WriteInt(components.Length);
                for (int i = 0; i < components.Length; i++)
                {
                    var component = components[i];
                    var type = component.GetType();
                    TypeSerializer.Write(stream, type);
                    ComponentToStream(stream, type, component);
                }
            }
        }

        public static void HierarchyToStream(Stream stream, GameObject root)
        {
            var children = root.GetComponentsInChildren<Transform>();
            stream.WriteInt(children.Length);
            GameObjectToStream(stream, root);
            RecurseHierarchy(stream, root, children, 0);
        }

        private static void RecurseHierarchy(Stream stream, GameObject root, Transform[] children, int rootDepth)
        {
            var rootTransform = root.transform;
            for (int i = 0; i < children.Length; i++)
            {
                var child = children[i];
                if (child.parent == rootTransform)
                {
                    var go = child.gameObject;
                    var currentDepth = rootDepth + 1;
                    Log(go + ": " + currentDepth);
                    stream.WriteInt(currentDepth);
                    GameObjectToStream(stream, go);
                    RecurseHierarchy(stream, go, go.GetComponentsInChildren<Transform>(), currentDepth);
                }
            }
        }

        public static void ComponentToStream(Stream stream, Component value)
        {
            ComponentToStream(stream, value.GetType(), value);
        }

        public static void ComponentToStream(Stream stream, Type type, Component value)
        {
            FSCommon.Serializer.Serialize(stream, type, value);
        }

        public static void ObjectToStream<T>(Stream stream, T value)
        {
            FSCommon.Serializer.Serialize(stream, value.GetType(), value);
        }
        #endregion

        // File
        #region
        public static bool MarkedToFile(string path)
        {
            using(var file = File.OpenWrite(path))
                return MarkedToStream(file);
        }

        public static void ComponentToFile(string path, Component value)
        {
            using (var file = File.OpenWrite(path))
                ComponentToStream(file, value);
        }

        public static void GameObjectToFile(string path, GameObject value)
        {
            using (var file = File.OpenWrite(path))
                GameObjectToStream(file, value);
        }

        public static void HierarchyToFile(string path, GameObject root)
        {
            using (var file = File.OpenWrite(path))
                HierarchyToStream(file, root);
        }
        #endregion

        // Memory
        #region
        public static byte[] ComponentToMemory(Component value)
        {
            using (var ms = new MemoryStream())
            {
                ComponentToStream(ms, value);
                return ms.ToArray();
            }
        }

        public static byte[] HierarchyToMemory(GameObject root)
        {
            using (var ms = new MemoryStream())
            {
                HierarchyToStream(ms, root);
                return ms.ToArray();
            }
        }

        public static byte[] GameObjectToMemory(GameObject value)
        {
            using (var ms = new MemoryStream())
            {
                GameObjectToStream(ms, value);
                return ms.ToArray();
            }
        }

        public static byte[] ObjectToMemory<T>(T value)
        {
            using (var ms = new MemoryStream())
            {
                ObjectToStream(ms, value);
                return ms.ToArray();
            }
        }

        public static byte[] MarkedToMemory()
        {
            bool success;
            return MarkedToMemory(out success);
        }

        public static byte[] MarkedToMemory(out bool success)
        {
            using(var memory = new MemoryStream())
            { 
                success = MarkedToStream(memory);
                return memory.ToArray();
            }
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
