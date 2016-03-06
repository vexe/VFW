using System;
using UnityEditor;
using Vexe.Editor;
using Vexe.Editor.Drawers;
using Vexe.Editor.Types;
using Vexe.Runtime.Types;

namespace VFWExamples
{
    /// <summary>
    /// An example showing how to register your own custom drawers with the new mapping system.
    /// The easiest way is just to create a static class marked with [InitializeOnLoad]
    /// and map your drawers in its static constructor.
    /// </summary>
    [InitializeOnLoad]
    public static class CustomMapper
    {
        static CustomMapper()
        {
            MemberDrawersHandler.Mapper.Insert<CustomObject, CustomDrawer1>()
                                       .Insert<OverrideAttribute, CustomDrawer2>()
                                       .Insert<Index2D, CustomDrawer3>()
                                       .Insert<ItemsLookup, LookupDrawer>();
        }
    }

    public class CustomDrawer1 : ObjectDrawer<CustomObject>
    {
        public override void OnGUI()
        {
            if (memberValue == null)
                memberValue = new CustomObject();

            gui.HelpBox("Hey yo check me out I'm all customly drawn n' stuff");
            memberValue.str = gui.Text("Monster name", memberValue.str);
        }
    }

    public class CustomDrawer2 : AttributeDrawer<CustomObject, OverrideAttribute>
    {
        public override void OnGUI()
        {
            if (memberValue == null)
                memberValue = new CustomObject();

            gui.HelpBox("I'm overridden :(");
            memberValue.str = gui.Text("Override", memberValue.str);
        }
    }

    public class CustomDrawer3 : ObjectDrawer<Index2D>
    {
        private EditorMember i, j;

        protected override void Initialize()
        {
            // this is only necessary if Index2D was a class
            //if (memberValue == null)
            //    memberValue = new Index2D();

            // 'i' and 'j' could be fields or properties
            i = FindRelativeMember("i");
            j = FindRelativeMember("j");
        }

        public override void OnGUI()
        {
            using (gui.Horizontal())
            {
                gui.Prefix(displayText);
                using (gui.LabelWidth(13f))
                {
                    // gui.Member will take into consideration attributes applied on 'i' and 'j'
                    // as well as handling undo
                    gui.Member(i);
                    gui.Member(j);
                }
            }
        }
    }

    public class LookupDrawer : ObjectDrawer<ItemsLookup>
    {
        // since our lookup 'is a' Dictionary, we will use a dictionary drawer
        IDictionaryDrawer<string, int> drawer;

        protected override void Initialize()
        {
            base.Initialize();

            drawer = new IDictionaryDrawer<string, int>();

            // Here we play our secret card showing the power of injecting attributes!
            drawer.Initialize(member,
                new Attribute[]
                {
                    new DisplayAttribute(Dict.HorizontalPairs), // apply horizontal display on the dictionary pairs
                    new PerKeyAttribute("Popup"), // popup per key
                    new PopupAttribute("ItemsLookup.GetValues")
                    { 
                        TextField = true, // we want a nice text field
                        TakeLastPathItem = true, // since we're using '/' in our values, we only care about the last item in the path
                        HideUpdate = true, // we don't care about updating the popup values
                        Filter = true, // since we have many values, it'd be nice if we can filter them quickly
                    },
                }, gui, prefs);
        }

        public override void OnGUI()
        {
            if (memberValue == null)
                memberValue = new ItemsLookup();

            drawer.OnGUI();
        }
    }
}
