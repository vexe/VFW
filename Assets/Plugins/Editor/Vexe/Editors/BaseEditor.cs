//#define PROFILE
//#define DBG

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Vexe.Editor.GUIs;
using Vexe.Editor.Internal;
using Vexe.Editor.Types;
using Vexe.Editor.Visibility;
using Vexe.Runtime.Exceptions;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Helpers;
using Vexe.Runtime.Types;
using UnityObject = UnityEngine.Object;

namespace Vexe.Editor.Editors
{
    using Editor = UnityEditor.Editor;

    public abstract class BaseEditor : Editor
    {
        /// <summary>
        /// Whether or not to show the script field header
        /// </summary>
        public bool ShowScriptHeader = true;

        protected BaseGUI gui;

        private int _id = -1;
        private Type _targetType;
        private static Foldouts _foldouts;

        private List<MembersCategory> _categories;
        private List<MemberInfo> _visibleMembers;
        private SerializedProperty _script;
        private EditorMember _serializationData, _debug;
        private bool _previousGUI;
        private int _repaintCount, _spacing;
        private MembersDisplay _display;
        private Action _onGUIFunction;
        static int guiKey = "UnityGUI".GetHashCode();

        private static bool useUnityGUI
        {
            get { return prefs.Bools.ValueOrDefault(guiKey); }
            set { prefs.Bools[guiKey] = value; }
        }

        protected GameObject gameObject
        {
            get
            {
                var comp = target as Component;
                return comp == null ? null : comp.gameObject;
            }
        }

        protected static BetterPrefs prefs
        {
            get { return BetterPrefs.EditorInstance; }
        }

        protected int id
        {
            get { return _id != -1 ? _id : (_id = RuntimeUtils.GetTargetID(target)); }
        }

        protected bool foldout
        {
            get { return foldouts[id]; }
            set { foldouts[id] = value; }
        }

        protected static Foldouts foldouts
        {
            get { return _foldouts ?? (_foldouts = new Foldouts(prefs)); }
        }

        protected Type targetType
        {
            get { return _targetType ?? (_targetType = target.GetType()); }
        }

        private void OnEnable()
        {
            _previousGUI = useUnityGUI;
            gui = _previousGUI ? (BaseGUI)new TurtleGUI() : new RabbitGUI();

            Initialize();

            //addresses the disappearing of the editor when changing playmode
            var rabbit = gui as RabbitGUI;
            if (rabbit != null)
            {
                if (!rabbit.validRect.HasValue)
                { 
                    var key = RTHelper.CombineHashCodes(id, "rabbit_coords");
                    Vector3 prevCoords;
                    if (prefs.Vector3s.TryGetValue(key, out prevCoords))
                    {
                        //Log("Seems we changed play modes and rabbit doesn't have a coord. but we have in store a prev coord from a previous editor session that should work");
                        var rect = new Rect();
                        rect.x = prevCoords.x;
                        rect.y = prevCoords.y;
                        rabbit.validRect = rect;
                    }
                }
            }

            EditorApplication.playmodeStateChanged += StoreGUICoords;
        }

        void OnDisable()
        {
            EditorApplication.playmodeStateChanged -= StoreGUICoords;
        }

        private void StoreGUICoords()
        {
            //Log("Playmode changed");
            var rabbit = gui as RabbitGUI;
            if (rabbit != null)
            {
                if (rabbit.validRect.HasValue)
                {
                    var key = RTHelper.CombineHashCodes(id, "rabbit_coords");
                    prefs.Vector3s[key] = new Vector3(rabbit.validRect.Value.x, rabbit.validRect.Value.y);
                }
            }
        }

        /// <summary>
        /// Call this if you're inlining this editor and you're using your own gui instance
        /// It's important in that case to use the same gui in order to have the same layout
        /// </summary>
        public void OnInlinedGUI(BaseGUI otherGui)
        {
            this.gui = otherGui;
            OnGUI();
        }

        public sealed override void OnInspectorGUI()
        {
            if (_previousGUI != useUnityGUI)
            {
                _previousGUI = useUnityGUI;
                gui = _previousGUI ? (BaseGUI)new TurtleGUI() : new RabbitGUI();
            }

            if (_onGUIFunction == null) // creating the delegate once, reducing allocation
                _onGUIFunction = OnGUI;

            gui.OnGUI(_onGUIFunction, new Vector2(0f, 18f));

            // addresses somes cases of editor slugishness when selecting gameObjects
            if (_repaintCount < 2)
            {
                _repaintCount++;
                Repaint();
            }
        }

