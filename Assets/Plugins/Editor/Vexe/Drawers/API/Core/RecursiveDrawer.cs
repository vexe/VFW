using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Vexe.Editor.Extensions;
using Vexe.Editor.GUIs;
using Vexe.Editor.Helpers;
using Vexe.Editor.Internal;
using Vexe.Editor.Types;
using Vexe.Editor.Visibility;
using Vexe.Editor.Windows;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Helpers;
using Vexe.Runtime.Serialization;
using Vexe.Runtime.Types;
using UnityObject = UnityEngine.Object;

namespace Vexe.Editor.Drawers
{
    public class RecursiveDrawer : ObjectDrawer<object>
    {
        private bool _isToStringImpl;
        private Type _polymorphicType;
        private string _nullString;
        private Tab[] _tabs;
        private Predicate<UnityObject[]> _isDropAccepted;
        private Func<UnityObject[], UnityObject> _getDraggedObject;
        private Func<Func<Type[]>, Action<Type>, string, Tab> _newTypeTab;
        private Func<Func<UnityObject[]>, string, Tab> _newUnityTab;

        protected override void Initialize()
        {
            _nullString = string.Format("null ({0})", memberTypeName);

            if (memberValue != null)
            {
                _polymorphicType = memberValue.GetType();
                _isToStringImpl = _polymorphicType.IsMethodImplemented("ToString", Type.EmptyTypes);
            }
            else
                _isToStringImpl = memberType.IsMethodImplemented("ToString", Type.EmptyTypes);

            _getDraggedObject = objs =>
            {
                for (int i = 0; i < objs.Length; i++)
                {
                    var drag = objs[i];
                    if (drag == null)
                        continue;

                    var go = drag as GameObject;
                    if (go != null)
                    {
                        var c = go.GetComponent(memberType);
                        if (c != null)
                            return c;
                    }

                    if (drag.GetType().IsA(memberType))
                        return drag;
                }
                return null;
            };

            _isDropAccepted = objs => _getDraggedObject(objs) != null;

            _newTypeTab = (getValues, create, title) =>
                new Tab<Type>(
                    @getValues: getValues,
                    @getCurrent: () => { var x = memberValue; return x == null ? null : x.GetType(); },
                    @setTarget: newType => { if (newType == null) memberValue = memberType.GetDefaultValueEmptyIfString(); else create(newType); },
                    @getValueName: type => type.GetNiceName(),
                    @title: title
                );

            _newUnityTab = (getValues, title) =>
                new Tab<UnityObject>(
                    @getValues: getValues,
                    @getCurrent: member.As<UnityObject>,
                    @setTarget: member.Set,
                    @getValueName: obj => obj.name + " (" + obj.GetType().GetNiceName() + ")",
                    @title: title
                );

            int idx = 0;
            if (memberType.IsInterface)
            {
                _tabs = new Tab[4];

                _tabs[idx++] = _newUnityTab(() => UnityObject.FindObjectsOfType<UnityObject>()
                                              .OfType(memberType)
                                              .ToArray(), "Scene");

                _tabs[idx++] = _newUnityTab(() => PrefabHelper.GetComponentPrefabs(memberType)
                                               .ToArray(), "Prefab");

                _tabs[idx++] = _newTypeTab(() => ReflectionHelper.GetAllUserTypesOf(memberType)
                                    .Where(t => t.IsA<MonoBehaviour>() && !t.IsAbstract)
                                    .ToArray(), TryCreateInstanceInGO, "New Behaviour");
            }
            else _tabs = new Tab[1];

            var systemTypes = ReflectionHelper.GetAllUserTypesOf(memberType)
                                .Where(t => !t.IsA<UnityObject>() && !t.IsAbstract)
                                .ToArray();

            if (memberType.IsGenericType && !systemTypes.Contains(memberType))
                ArrayUtility.Add(ref systemTypes, memberType);

            _tabs[idx] = _newTypeTab(() => systemTypes, TryCreateInstance, "System Type");
        }

