//#define DBG

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Vexe.Editor.Types;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Helpers;
using Vexe.Runtime.Types;
using UnityObject = UnityEngine.Object;

namespace Vexe.Editor.Drawers
{
    public class DelegateDrawer : ObjectDrawer<IBaseDelegate>
    {
        private AddingData adding;
        private EditorMember[] argMembers;
        private object[] argValues;
        private int kAdvanced, kAdd, kInvoke;
        private string kHeaderStr;

        protected override void Initialize()
        {
            adding = new AddingData();
            kAdvanced = RuntimeHelper.CombineHashCodes(id, "advanced");
            kAdd = RuntimeHelper.CombineHashCodes(id, "add");
            kInvoke = RuntimeHelper.CombineHashCodes(id, "invoke");
            kHeaderStr = string.Format("{0} ({1})", displayText, memberTypeName);

            if (memberValue == null)
                memberValue = memberType.Instance<IBaseDelegate>();

            int len = memberValue.ParamTypes.Length;
            argValues = new object[len];
            argMembers = new EditorMember[len];

            for (int iLoop = 0; iLoop < len; iLoop++)
            {
                int i = iLoop;
                var paramType = memberValue.ParamTypes[i];

                argValues[i] = paramType.GetDefaultValueEmptyIfString();

                var argMember = EditorMember.WrapGetSet(
                        @get: () => argValues[i],
                        @set: x => argValues[i] = x,
                        @rawTarget: argValues,
                        @unityTarget: unityTarget,
                        @name: string.Format("({0})", paramType.GetNiceName()),
                        @attributes: null,
                        @id: RuntimeHelper.CombineHashCodes(id, i),
                        @dataType: paramType
                    );

                argMember.RawTarget = rawTarget;
                argMembers[i] = argMember;
            }

#if DBG
            Log("Delegate drawer init. " + niceName);
#endif
        }

