using System;
using UnityEngine;
using Vexe.Editor.Helpers;
using UnityObject = UnityEngine.Object;

namespace Vexe.Editor.GUIs
{
	public abstract partial class BaseGUI
	{
		public UnityObject DraggableObject<T>(T value) where T : UnityObject
		{
			return DraggableObject(string.Empty, value);
		}

		public UnityObject DraggableObject<T>(string label, T value) where T : UnityObject
		{
			return DraggableObject(label, string.Empty, value);
		}

		public UnityObject DraggableObject<T>(string label, string tooltip, T value) where T : UnityEngine.Object
		{
			value = Object(label, tooltip, value, true) as T;
			RegisterFieldForDrag(LastRect, value);
			return value;
		}

		public T Object<T>(T value) where T : UnityObject
		{
			return Object(string.Empty, value);
		}

		public T Object<T>(string label, T value) where T : UnityObject
		{
			return Object(label, string.Empty, value);
		}

		public T Object<T>(string label, string tooltip, T value) where T : UnityObject
		{
			return Object(label, tooltip, value, true);
		}

		public T Object<T>(string label, T value, bool allowSceneObjects) where T : UnityObject
		{
			return Object(label, string.Empty, value, allowSceneObjects);
		}

		public T Object<T>(string label, string tooltip, T value, bool allowSceneObjects) where T : UnityObject
		{
			return Object(GetContent(label, tooltip), value, typeof(T), allowSceneObjects, null) as T;
		}

		public UnityObject Object(UnityObject value)
		{
			return Object(string.Empty, value);
		}

		public UnityObject Object(UnityObject value, Type type)
		{
			return Object(string.Empty, value, type);
		}

		public UnityObject Object(string label, UnityObject value)
		{
			return Object(label, value, typeof(UnityObject));
		}

		public UnityObject Object(string label, UnityObject value, Type type)
		{
			return Object(label, value, type, null);
		}

        public UnityObject Object(UnityObject value, Layout option)
        {
            return Object(string.Empty, value, option);
        }

        public UnityObject Object(string label, UnityObject value, Layout option)
        {
            return Object(label, value, typeof(UnityObject), option);
        }

		public UnityObject Object(string label, UnityObject value, Type type, Layout option)
		{
			return Object(label, value, type, true, option);
		}

		public UnityObject Object(string label, UnityObject value, Type type, bool allowSceneObjects)
		{
			return Object(label, value, type, allowSceneObjects, null);
		}

		public UnityObject Object(string label, UnityObject value, Type type, bool allowSceneObjects, Layout option)
		{
			return Object(label, string.Empty, value, type, allowSceneObjects, option);
		}

		public UnityObject Object(string label, string tooltip, UnityObject value, Type type, Layout option)
		{
			return Object(GetContent(label, tooltip), value, type, true, option);
		}

		public UnityObject Object(string label, string tooltip, UnityObject value, Type type, bool allowSceneObjects, Layout option)
		{
			return Object(GetContent(label, tooltip), value, type, allowSceneObjects, option);
		}

		public abstract UnityObject Object(GUIContent content, UnityObject value, Type type, bool allowSceneObjects, Layout option);
	}
}
