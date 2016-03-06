using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Helpers;
using UnityObject = UnityEngine.Object;

namespace Vexe.Editor.Helpers
{
    using Editor = UnityEditor.Editor;

	/// <summary>
	/// A bunch of misc editor helper methods
	/// </summary>
	public static class EditorHelper
	{
		/// <summary>
		/// Returns the specified animator's variables names
		/// </summary>
		public static string[] GetAnimatorVariableNames(Animator animator)
		{
            var controller  = animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
            var parameters = controller.parameters;
            return parameters.Select(x => x.name).ToArray();
		}

		/// <summary>
		/// Gets all the input axis defined in the project's Input manager
		/// Credits: http://www.plyoung.com/blog/manipulating-input-manager-in-script.html
		/// </summary>
		public static List<string> GetInputAxes()
		{
			var axes = new List<string>();

			var serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
			var axesProperty = serializedObject.FindProperty("m_Axes");

			axesProperty.Next(true);
			axesProperty.Next(true);
			while (axesProperty.Next(false))
			{
				SerializedProperty axis = axesProperty.Copy();
				axis.Next(true);
				axes.Add(axis.stringValue);
			}

			return axes;
		}

		/// <summary>
		/// Marks the target as dirty for the editor to save it
		/// </summary>
		public static void SetDirty(UnityObject target)
		{
			EditorUtility.SetDirty(target);
		}

		/// <summary>
        /// Returns all the tags used in the project
        /// </summary>
        public static string[] GetTags()
        {
            return InternalEditorUtility.tags;
        }

        /// <summary>
        /// Returns all the layers used in the project
        /// </summary>
        public static string[] GetLayers()
        {
            return InternalEditorUtility.layers;
        }

        /// <summary>
		/// Returns true if the specified Object is a scene object
		/// </summary>
		public static bool IsSceneObject(UnityObject obj)
		{
			return !EditorUtility.IsPersistent(obj); // or return !AssetDatabase.Contains(obj);
		}

        /// <summary>
        /// Repaints all inspectors. This is what essentially ends up getting called in EditorWindow.Repaint
        /// </summary>
        public static void RepaintAllInspectors()
        {
            var wnd = GetCachedInspectorWindowType();
            var repaint = wnd.GetMethod("RepaintAllInspectors", BindingFlags.Static | BindingFlags.NonPublic);
            repaint.Invoke(null, null);
        }

        private static Type _cachedInspWndType;
        public static Type GetCachedInspectorWindowType()
        {
            if (_cachedInspWndType == null)
                _cachedInspWndType = typeof(Editor).Assembly.GetType("UnityEditor.InspectorWindow");
            return _cachedInspWndType;
        }

        /// <summary>
        /// Returns the full path of the "ScriptableAssets" directory
        /// </summary>
        public static string GetScriptableAssetsPath()
        {
            return DirectoryHelper.GetDirectoryPath("ScriptableAssets");
        } 

		/// <summary>
		/// Focuses on the specified window, and returns a reference to it
		/// </summary>
		public static EditorWindow GetFocusedWindow(string window)
		{
			FocusOnWindow(window);
			return EditorWindow.focusedWindow;
		}

		/// <summary>
		/// Focuses on the specified window. Ex "Hierarchy", "Inspector" - pretty much any window under the "Window" tab
		/// </summary>
		public static void FocusOnWindow(string window)
		{
			EditorApplication.ExecuteMenuItem("Window/" + window);
		}

		/// <summary>
		/// Creates a ScriptableObject asset at the specified path. ".asset" is postfixed if it doesn't exist
		/// </summary>
		public static T CreateScriptableObjectAsset<T>(string path) where T : ScriptableObject
		{
			T so = ScriptableObject.CreateInstance<T>();
			if (Path.GetExtension(path) != ".asset")
				path += ".asset";
			AssetDatabase.CreateAsset(so, path);
			return so;
		}

		/// <summary>
		/// Loads and returns the asset type specified by the passed generic argument at the specified type
		/// </summary>
		public static T LoadAssetAt<T>(string path) where T : UnityObject
		{
			return AssetDatabase.LoadAssetAtPath(path, typeof(T)) as T;
		}

		/// <summary>
		/// Lazy-Loads and returns the asset type specified by the passed generic argument at the specified type
		/// That is, if 'value' was not null it gets returned, otherwise we assign it an asset of type T at 'path'
		/// if 'value' is still null, we create an asset of type T at path and return it
		/// </summary>
		public static T LazyLoadScriptableAsset<T>(ref T value, string path) where T : ScriptableObject
		{
			return LazyLoadScriptableAsset(ref value, path, false);
		}

		/// <summary>
		/// Lazy-Loads and returns the asset type specified by the passed generic argument at the specified type
		/// That is, if 'value' was not null it gets returned, otherwise we assign it an asset of type T at 'path'
		/// if 'value' is still null, we create an asset of type T at path and return it.
		/// Pass in true to 'log' to get feedback if no asset of type T was found.
		/// </summary>
		public static T LazyLoadScriptableAsset<T>(ref T value, string path, bool log) where T : ScriptableObject
		{
			if (value == null)
			{
				path = path.NormalizePath();
				value = LoadAssetAt<T>(path);
				if (value == null)
				{
					if (log) Debug.Log("No asset of type `" + typeof(T) + "` was found - creating new");
					value = CreateScriptableObjectAsset<T>(path);
				}
			}
			return value;
		}

		/// <summary>
		/// Pings the specified object
		/// </summary>
		public static void PingObject(UnityObject obj)
		{
			EditorGUIUtility.PingObject(obj);
		}

		/// <summary>
		/// Selects the specified objects
		/// </summary>
		public static void SelectObjects(UnityObject[] obj)
		{
			Selection.objects = obj;
		}

		/// <summary>
		/// Selects the specified object
		/// </summary>
		public static void SelectObject(UnityObject obj)
		{
			Selection.activeObject = obj;
		}

		/// <summary>
		/// Creates a new inspector window instance and locks it to inspect the specified target
		/// </summary>
		public static void InspectTarget(GameObject target)
		{
			// Get a ref to an InspectorWindow type object
			var inspectorType = GetCachedInspectorWindowType();

			// Create an InspectorWindow instance
			var inspectorInstance = ScriptableObject.CreateInstance(inspectorType) as EditorWindow;

			// We display it - currently, it will inspect whatever gameObject is currently selected
			// So we need to find a way to let it inspect/aim at our target GO that we passed
			// For that we do a simple trick:
			// 1- Cache the current selected gameObject
			// 2- Set the current selection to our target GO (so now all inspectors are targeting it)
			// 3- Lock our created inspector to that target
			// 4- Fallback to our previous selection
			inspectorInstance.Show();

			// Cache previous selected gameObject
			var prevSelection = Selection.activeGameObject;

			// Set the selection to GO we want to inspect
			Selection.activeGameObject = target;

			// Get a ref to the "locked" property, which will lock the state of the inspector to the current inspected target
			var isLocked = inspectorType.GetProperty("isLocked", BindingFlags.Instance | BindingFlags.Public);

			// Invoke pass 'true' to `isLocked` setter method
			isLocked.GetSetMethod().Invoke(inspectorInstance, new object[] { true });

			// Finally revert back to the previous selection so that other inspectors continue to inspect whatever they were inspecting...
			Selection.activeGameObject = prevSelection;
		}
	}
}