        protected static void LogFormat(string msg, params object[] args)
        {
            Debug.Log(string.Format(msg, args));
        }

        protected static void Log(object msg)
        {
            Debug.Log(msg);
        }

        protected virtual void OnBeforeInitialized() { }
        protected virtual void OnAfterInitialized() { }

        private void Initialize()
        {
            OnBeforeInitialized();

            // Init members
            _visibleMembers = VFWVisibilityLogic.GetCachedVisibleMembers.Invoke(targetType).ToList();

            _categories = new List<MembersCategory>();

            Func<string, float,  MembersCategory> newCategory = (path, order) =>
                new MembersCategory(path, new List<MemberInfo>(), order, id, prefs);

            var resolver = new MembersResolution();

            var multiple	= targetType.GetCustomAttribute<DefineCategoriesAttribute>(true);
            var definitions = targetType.GetCustomAttributes<DefineCategoryAttribute>(true);
            if (multiple != null)
                definitions = definitions.Concat(multiple.names.Select(n => new DefineCategoryAttribute(n, 1000)));

            Func<string, string[]> ParseCategoryPath = fullPath =>
            {
                int nPaths = fullPath.Split('/').Length;
                string[] result = new string[nPaths];
                for (int i = 0, index = -1; i < nPaths - 1; i++)
                {
                    index = fullPath.IndexOf('/', index + 1);
                    result[i] = fullPath.Substring(0, index);
                }
                result[nPaths - 1] = fullPath;
                return result;
            };

            // Order by exclusivity
            var defs = from d in definitions
                       let paths = ParseCategoryPath(d.FullPath)
                       orderby !d.Exclusive
                       select new { def = d, paths };

            Func<MemberInfo, float> getDisplayOrder = member =>
            {
                var attr = member.GetCustomAttribute<DisplayOrderAttribute>();
                return attr == null ? -1 : attr.displayOrder;
            };

            // Parse paths and resolve definitions
            var lookup = new Dictionary<string, MembersCategory>();
            foreach (var x in defs)
            {
                var paths = x.paths;
                var d = x.def;

                MembersCategory parent = null;

                for (int i = 0; i < paths.Length; i++)
                {
                    var p = paths[i];

                    var current = (parent == null ?
                        _categories : parent.NestedCategories).FirstOrDefault(c => c.FullPath == p);

                    if (current == null)
                    {
                        current = newCategory(p, d.DisplayOrder);
                        if (i == 0)
                            _categories.Add(current);
                        if (parent != null)
                            parent.NestedCategories.Add(current);
                    }
                    lookup[p] = current;
                    parent = current;
                }

                var last = lookup[paths.Last()];
                last.ForceExpand = d.ForceExpand;
                last.AlwaysHideHeader = d.AlwaysHideHeader;
                resolver.Resolve(_visibleMembers, d).Foreach(last.AddMember);

                lookup.Clear();
                parent.Members = parent.Members.OrderBy(getDisplayOrder).ToList();
            }

            // filter out empty categories
            _categories = _categories.Where(x => x.NestedCategories.Count > 0 || x.Members.Count > 0)
                                     .OrderBy(x => x.DisplayOrder)
                                     .ToList();

            for (int i = 0; i < _categories.Count; i++)
            {
                var c = _categories[i];
                c.RemoveEmptyNestedCategories();
            }

            var displayKey = RTHelper.CombineHashCodes(id, "display");
            var displayValue = prefs.Ints.ValueOrDefault(displayKey, -1);
            var vfwSettings = VFWSettings.GetInstance();
            _display = displayValue == -1 ? vfwSettings.DefaultDisplay : (MembersDisplay)displayValue;
            prefs.Ints[displayKey] = (int)_display;

            var spacingKey = RTHelper.CombineHashCodes(id, "spacing");
            _spacing = prefs.Ints.ValueOrDefault(spacingKey, vfwSettings.DefaultSpacing);
            prefs.Ints[spacingKey] = _spacing;

            var field = targetType.GetAllMembers(typeof(MonoBehaviour), Flags.InstancePrivate)
                                  .FirstOrDefault(m => m.Name == "_serializationData");
            if (field == null) throw new MemberNotFoundException("_serializationData in " + targetType.Name);

            _serializationData = new EditorMember(field, target, target, id);

            field = targetType.GetField("dbg", Flags.InstanceAnyVisibility);
            if (field == null) throw new MemberNotFoundException("dbg");

            _debug = new EditorMember(field, target, target, id);

            OnAfterInitialized();
        }

