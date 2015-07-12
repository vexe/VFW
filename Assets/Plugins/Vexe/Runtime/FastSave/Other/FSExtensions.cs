using UnityEngine;
using System.IO;

namespace Vexe.FastSave
{
    public static class FSExtensions
    {
        public static void SaveHierarchyToStream(this GameObject root, Stream stream)
        {
            Save.HierarchyToStream(stream, root);
        }

        public static byte[] SaveHierarchyToMemory(this GameObject root)
        {
            return Save.HierarchyToMemory(root);
        }

        public static void LoadHierarchyFromStream(this GameObject into, Stream stream)
        {
            Load.HierarchyFromStream(stream, into);
        }

        public static void SaveToStream(this GameObject value, Stream stream)
        {
            Save.GameObjectToStream(stream, value);
        }

        public static byte[] SaveToMemory(this GameObject value)
        {
            return Save.GameObjectToMemory(value);
        }

        public static void LoadFromMemory(this GameObject into, byte[] bytes)
        {
            Load.GameObjectFromMemory(bytes, into);
        }

        public static void LoadFromStream(this GameObject into, Stream stream)
        {
            Load.GameObjectFromStream(stream, into);
        }

        public static byte[] SaveToMemory(this Component value)
        {
            return Save.ComponentToMemory(value);
        }

        public static void LoadFromMemory(this Component into, byte[] bytes)
        {
            Load.ComponentFromMemory(bytes, into);
        }
    }
}