        public override void OnGUI()
        {
            foldout = gui.Foldout(kHeaderStr, foldout, Layout.Auto);
            if (!foldout) return;

            if (memberValue == null)
            {
#if DBG
                Log("Creating instance " + niceName);
#endif
                memberValue = memberType.Instance<IBaseDelegate>();
            }

            // read
            var handlers = memberValue.handlers;

            using (gui.Vertical(GUI.skin.box))
            {
                if (handlers.Count == 0)
                {
                    gui.HelpBox("There are no handlers for this delegate");
                    goto FOOTER;
                }

                // header
                {
                    gui.Space(1.5f);
                    using (gui.Horizontal())
                    {
                        gui.BoldLabel("Target :: Handler");
                        using (gui.State(foldout))
                            foldouts[kAdvanced] = gui.CheckButton(foldouts[kAdvanced], "advanced mode", MiniButtonStyle.Right);
                    }
                }

                gui.Splitter();

                // body
                {
                    // target : handler
                    for (int iLoop = 0; iLoop < handlers.Count; )
                    {
                        var i = iLoop;
                        var handler = handlers[i];
                        var target = handler.target;
                        var removed = false;

                        if (handler.method == null)
                        {
                            Debug.Log("One of the handlers' method was null! This shouldn't happen under normal circumstances. " +
                                      "Removing that handler from the handlers list. " +
                                      "If you keep getting this message please report it");

                            handlers.RemoveAt(i);
                            continue;
                        }

                        using (gui.Horizontal())
                        {
                            var obj = target as UnityObject;
                            if (obj != null)
                                gui.Object(obj);
                            else
                            {
                                string str;
                                if (target == null)
                                    str = "null";
                                else str = target.ToString();
                                gui.Text(str);
                            }

                            gui.Text(handler.method.Name);

                            if (foldouts[kAdvanced])
                            {
                                if (gui.MoveDownButton())
                                    handlers.MoveElementDown(i);
                                if (gui.MoveUpButton())
                                    handlers.MoveElementUp(i);
                                if (gui.RemoveButton("handler", MiniButtonStyle.ModRight))
                                {
                                    handlers.RemoveAt(i);
                                    removed = true;
                                }
                            }
                        }

                        if (!removed) iLoop++;
                    }
                }

            FOOTER:
                gui.Space(3f);
                gui.Splitter();
                {
                    // add> source :: invoke>
                    using (gui.Horizontal())
                    {
                        using (gui.State(foldouts[kAdd] && adding.source != null && adding.target != null && adding.method != null))
                        {
                            if (gui.Button("Add", "Add new handler", EditorStyles.miniButton, Layout.sWidth(40f)))
                            {
                                handlers.Add(new IBaseDelegate.Handler
                                {
                                    target = adding.target,
                                    method = adding.method
                                });
                            }
                        }

                        gui.Space(10f);

                        // add foldout
                        if (adding.source != null)
                            foldouts[kAdd] = gui.Foldout(foldouts[kAdd]);

                        // source
                        var obj = gui.Object(adding.source);
                        {
                            if (adding.source != obj)
                            {
                                adding.source = obj;
                                foldouts[kAdd] = true;
                            }
                        }

                        bool paramless = argMembers.IsEmpty();
                        using (gui.State(handlers.Count > 0 && (foldouts[kInvoke] || paramless)))
                        {
                            if (gui.Button("Invoke", EditorStyles.miniButtonRight, Layout.sWidth(50f)))
                            {
                                for (int i = 0; i < handlers.Count; i++)
                                {
                                    var handler = handlers[i];
                                    var method = handler.method;
                                    method.Invoke(handler.target, argValues);
                                }
                            }
                        }

                        if (!paramless && handlers.Count > 0)
                        {
                            gui.Space(8f);
                            foldouts[kInvoke] = gui.Foldout(foldouts[kInvoke]);
                        }
                    }


                    // adding area: target -- method
                    if (foldouts[kAdd] && adding.source != null)
                    {
                        gui.Space(5f);
                        gui.Label("Add handler:");
                        using (gui.Indent(GUI.skin.textArea))
                        {
                            // target
                            var gameObject = adding.gameObject;
                            if (gameObject == null)
                            {
                                gui.Popup("Target", 0, new string[] { adding.source.GetType().Name });
                                {
                                    adding.target = adding.source;
                                }
                            }
                            else
                            {
                                if (adding.source is Component)
                                    adding.target = adding.source;

                                var components      = gameObject.GetAllComponents();
                                int targetIdx       = components.IndexOfZeroIfNotFound(adding.target);
                                var uniqueNames     = components.Select(c => c.GetType().Name).ToList().Uniqify();
                                var targetSelection = gui.Popup("Target", targetIdx, uniqueNames.ToArray());
                                {
                                    if (targetIdx != targetSelection || adding.target as Component != components[targetSelection])
                                        adding.target = components[targetSelection];
                                }
                            }

                            // method
                            if (adding.target != null)
                            {
                                var methods = adding.target.GetType()
                                                    .GetMethods(memberValue.ReturnType, memberValue.ParamTypes, Flags.InstancePublic, false)
                                                    .Where(m => !m.IsDefined<HideAttribute>())
                                                    .Where(m => !DelegateSettings.IgnoredMethods.Contains(m.Name))
                                                    .ToList();
                                int mIndex  = methods.IndexOfZeroIfNotFound(adding.method);
                                int methodSelection = gui.Popup("Handler", mIndex, methods.Select(m => m.GetFullName()).ToArray());
                                {
                                    if (!methods.IsEmpty() && (mIndex != methodSelection || adding.method != methods[methodSelection]))
                                        adding.method = methods[methodSelection];
                                }
                            }
                        }
                    }

                    // invocation args
                    if (foldouts[kInvoke])
                    {
                        gui.Label("Invoke with arguments:");
                        using (gui.Indent())
                        {
                            for (int i = 0; i < argMembers.Length; i++)
                                gui.Member(argMembers[i], false);
                        }
                    }
                }
            }

            // write
            memberValue.handlers = handlers;
        }

        private class AddingData
        {
            public object target;
            public UnityObject source;
            public MethodInfo method;

            public GameObject gameObject
            {
                get
                {
                    var component = source as Component;
                    return component == null ? source as GameObject : component.gameObject;
                }
            }
        }
    }