        public override void OnGUI()
        {
            using (gui.Horizontal())
            {
                var isEmpty  = string.IsNullOrEmpty(displayText);
                var label    = isEmpty ? string.Empty : displayText + " " + (foldout ? "^" : ">");
                var value    = member.Value;
                var unityObj = value as UnityObject;

                string field;
                if (value == null)
                    field = _nullString;
                else
                    field = (_isToStringImpl || unityObj != null) ? value.ToString() : value.GetType().GetNiceName();

                if (isEmpty)
                    Foldout();

                var e = Event.current;

                gui.Prefix(label);

                var labelRect = gui.LastRect;

                gui.Cursor(labelRect, MouseCursor.Link);
                if (!isEmpty && e.IsMouseContained(labelRect) && e.IsLMBDown())
                    foldout = !foldout;

                gui.Space(2.3f);

                if (unityObj != null)
                {
                    var icon = AssetPreview.GetMiniThumbnail(unityObj);
                    gui.Label(new GUIContent(field, icon), GUIStyles.ObjectField);
                }
                else
                    gui.Label(field, GUIStyles.ObjectField);

                var totalRect = gui.LastRect;
                var fieldRect = totalRect;
                fieldRect.width -= 15f;

                if (unityObj != null)
                {
                    gui.Cursor(fieldRect, MouseCursor.Zoom);
                    if (fieldRect.Contains(e.mousePosition))
                    {
                        if (e.IsLMBDown())
                        {
                            EditorHelper.PingObject(unityObj);
                            if (e.IsDoubleClick())
                                EditorHelper.SelectObject(unityObj);
                            e.Use();
                        }
                        else if (e.IsRMBDown())
                        {
                            var mb = unityObj as MonoBehaviour;
                            if (mb != null)
                            { 
                                var monoscript = MonoScript.FromMonoBehaviour(mb);
                                var scriptPath = AssetDatabase.GetAssetPath(monoscript);
                                UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(scriptPath, 0);
                            }
                        }
                    }
                }

                var drop = gui.RegisterFieldForDrop<UnityObject>(fieldRect, _getDraggedObject, _isDropAccepted);
                if (drop != null)
                {
                    memberValue = drop;
                    GUI.changed = true;
                }

                var thumbRect = totalRect;
                thumbRect.width -= fieldRect.width;
                thumbRect.x += fieldRect.width;

                gui.Cursor(thumbRect, MouseCursor.Link);

                // Selection/thumb button
                { 
                    if (e.IsMouseContained(thumbRect) && e.IsMouseDown())
                    {
                        if (e.IsLMB())
                        {
                            SelectionWindow.Show("Select a `" + memberTypeName + "` object", _tabs);
                        }
                        else if (e.IsRMB())
                        {
                            try
                            {
                                memberValue = memberType.ActivatorInstance();
                            }
                            catch(Exception ex)
                            {
                                Debug.Log("Error creating new instance of type `{0}`: {1}".FormatWith(memberType.GetNiceName(), ex.Message));
                            }
                        }
                    }
                }
            }

            if (!foldout)
                return;

            if (member.IsNull())
            {
                gui.HelpBox("Member value is null");
                return;
            }

            if (_polymorphicType == null || _polymorphicType == memberType)
            {
                object value = member.Value;
                DrawRecursive(ref value, gui, id, unityTarget);
                member.Value = value;
            }
            else
            {
                var drawer = MemberDrawersHandler.CachedGetObjectDrawer(_polymorphicType);
                var drawerType = drawer.GetType();
                if (drawerType == typeof(RecursiveDrawer) || drawerType == typeof(UnityObjectDrawer))
                {
                    object value = member.Value;
                    DrawRecursive(ref value, gui, id, unityTarget);
                    member.Value = value;
                }
                else
                {
                    drawer.Initialize(member, attributes, gui);
                    gui.Member(member, attributes, drawer, false);
                }
            }
        }

        /// <summary>
        /// if memberNames was null or empty, draws members in 'obj' recursively. Members are fetched according to the default visibility logic
        /// otherwise, draws only the specified members by memberNames
        /// </summary>
        public static bool DrawRecursive(object target, BaseGUI gui, int id, UnityObject unityTarget, params string[] memberNames)
        {
            return DrawRecursive(ref target, gui, id, unityTarget, memberNames);
        }

        public static bool DrawRecursive(ref object target, BaseGUI gui, int id, UnityObject unityTarget, params string[] memberNames)
        {
            RuntimeMember[] serialized = null;
            var vfwObj = target as IVFWObject;
            if (vfwObj != null)
                serialized = vfwObj.GetSerializedMembers();

            List<MemberInfo> members;
            var targetType = target.GetType();
            if (memberNames.IsNullOrEmpty())
            {
                members = VisibilityLogic.CachedGetVisibleMembers(targetType, serialized);
            }
            else
            {
                members = new List<MemberInfo>();
                for (int i = 0; i < memberNames.Length; i++)
                {
                    var name = memberNames[i];
                    var member = ReflectionHelper.CachedGetMember(targetType, name);
                    if (member == null)
                    {
                        LogFormat("RecursiveDrawer: Couldn't find member {0} in {1}", name, targetType.Name);
                        continue;
                    }
                    if (VisibilityLogic.IsVisibleMember(member, serialized))
                        members.Add(member);
                }
            }

            if (members.Count == 0)
            {
                gui.HelpBox("Object doesn't have any visible members");
                return false;
            }

            bool changed = false;
            using (gui.Indent())
            {
                for (int i = 0; i < members.Count; i++)
                {
                    MemberInfo member = members[i];

                    if (!ConditionalVisibility.IsVisible(member, target))
                        continue;

                    EditorMember em;
                    changed |= gui.Member(member, target, unityTarget, id, false, out em);
                    if (em != null)
                        target = em.RawTarget;
                }
            }

            return changed;
        }

        private void TryCreateInstanceInGO(Type newType)
        {
            TryCreateInstance(() => new GameObject("(new) " + newType.GetNiceName()).AddComponent(newType));
        }

        private void TryCreateInstance(Type newType)
        {
            TryCreateInstance(() => newType.Instance());
        }

        private void TryCreateInstance(Func<object> create)
        {
            try
            {
                var inst = create();
                member.Set(inst);

                if (memberValue != null)
                    _polymorphicType = memberValue.GetType();
            }
            catch (TargetInvocationException e)
            {
                Debug.LogError(string.Format("Couldn't create instance of type {0}. Make sure the type has an empty constructor. Message: {1}, Stacktrace: {2}", memberTypeName, e.Message, e.StackTrace));
            }
        }
    }
}
