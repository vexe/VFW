using UnityEngine;
using UnityEditor;
using Vexe.Editor;
using Vexe.Editor.Drawers;

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
                                       .Insert<OverrideAttribute, CustomDrawer2>();
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
}
