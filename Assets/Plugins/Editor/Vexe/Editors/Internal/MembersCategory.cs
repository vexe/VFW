//#define PROFILE

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Vexe.Editor.GUIs;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Types;
using UnityObject = UnityEngine.Object;

namespace Vexe.Editor.Internal
{
    public class MembersCategory
    {
        private readonly int id;
        private readonly BetterPrefs prefs;
        private MembersDisplay display;
        private UnityObject prevTarget;

        public readonly string FullPath;
        public readonly string Name;

        public List<MemberInfo> Members;
        public List<MembersCategory> NestedCategories;

        public int Spacing;
        public BaseGUI gui;
        public float DisplayOrder;
        public bool ForceExpand, HideHeader, IsExpanded, Indent, AlwaysHideHeader;

        public MembersDisplay Display
        {
            get { return display; }
            set
            {
                if (display != value)
                {
                    display = value;
                    Members.OfType<MembersCategory>().Foreach(c => c.Display = display);
                }
            }
        }

        public MembersCategory(string fullPath, List<MemberInfo> members, float displayOrder, int id, BetterPrefs prefs)
        {
            Members = members;
            DisplayOrder = displayOrder;
            this.prefs = prefs;
            this.FullPath = fullPath;
            this.Name = FullPath.Substring(FullPath.LastIndexOf('/') + 1);
            this.id = id + fullPath.GetHashCode();
            Indent = true;

            NestedCategories = new List<MembersCategory>();
        }

        public void AddMember(MemberInfo member)
        {
            Members.Add(member);
        }

        public void RemoveEmptyNestedCategories()
        {
            for (int i = NestedCategories.Count - 1; i > -1; i--)
            {
                var nested = NestedCategories[i];
                if (nested.Members.Count == 0 && nested.NestedCategories.Count == 0)
                    NestedCategories.RemoveAt(i);
                else nested.RemoveEmptyNestedCategories();
            }
        }

        private bool DoHeader()
        {
            bool foldout = false;
            var boxed = (display & MembersDisplay.BoxedHeaders) == MembersDisplay.BoxedHeaders;
            using (gui.Horizontal(boxed ? Styles.ToolbarButton : GUIStyle.none))
            {
                gui.Space(10f);
                foldout = gui.Foldout(Name, prefs.Bools.ValueOrDefault(id, true), Layout.sExpandWidth());
                prefs.Bools[id] = foldout;
            }

            return foldout;
        }

        public void Draw(UnityObject target)
        {
            if (Members.Count == 0 && NestedCategories.Count == 0)
                return;

            IsExpanded = AlwaysHideHeader || HideHeader || DoHeader();
            if (!(IsExpanded || ForceExpand))
                return;

            Indent = !(AlwaysHideHeader || HideHeader);

            bool changedTarget;
            if (target != prevTarget)
            {
                prevTarget = target;
                changedTarget = true;
            }
            else changedTarget = false;

            gui.Space(1f);

            bool showGuiBox   = (Display & MembersDisplay.BoxedMembersArea) > 0;
            bool memberSplitter = (Display & MembersDisplay.MemberSplitter) > 0;

            using (gui.Indent(showGuiBox ? EditorStyles.textArea : GUIStyle.none, Indent))
            {
                gui.Space(5f);
#if PROFILE
				Profiler.BeginSample(name + " Members");
#endif
                for (int i = 0, imax = Members.Count; i < imax; i++)
                {
                    var member = Members[i];

                    if (!IsVisible(member, target, changedTarget))
                        continue;

                    using (gui.Horizontal())
                    {
                        gui.Space(Spacing);
                        using (gui.Vertical())
                            gui.Member(member, target, target, id, false);
                    }

                    if (memberSplitter && i != imax - 1)
                        gui.Splitter();

                    gui.Space(2f);
                }

                for (int i = 0, imax = NestedCategories.Count; i < imax; i++)
                {
                    var cat = NestedCategories[i];

                    cat.gui = this.gui;
                    cat.HideHeader = this.HideHeader;
                    cat.display = display;

                    if ((display & MembersDisplay.CategorySplitter) != 0)
                        gui.Splitter();

                    using (gui.Horizontal())
                    {
                        if (IsExpanded)
                            gui.Space(4f);

                        using (gui.Vertical())
                            cat.Draw(target);
                    }
                }
#if PROFILE
				Profiler.EndSample();
#endif
            }
        }

        static Dictionary<MemberInfo, MethodCaller<UnityObject, bool>> _isVisibleCache = new Dictionary<MemberInfo, MethodCaller<UnityObject, bool>>();

        static public bool IsVisible(MemberInfo member, UnityObject target, bool changedTarget)
        {
            MethodCaller<UnityObject, bool> isVisible;
            if (changedTarget || !_isVisibleCache.TryGetValue(member, out isVisible))
            {
                var vis = member.GetAttributes().GetAttribute<VisibleWhenAttribute>();
                if (vis == null)
                    return true;

                var flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
                var method = target.GetType().GetMethod(vis.conditionMethod, flags);
                if (method == null)
                {
                    Debug.LogError("Method not found: " + vis.conditionMethod);
                    _isVisibleCache[member] = null;
                    return true;
                }

                _isVisibleCache[member] = isVisible = method.DelegateForCall<UnityObject, bool>();
            }

            return isVisible.Invoke(target, null);
        }
    }
}
