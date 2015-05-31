//#define DBG

using System;
using System.Reflection;
using UnityEngine;
using Vexe.Editor.GUIs;
using Vexe.Editor.Types;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Types;
using UnityObject = UnityEngine.Object;

namespace Vexe.Editor.Drawers
{
    public abstract class BaseDrawer
    {
        protected BaseGUI gui              { private set; get; }
        protected EditorMember member      { private set; get; }
        protected Attribute[] attributes   { private set; get; }

        protected string displayText       { get { return member.DisplayText;  } set { member.DisplayText = value; } }
        protected object rawTarget         { get { return member.RawTarget;    } }
        protected UnityObject unityTarget  { get { return member.UnityTarget;  } }
        protected string memberTypeName    { get { return member.TypeNiceName; } }
        protected int id                   { get { return member.Id;           } }
        protected Type memberType          { get { return member.Type;         } }
        protected Type targetType          { get { return rawTarget.GetType(); } }

        protected static BetterPrefs prefs;
        protected static Foldouts foldouts;

        private bool _hasInit;
        private MethodCaller<object, string> _dynamicFormatter;
        private static Attribute[] Empty = new Attribute[0];
        private static object[] _formatArgs = new object[1];

        protected bool foldout
        {
            get { return foldouts[id]; }
            set { foldouts[id] = value; }
        }

        protected GameObject gameObject
        {
            get
            {
                var component = unityTarget as Component;
                return component == null ? null : component.gameObject;
            }
        }

        public BaseDrawer()
        {
            if (prefs == null)
                prefs = BetterPrefs.GetEditorInstance();
            if (foldouts == null)
                foldouts = new Foldouts(prefs);
        }

        public BaseDrawer Initialize(EditorMember member, Attribute[] attributes, BaseGUI gui)
        {
            if (attributes == null)
                attributes = Empty;

            this.member     = member;
            this.attributes = attributes;
            this.gui        = gui;

            if (_dynamicFormatter != null)
            { 
                _formatArgs[0] = member.Value;
                displayText = _dynamicFormatter(rawTarget, _formatArgs);
            }

            if (_hasInit)
            {
#if DBG
                Log(this + " is Already initialized");
#endif
                return this;
            }
#if DBG
            Log("Initializing: " + this);
#endif
            var displayAttr = attributes.GetAttribute<DisplayAttribute>();
            if (displayAttr != null && MemberDrawersHandler.IsApplicableAttribute(memberType, displayAttr, attributes))
            {
                var hasCustomFormat = !string.IsNullOrEmpty(displayAttr.FormatMethod);
                var formatMethod = hasCustomFormat ? displayAttr.FormatMethod : ("Format" + member.Name);
                var method = targetType.GetMemberFromAll(formatMethod, Flags.StaticInstanceAnyVisibility) as MethodInfo;
                if (method == null)
                {
                    if (hasCustomFormat)
                        Debug.Log("Couldn't find format method: " + displayAttr.FormatMethod);
                }
                else
                {
                    if (method.ReturnType != typeof(string) && method.GetParameters().Length > 0)
                        Debug.Log("Format Method should return a string and take no parameters: " + method);
                    else
                    { 
                        _dynamicFormatter = method.DelegateForCall<object, string>();
                        _formatArgs[0] = member.Value;
                        displayText = _dynamicFormatter(rawTarget, _formatArgs);
                    }
                }
            }

            _hasInit = true;
            InternalInitialize();
            Initialize();
            return this;
        }

        public bool Foldout()
        {
            return foldout = gui.Foldout(foldout);
        }

        protected virtual void InternalInitialize() { }
        protected virtual void Initialize() { }

        public abstract void OnGUI();
        public virtual void OnUpperGUI() { }
        public virtual void OnLeftGUI()  { }
        public virtual void OnRightGUI() { }
        public virtual void OnLowerGUI() { }
        public virtual void OnMemberDrawn(Rect rect) { }

        public abstract bool CanHandle(Type memberType);

        protected static void LogFormat(string msg, params object[] args)
        {
            Debug.Log(string.Format(msg, args));
        }

        protected static void Log(object msg)
        {
            Debug.Log(msg);
        }
    }
}