        protected virtual void OnGUI()
        {
#if PROFILE
            Profiler.BeginSample(targetType.Name + " OnInspectorGUI");
            Profiler.BeginSample(targetType.Name + " Header");
#endif
            if (ShowScriptHeader)
            {
                var scriptKey = id + "script".GetHashCode();
                gui.Space(3f);
                using (gui.Horizontal(EditorStyles.toolbarButton))
                {
                    gui.Space(10f);
                    foldouts[scriptKey] = gui.Foldout(foldouts[scriptKey]);
                    gui.Space(-12f);

                    if (ScriptField()) // script changed? exit!
                        return;
                }

                if (foldouts[scriptKey])
                {
                    gui.Space(2f);

                    using (gui.Indent(GUI.skin.textField))
                    {
                        gui.Space(3f);
                        if (targetType.IsDefined<HasRequirementsAttribute>())
                        {
                            using (gui.Horizontal())
                            {
                                gui.Space(3f);
                                if (gui.MiniButton("Resolve Requirements", (Layout)null))
                                    Requirements.Resolve(target, gameObject);
                            }
                        }

                        gui.Member(_debug);

                        var mask = gui.BunnyMask("Display", _display);
                        {
                            var newValue = (MembersDisplay)mask;
                            if (_display != newValue)
                            {
                                _display = newValue;
                                prefs.Ints[id + "display".GetHashCode()] = mask;
                            }
                        }

                        var spacing = Mathf.Clamp(gui.Int("Spacing", _spacing), -13, (int)EditorGUIUtility.currentViewWidth / 4);
                        if (_spacing != spacing)
                        {
                            _spacing = spacing;
                            prefs.Ints[id + "spacing".GetHashCode()] = _spacing;
                            gui.RequestResetIfRabbit();
                        }

                        gui.Member(_serializationData, true);
                    }
                }
            }

#if PROFILE
            Profiler.EndSample();
#endif

            gui.BeginCheck();

#if PROFILE
            Profiler.BeginSample(targetType.Name + " Members");
#endif

            for (int i = 0; i < _categories.Count; i++)
            {
                var cat     = _categories[i];
                cat.Display = _display;
                cat.Spacing = _spacing;
                cat.gui = gui;
                cat.HideHeader = (_display & MembersDisplay.Headers) != MembersDisplay.Headers;
                if ((_display & MembersDisplay.CategorySplitter) != 0)
                    gui.Splitter();
                cat.Draw(target);
            }

#if PROFILE
            Profiler.EndSample();
#endif

            if (gui.HasChanged())
            {
                //Log("setting dirty " + target);
                EditorUtility.SetDirty(target);
            }

#if PROFILE
            Profiler.EndSample();
#endif
        }

        private bool ScriptField()
        {
            serializedObject.Update();

            _script = serializedObject.FindProperty("m_Script");

            EditorGUI.BeginChangeCheck();
            _script.objectReferenceValue = gui.Object("Script", _script.objectReferenceValue, typeof(MonoScript), false);
            if (EditorGUI.EndChangeCheck())
            {
                var sel = Selection.objects;
                Selection.objects = new UnityObject[0];
                EditorApplication.delayCall += () => Selection.objects = sel;
                serializedObject.ApplyModifiedProperties();
                return true;
            }

            return false;
        }

        public static class MenuItems
        {
            [MenuItem("Tools/Vexe/GUI/UseUnityGUI")]
            public static void UseUnityGUI()
            {
                BetterPrefs.EditorInstance.Bools[guiKey] = true;
            }

            [MenuItem("Tools/Vexe/GUI/UseRabbitGUI")]
            public static void UseRabbitGUI()
            {
                BetterPrefs.EditorInstance.Bools[guiKey] = false;
            }
        }
    }
}
