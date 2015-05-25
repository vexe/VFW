using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Vexe.Editor.GUIs;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Helpers;
using Vexe.Runtime.Types;
using UnityObject = UnityEngine.Object;

namespace Vexe.Editor.Internal
{
    public class MembersCategory
    {
        private static readonly BetterPrefs _prefs = BetterPrefs.GetEditorInstance();

        private readonly int _id;
        private CategoryDisplay _display;

        public readonly string FullPath;
        public readonly string Name;

        public List<MemberInfo> Members;
        public List<MembersCategory> NestedCategories;

        public int Spacing;
        public BaseGUI gui;
        public float DisplayOrder;
        public bool ForceExpand, HideHeader, IsExpanded, Indent, AlwaysHideHeader;

        public CategoryDisplay Display
        {
            get { return _display; }
            set
            {
                if (_display != value)
                {
                    _display = value;
                    for (int i = 0; i < NestedCategories.Count; i++)
                        NestedCategories[i].Display = value;
                }
            }
        }

        public MembersCategory(string fullPath, float displayOrder, int id)
        {
            this.DisplayOrder = displayOrder;
            this.FullPath = fullPath;
            this.Name = FullPath.Substring(FullPath.LastIndexOf('/') + 1);
            this._id = RuntimeHelper.CombineHashCodes(id, fullPath.GetHashCode());
            Indent = true;

            NestedCategories = new List<MembersCategory>();
            Members = new List<MemberInfo>();
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
            var boxed = (_display & CategoryDisplay.BoxedHeaders) == CategoryDisplay.BoxedHeaders;
            using (gui.Horizontal(boxed ? GUIStyles.ToolbarButton : GUIStyle.none))
            {
                gui.Space(10f);
                foldout = gui.Foldout(Name, _prefs.Bools.ValueOrDefault(_id, true), Layout.Auto);
                _prefs.Bools[_id] = foldout;
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

            gui.Space(1f);

            bool showGuiBox   = (Display & CategoryDisplay.BoxedMembersArea) > 0;
            bool memberSplitter = (Display & CategoryDisplay.MemberSplitter) > 0;

            using (gui.Indent(showGuiBox ? EditorStyles.textArea : GUIStyle.none, Indent))
            {
                gui.Space(5f);
                for (int i = 0, imax = Members.Count; i < imax; i++)
                {
                    var member = Members[i];

                    var isVisible = ConditionalVisibility.IsVisible(member, target);
                    if (!isVisible)
                        continue;

                    using (gui.Horizontal())
                    {
                        gui.Space(Spacing);
                        using (gui.Vertical())
                            gui.Member(member, target, target, _id, false);
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
                    cat.Display = _display;

                    if ((_display & CategoryDisplay.CategorySplitter) != 0)
                        gui.Splitter();

                    using (gui.Horizontal())
                    {
                        if (IsExpanded)
                            gui.Space(4f);

                        using (gui.Vertical())
                            cat.Draw(target);
                    }
                }
            }
        }
    }
}
