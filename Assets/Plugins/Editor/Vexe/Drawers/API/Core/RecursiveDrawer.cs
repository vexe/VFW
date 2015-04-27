using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Vexe.Editor.GUIs;
using Vexe.Editor.Helpers;
using Vexe.Editor.Types;
using Vexe.Editor.Visibility;
using Vexe.Editor.Windows;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Helpers;
using Vexe.Runtime.Types;
using UnityObject = UnityEngine.Object;

namespace Vexe.Editor.Drawers
{
    public class RecursiveDrawer : ObjectDrawer<object>
    {
        private bool _isToStringImpl;
        private Type _polymorphicType;
        private string _nullString;

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
        }

        public override void OnGUI()
        {
            if (!DrawField())
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
                var drawer = MemberDrawersHandler.GetCachedObjectDrawer.Invoke(_polymorphicType);
                var drawerType = drawer.GetType();
                if (drawerType == typeof(RecursiveDrawer))
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
        /// if memberNames was null or empty, draws members in 'obj' recursively. Members are fetched according to the default serializaiton logic
        /// otherwise, draws only the specified members by memberNames
        /// </summary>
        public static bool DrawRecursive(object target, BaseGUI gui, int id, UnityObject unityTarget, params string[] memberNames)
        {
            return DrawRecursive(ref target, gui, id, unityTarget, memberNames);
        }

        public static bool DrawRecursive(ref object target, BaseGUI gui, int id, UnityObject unityTarget, params string[] memberNames)
        {
            List<MemberInfo> members;
            var targetType = target.GetType();
            if (memberNames.IsNullOrEmpty())
            {
                members = VFWVisibilityLogic.CachedGetVisibleMembers(targetType);
            }
            else
            {
                members = new List<MemberInfo>();
                for (int i = 0; i < memberNames.Length; i++)
                {
                    var name = memberNames[i];
                    var member = ReflectionHelper.GetCachedMember(targetType, name);
                    if (member == null)
                    {
                        LogFormat("RecursiveDrawer: Couldn't find member {0} in {1}", name, targetType.Name);
                        continue;
                    }
                    if (VFWVisibilityLogic.IsVisibleMember(member))
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
                    EditorMember em;
                    changed |= gui.Member(member, target, unityTarget, id, false, out em);
                    target = em.RawTarget;
                }
            }

            return changed;
        }

        private bool DrawField()
        {
            using (gui.Horizontal())
            {
                var isEmpty = string.IsNullOrEmpty(displayText);
                var display = isEmpty ? string.Empty : displayText + " " + (foldout ? "^" : ">");
                var value   = member.Value;
                string field;

                if (value == null)
                    field = _nullString;
                else
                    field = _isToStringImpl ? value.ToString() : value.GetType().GetNiceName();

                if (isEmpty)
                    Foldout();

                gui.Text(display, field);

                var fieldRect = gui.LastRect;
                gui.Cursor(fieldRect, MouseCursor.Link);
                if (!isEmpty && fieldRect.Contains(Event.current.mousePosition) && EventsHelper.IsLMBMouseDown())
                    foldout = !foldout;

                var drop = gui.RegisterFieldForDrop<UnityObject>(fieldRect, objs => objs.Select(x =>
                {
                    if (x == null)
                        return null;

                    var go = x as GameObject;
                    if (go != null)
                        return go.GetComponent(memberType);

                    return x.GetType().IsA(memberType) ? x : null;

                }).FirstOrDefault());

                if (drop != null)
                {
                    value = memberValue = drop;
                    GUI.changed = true;
                }

                SelectionButton();
            }

            return foldout;
        }

        protected virtual void SelectionButton()
        {
            var tabs = new List<Tab>();

            Action<Func<Type[]>, Action<Type>, string> newTypeTab = (getValues, create, title) =>
                tabs.Add(new Tab<Type>(
                    @getValues: getValues,
                    @getCurrent: () => { var x = memberValue; return x == null ? null : x.GetType(); },
                    @setTarget: newType => { if (newType == null) memberValue = memberType.GetDefaultValueEmptyIfString(); else create(newType); },
                    @getValueName: type => type.Name,
                    @title: title
                ));

            if (memberType.IsInterface)
            {
                Action<Func<UnityObject[]>, string> newUnityTab = (getValues, title) =>
                    tabs.Add(new Tab<UnityObject>(
                        @getValues: getValues,
                        @getCurrent: member.As<UnityObject>,
                        @setTarget: member.Set,
                        @getValueName: obj => obj.name + " (" + obj.GetType().Name + ")",
                        @title: title
                    ));

                newUnityTab(() => UnityObject.FindObjectsOfType<UnityObject>()
                                             .OfType(memberType)
                                             .ToArray(), "Scene");

                newUnityTab(() => PrefabHelper.GetComponentPrefabs(memberType)
                                              .ToArray(), "Prefabs");

                newTypeTab(() => GetAllUserTypesOf(memberType)
                                    .Where(t => t.IsA<MonoBehaviour>())
                                    .Where(t => !t.IsAbstract)
                                    .ToArray(), TryCreateInstanceInGO, "MonoBehaviours");
            }

            newTypeTab(() => GetAllUserTypesOf(memberType)
                                .Disinclude(memberType.IsAbstract ? memberType : null)
                                .Where(t => !t.IsA<UnityObject>() && !t.IsAbstract)
                                .ToArray(), TryCreateInstance, "System types");

            var click = Event.current.button;
            if (gui.SelectionButton("Left click: select type. Right click: try instantiate"))
            {
                if (click == 0)
                {
                    SelectionWindow.Show("Select a `" + memberTypeName + "` object", tabs.ToArray());
                }
                else if (click == 1)
                {
                    try
                    {
                        memberValue = memberType.ActivatorInstance();
                    }
                    catch(Exception e)
                    {
                        Debug.Log("Error. Couldn't create instance: " + e.Message);
                    }
                }
            }
        }

        public static Type[] GetAllUserTypesOf(Type wantedType)
        {
            return wantedType.Assembly.GetTypes().Where(t => wantedType.IsAssignableFrom(t)).ToArray();
        }

        private void TryCreateInstanceInGO(Type newType)
        {
            TryCreateInstance(() => new GameObject("(new) " + newType.Name).AddComponent(newType));
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

                EditorHelper.RepaintAllInspectors();
            }
            catch (TargetInvocationException e)
            {
                Debug.LogError(string.Format("Couldn't create instance of type {0}. Make sure the type has an empty constructor. Message: {1}, Stacktrace: {2}", memberTypeName, e.Message, e.StackTrace));
            }
        }
    }
}
