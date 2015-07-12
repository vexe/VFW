using System;
using System.Collections.Generic;
using UnityEngine;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Types;

#if UNITY_EDITOR
using UnityEditor;
using System.Linq;
#endif

namespace Vexe.FastSave
{
    /// <summary>
    /// Attach this script to gameObjects that you want to save.
    /// You can then explictly specify which components within a gameObject.
    /// By default all components are saved.
    /// You can use the methods visible in the inspector to mark/unmark children,
    /// or apply the current saved components to children.
    /// You can also press Alt-Shift-M to quickly add a marker to a selection of gameObjects (Alt-Shift-u) to remove
    /// </summary>
    [ExecuteInEditMode, DisallowMultipleComponent, DefineCategory("Editor", Order = 1)]
    public class FSMarker : BaseBehaviour, ISerializationCallbackReceiver
    {
        [Display(Seq.PerItemRemove | Seq.UniqueItems),
         PerItem("ShowType"), ShowType(typeof(Component), FromThisGo = true)]
        public List<Type> ComponentsToSave
        {
            set { _TypeList = value; }
            get
            {
                if (_TypeList == null)
                {
                    _TypeList = new List<Type>();
#if UNITY_EDITOR
                    MarkAllComponentsForSave();
#endif
                }
                return _TypeList;
            }
        }

        [SerializeField, Hide] List<string> _SerializedList = new List<string>();
        [NonSerialized] List<Type> _TypeList;

        public void OnBeforeSerialize()
        {
           _SerializedList.Clear();
           for (int i = 0; i < ComponentsToSave.Count; )
           {
               var type = ComponentsToSave[i];
               if (type == null)
               {
                   ComponentsToSave.RemoveAt(i);
                   continue;
               }
               _SerializedList.Add(type.AssemblyQualifiedName);
               i++;
           }
        }

        public void OnAfterDeserialize()
        {
            _TypeList = new List<Type>();
            for (int i = 0; i < _SerializedList.Count; i++)
            {
                string serialized = _SerializedList[i];
                Type type = Type.GetType(serialized);
                _TypeList.Add(type);
            }
        }

        void OnEnable()
        {
            // Since we're executing in Edit mode, we want to
            // make sure our id is assigned so we do a ping
            GetPersistentId();
        }

#if UNITY_EDITOR
        const string kMarkSelectionKey = "&#m";    // Alt-Shift-m
        const string kUnmarkSelectionKey = "&#u";  // Alt-Shift-u

        /// <summary>
        /// Marks all the components on this gameObject to be saved
        /// </summary>
        [Show("Editor")] void MarkAllComponentsForSave()
        {
            ComponentsToSave.Clear();
            ComponentsToSave.AddRange(gameObject.GetAllComponents()
                                                .Where(x => x != null)
                                                .Select(x => x.GetType()));
        }

        /// <summary>
        /// Recursively adds a marker to children
        /// </summary>
        [Show("Editor")] void MarkChildren()
        {
            var children = gameObject.GetComponentsInChildren<Transform>();
            for (int i = 0; i < children.Length; i++)
                children[i].gameObject.GetOrAddComponent<FSMarker>();
        }

        /// <summary>
        /// Recurisvely removes marker from children
        /// </summary>
        [Show("Editor")] void UnmarkChildren()
        {
            var children = gameObject.GetComponentsInChildren<Transform>();
            for (int i = 0; i < children.Length; i++)
            {
                var child = children[i];
                if (child == transform) continue;
                var marker = child.GetComponent<FSMarker>();
                if (marker != null)
                    marker.Destroy();
            }
        }

        /// <summary>
        /// Recurisvely removes marker from this gameObject and children
        /// </summary>
        [Show("Editor")] void UnmarkThisAndChildren()
        {
            UnmarkChildren();
            EditorApplication.delayCall += this.Destroy;
        }

        /// <summary>
        /// Recurisvely adds a marker to this gameObject's children
        /// applying the same ComponentToSave types that are already defined in this marker
        /// </summary>
        [Show("Editor")] void ApplyToChildren()
        {
            var children = gameObject.GetComponentsInChildren<Transform>();
            for (int i = 0; i < children.Length; i++)
            {
                var child = children[i];
                if (child == transform) continue;
                var marker = child.gameObject.GetOrAddComponent<FSMarker>();
                marker.ComponentsToSave.Clear();
                for (int j = 0; j < ComponentsToSave.Count; j++)
                {
                    var toSave = ComponentsToSave[j];
                    if (child.HasComponent(toSave))
                    {
                        marker.ComponentsToSave.Add(toSave);
                    }
                }
            }
        }

        [MenuItem("Tools/Vexe/FastSave/MarkSelection " + kMarkSelectionKey)]
        public static void MarkSelection()
        {
            var selection = Selection.gameObjects;

            bool markChildren = selection.Any(x => x.transform.childCount > 0) &&
                                EditorUtility.DisplayDialog("Mark children", "Do you want to mark the selected object(s) children as well?", "Yes", "No");

            foreach (var go in selection)
            {
                go.GetOrAddComponent<FSMarker>();
                if (markChildren)
                {
                    var children = go.GetComponentsInChildren<Transform>();
                    for (int i = 0; i < children.Length; i++)
                        children[i].GetOrAddComponent<FSMarker>();
                }
            }
        }

        [MenuItem("Tools/Vexe/FastSave/UnmarkSelection " + kUnmarkSelectionKey)]
        public static void UnmarkSelection()
        {
            var selection = Selection.gameObjects;
            bool unmarkChildren = selection.Any(x => x.transform.childCount > 0) &&
                                  EditorUtility.DisplayDialog("Unmark children", "Do you want to unmark the selected object(s) children as well?", "Yes", "No");

            foreach (var go in selection)
            {
                var marker = go.GetComponent<FSMarker>();
                if (marker != null)
                    marker.Destroy();

                if (unmarkChildren)
                {
                    var children = go.GetComponentsInChildren<FSMarker>();
                    for (int i = 0; i < children.Length; i++)
                        children[i].Destroy();
                }
            }
        }

        static FSMarker()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnMarkerGUI;
        }

        static void OnMarkerGUI(int id, Rect rect)
        {
            var go = EditorUtility.InstanceIDToObject(id) as GameObject;
            if (go == null)
                return;

            var marker = go.GetComponent<FSMarker>();
            if (marker == null)
                return;

            var color = Color.green;
            color.a *= 0.2f;

            rect.x = 0f;
            rect.width = EditorGUIUtility.currentViewWidth;

            var prev = GUI.contentColor;
            GUI.color = color;
            GUI.Box(rect, GUIContent.none);
            GUI.color = prev;
        }
#endif
    }
}