    // Ugly as hell, I know...
    public class uDelegateDrawer : ObjectDrawer<uDelegate>
    {
        private AddingData adding;
        private List<object[]> argValuesList
        {
            get { return memberValue.arguments; }
            set { memberValue.arguments = value; }
        }
        private List<EditorMember[]> argMembersList;
        private int  kAdvanced, kAdd;
        private string kHeaderStr;

        protected override void Initialize()
        {
            adding = new AddingData();

            kAdvanced = RuntimeHelper.CombineHashCodes(id, "advanced");
            kAdd = RuntimeHelper.CombineHashCodes(id, "add");
            kHeaderStr = string.Format("{0} ({1})", displayText, memberTypeName);

            if (memberValue == null)
                memberValue = new uDelegate();

            if (argValuesList == null)
                argValuesList = new List<object[]>();

            argMembersList = new List<EditorMember[]>();
#if DBG
			Log("Delegate drawer init. " + niceName);
#endif
        }

        public override void OnGUI()
        {
            foldout = gui.Foldout(kHeaderStr, foldout, Layout.Auto);
            if (!foldout) return;

            if (memberValue == null)
                memberValue = new uDelegate();

            // read
            var handlers = memberValue.handlers;

            using (gui.Vertical(GUI.skin.box))
            {
                if (handlers.Count == 0)
                {
                    gui.HelpBox("There are no handlers for this delegate");
                    goto FOOTER;
                }

                // header
                {
                    gui.Space(1.5f);
                    using (gui.Horizontal())
                    {
                        gui.BoldLabel("Target :: Handler");
                        using (gui.State(foldout))
                        {
                            foldouts[kAdvanced] = gui.CheckButton(foldouts[kAdvanced], "advanced mode", MiniButtonStyle.Right);
                        }
                    }
                }

                gui.Splitter();

                // body
                {
                    // target : handler
                    for (int iLoop = 0; iLoop < handlers.Count; )
                    {
                        var i = iLoop;
                        var handler = handlers[i];
                        var target = handler.target;
                        var removed = false;
                        var key = RuntimeHelper.CombineHashCodes(id, "udel", i);

                        using (gui.Horizontal())
                        {
                            var obj = target as UnityObject;
                            if (obj != null)
                                gui.Object(obj);
                            else
                            {
                                string str;
                                if (target == null)
                                    str = "null";
                                else str = target.ToString();
                                gui.Text(str);
                            }

                            gui.Text(handler.method.Name);

                            gui.Space(10f);
                            foldouts[key] = gui.Foldout(foldouts[key]);
                            gui.Space(-5f);

                            if (foldouts[kAdvanced])
                            {
                                if (gui.MoveDownButton())
                                    handlers.MoveElementDown(i);
                                if (gui.MoveUpButton())
                                    handlers.MoveElementUp(i);
                                if (gui.RemoveButton("handler", MiniButtonStyle.ModRight))
                                {
                                    handlers.RemoveAt(i);
                                    argMembersList.RemoveAt(i);
                                    argValuesList.RemoveAt(i);
                                    removed = true;
                                }
                            }
                        }

                        if (foldouts[key] && !removed)
                        {
                            var method = handler.method;
                            Type[] paramTypes = method.GetParameters().Select(x => x.ParameterType).ToArray();

                            SetupMembers(i, paramTypes);

                            using (gui.Indent())
                            {
                                gui.Label("Invocation args");
                                using (gui.Indent())
                                {
                                    var argMembers = argMembersList[i];
                                    for (int j = 0; j < argMembers.Length; j++)
                                        gui.Member(argMembers[j], true);
                                }
                            }
                        }

                        if (!removed) iLoop++;
                    }
                }

            FOOTER:
                gui.Space(3f);
                gui.Splitter();
                {
                    // add> source :: invoke>
                    using (gui.Horizontal())
                    {
                        using (gui.State(foldouts[kAdd] && adding.source != null && adding.target != null && adding.method != null))
                        {
                            if (gui.Button("Add", "Add new handler", EditorStyles.miniButton, Layout.sWidth(40f)))
                            {
                                handlers.Add(new IBaseDelegate.Handler
                                {
                                    target = adding.target,
                                    method = adding.method
                                });

                                var method = adding.method;
                                var paramTypes = method.GetParameters().Select(x => x.ParameterType).ToArray();
                                int len = paramTypes.Length;
                                var argValues = new object[len];
                                argValuesList.Add(argValues);
                                for (int x = 0; x < argValues.Length; x++)
                                    argValues[x] = paramTypes[x].GetDefaultValueEmptyIfString();
                            }
                        }

                        gui.Space(10f);

                        // add foldout
                        if (adding.source != null)
                            foldouts[kAdd] = gui.Foldout(foldouts[kAdd]);

                        // source
                        var obj = gui.Object(adding.source);
                        {
                            if (adding.source != obj)
                            {
                                adding.source = obj;
                                foldouts[kAdd] = true;
                            }
                        }

                        using (gui.State(handlers.Count > 0))
                        {
                            if (gui.Button("Invoke", EditorStyles.miniButtonRight, Layout.sWidth(50f)))
                                memberValue.Invoke();
                        }
                    }

                    // adding area: target -- method
                    if (foldouts[kAdd] && adding.source != null)
                    {
                        gui.Space(5f);
                        gui.Label("Add handler:");
                        using (gui.Indent(GUI.skin.textArea))
                        {
                            // target
                            var gameObject = adding.gameObject;
                            if (gameObject == null)
                            {
                                gui.Popup("Target", 0, new string[] { adding.source.GetType().Name });
                                {
                                    adding.target = adding.source;
                                }
                            }
                            else
                            {
                                if (adding.source is Component)
                                    adding.target = adding.source;

                                var components      = gameObject.GetAllComponents();
                                int cIndex          = components.IndexOfZeroIfNotFound(adding.target);
                                var uniqueNames     = components.Select(c => c.GetType().Name).ToList().Uniqify();
                                var targetSelection = gui.Popup("Target", cIndex, uniqueNames.ToArray());
                                {
                                    if (cIndex != targetSelection || adding.target as Component != components[targetSelection])
                                        adding.target = components[targetSelection];
                                }
                            }

                            // method
                            if (adding.target != null)
                            {
                                var methods = adding.target.GetType()
                                                    .GetMethods(memberValue.ReturnType, memberValue.ParamTypes, Flags.InstancePublic, false)
                                                    .Where(m => !m.IsDefined<HideAttribute>())
                                                    .Where(m => !DelegateSettings.IgnoredMethods.Contains(m.Name))
                                                    .ToList();
                                int mIndex  = methods.IndexOfZeroIfNotFound(adding.method);
                                int methodSelection = gui.Popup("Handler", mIndex, methods.Select(m => m.GetFullName()).ToArray());
                                {
                                    if (!methods.IsEmpty() && (mIndex != methodSelection || adding.method != methods[methodSelection]))
                                        adding.method = methods[methodSelection];
                                }
                            }
                        }
                    }
                }
            }

            // write
            memberValue.handlers = handlers;
        }

        private void SetupMembers(int idx, Type[] paramTypes)
        {
            if (idx < argMembersList.Count)
                return;

            int len = paramTypes.Length;

            var argMembers = new EditorMember[len];
            var argValues = argValuesList[idx];

            argMembersList.Add(argMembers);

            for (int iLoop = 0; iLoop < len; iLoop++)
            {
                int i = iLoop;
                var paramType = paramTypes[i];

                var argMember = EditorMember.WrapGetSet(
                        get: () => argValues[i],
                        set: x => argValues[i] = x,
                        rawTarget: argValues,
                        @unityTarget: unityTarget,
                        @name: string.Format("({0})", paramType.GetNiceName()),
                        @id: id + i,
                        @dataType: paramType,
                        @attributes: null
                    );

                argMembers[i] = argMember;
            }
        }

        private class AddingData
        {
            public UnityObject source;
            public UnityObject target;
            public MethodInfo method;

            public GameObject gameObject
            {
                get
                {
                    var component = source as Component;
                    return component == null ? source as GameObject : component.gameObject;
                }
            }
        }
    }
}
