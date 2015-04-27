//#define DBG

using System;
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

        private bool initialized;

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
            this.member     = member;
            this.attributes = attributes;
            this.gui        = gui;

            if (initialized)
            {
#if DBG
                Log(this + " is Already initialized");
#endif
                return this;
            }
#if DBG
            Log("Initializing: " + this);
#endif
            initialized = true;
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
