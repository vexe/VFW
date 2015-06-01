//#define DBG

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Types;
using UnityObject = UnityEngine.Object;

#pragma warning disable 0168

namespace Vexe.Editor.Drawers
{
    using System.Reflection;
    using Vexe.Editor.GUIs;
    using Vexe.Runtime.Serialization;
    using Editor = UnityEditor.Editor;

    public class InlineDrawer : CompositeDrawer<UnityObject, InlineAttribute>
    {
        private List<bool> expandValues;

        public bool GuiBox;

        // Add the types you want to support here...
        private Dictionary<Type, Action<UnityObject, BaseGUI>> customEditors;
        
        public InlineDrawer ()
        {
            customEditors = new Dictionary<Type, Action<UnityObject, BaseGUI>>
            {
                {
                    typeof(Transform), (target, gui) =>
                    {
                        RecursiveDrawer.DrawRecursive(target, gui, id, target,
                            "localPosition", "localRotation", "localScale");
                    }
                },
                {
                    typeof(Rigidbody), (target, gui) =>
                    {
                        RecursiveDrawer.DrawRecursive(target, gui, id, target,
                            "mass", "drag", "angularDrag", "useGravity", "isKinematic", "interpolation", "collisionDetectionMode");

                        using (gui.Indent())
                        { 
                            var rb = target as Rigidbody;
                            rb.constraints = (RigidbodyConstraints)gui.BunnyMask("Constraints", rb.constraints);
                        }
                    }
                },
                {
                    typeof(BoxCollider), (target, gui) =>
                    {
                        RecursiveDrawer.DrawRecursive(target, gui, id, target,
                            "isTrigger", "sharedMaterial", "center", "size");
                    }
                },
                {
                    typeof(SphereCollider), (target, gui) =>
                    {
                        RecursiveDrawer.DrawRecursive(target, gui, id, target,
                            "isTrigger", "sharedMaterial", "center", "radius");
                    }
                },
                {
                    typeof(Animator), (target, gui) =>
                    {
                        RecursiveDrawer.DrawRecursive(target, gui, id, target,
                            "runtimeAnimatorController", "avatar", "applyRootMotion", "updateMode", "cullingMode");
                    }
                },
                {
                    typeof(Camera), (target, gui) =>
                    {
                        RecursiveDrawer.DrawRecursive(target, gui, id, target,
                            "clearFlags", "backgroundColor", "cullingMask", "nearClipPlane", "farClipPlane",
                            "rect", "depth", "renderingPath", "targetTexture", "useOcclusionCulling", "hdr",
                            "isOrthoGraphic");

                        // TODO: improve cullingMask drawing. right now it's a float, gui.Layer doesn't cut it too

                        using (gui.Indent())
                        {
                            var c = target as Camera;
                            if (c.orthographic)
                                c.orthographicSize = gui.Float("Size", c.orthographicSize);
                            else c.fieldOfView = gui.FloatSlider("FOV", c.fieldOfView, 1, 179);
                        }
                    }
                },
                {
                    typeof(MeshRenderer), (target, gui) =>
                    {
                        RecursiveDrawer.DrawRecursive(target, gui, id, target,
                            "castShadows", "receiveShadows", "sharedMaterials", "useLightProbes");
                    }
                },
                {
                    typeof(AudioSource), (target, gui) =>
                    {
                        RecursiveDrawer.DrawRecursive(target, gui, id, target,
                            "mute", "bypassEffects", "bypassListenerEffects", "bypassReverbZones", "playOnAwake", "loop");

                        using (gui.Indent())
                        {
                            var source = target as AudioSource;
                            source.priority = gui.IntSlider("Priority", source.priority, 0, 128);
                            source.volume = gui.FloatSlider("Volume", source.volume, 0f, 1f);
                            source.pitch = gui.FloatSlider("Pitch", source.pitch, -3f, 3f);
                        }
                    }
                },
            };
        }

        static void SetVisibility(UnityObject target, bool visible)
        {
            if (visible)
            {
                target.hideFlags = HideFlags.None;
            }
            else
            {
                var c =target as Component;
                if (c != null)
                    c.hideFlags = HideFlags.HideInInspector;
                else if (target is GameObject)
                    (target as GameObject).hideFlags = HideFlags.HideInHierarchy;
            }
        }

        static bool IsHidden(UnityObject target)
        {
            if (target is GameObject)
                return target.hideFlags.HasFlag(HideFlags.HideInHierarchy);
            return target.hideFlags.HasFlag(HideFlags.HideInInspector);
        }

        protected override void Initialize()
        {
            expandValues = new List<bool>();
            GuiBox       = attribute.GuiBox;
#if DBG
            Log("Initialized InlineDrawer " + niceName);
#endif
        }

        public override void OnLeftGUI()
        {
            Foldout();
            gui.Space(-10f);
        }

        public override void OnRightGUI()
        {
            if (!attribute.HideButton)
                return;

            var value = memberValue;
            if (value == null)
                return;

            var hidden = IsHidden(value);
            if (gui.Button(hidden ? "Show" : "Hide", GUIStyles.MiniRight, Layout.sWidth(40f)))
                SetVisibility(value, hidden);
        }

        public override void OnLowerGUI()
        {
            var objRef = memberValue;
            if (foldout)
            {
                if (objRef == null)
                {
                    gui.HelpBox("Please assign a target to inline");
                    return;
                }

                var go = objRef as GameObject;
                if (go != null)
                {
                    using (gui.Horizontal())
                    {
                        go.SetActive(gui.Toggle(string.Empty, go.activeSelf, Layout.sWidth(20f)));
                        go.name = gui.Text(go.name);
                    }

                    using (gui.LabelWidth(45f))
                    using (gui.Horizontal())
                    {
                        go.tag = gui.Tag("Tag", go.tag);
                        gui.Space(5f);
                        go.layer = gui.LayerField("Layer", go.layer);
                    }

                    var comps = go.GetAllComponents();
                    for (int i = 0; i < comps.Length; i++)
                        DrawExpandableHeader(comps[i], i);
                }
                else
                {
                    using (gui.Vertical(GuiBox ? GUI.skin.box : GUIStyle.none))
                        DrawTarget(objRef);
                }
            }
        }

        private void DrawTarget(UnityObject target)
        {
            Action<UnityObject, BaseGUI> draw;
            if (customEditors.TryGetValue(target.GetType(), out draw))
                draw(target, gui);
            else
                RecursiveDrawer.DrawRecursive(target, gui, id, unityTarget);
        }

        private void DrawExpandableHeader(Component target, int index)
        {
            bool expanded = gui.InspectorTitlebar(InternalEditorUtility.GetIsInspectorExpanded(target), target);

            bool previous;
            if (index >= expandValues.Count)
            {
                expandValues.Add(expanded);
                previous = !expanded;
            }
            else
            {
                previous = expandValues[index];
            }
            if (expanded != previous)
            {
                expandValues[index] = expanded;
                InternalEditorUtility.SetIsInspectorExpanded(target, expanded);
            }
            if (expanded)
            {
                gui.Space(5f);
                using (gui.Indent())
                {
                    DrawTarget(target);
                }
            }
        }
    }
}
