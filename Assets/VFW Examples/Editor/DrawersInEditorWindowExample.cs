using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Vexe.Editor.GUIs;
using Vexe.Editor.Visibility;
using Vexe.Runtime.Serialization;
using Vexe.Runtime.Types;

namespace VFWExamples
{
    public class DrawersInEditorWindowExample : EditorWindow
    {
        private BaseGUI gui;
        private List<MemberInfo> members;

        [Tags, FilterTags]
        public string playerTag { get; set; }

        [Show, SelectEnum, FilterEnum]
        private KeyCode jumpKey;

        [Comment("The GUI layouting system to use to draw this editor window.\nCurrently there's TurtleGUI which wraps Unity's GUILayout\nand RabbitGUI which is a faster alternative layouting system"),
        ShowType(typeof(BaseGUI))]
        public Type guiType = typeof(RabbitGUI);

        public ITestInterface itface;

        public Component[] targets;

        private int id;

        [Show]
        void SomeMethod()
        {
            Debug.Log("SomeMethod");
        }

        private void OnEnable()
        {
            id = Guid.NewGuid().GetHashCode();
            members = VisibilityLogic.CachedGetVisibleMembers(GetType());
        }

        private void OnGUI()
        {
            if (gui == null || guiType != gui.GetType())
            {
                gui = BaseGUI.Create(guiType);
                Repaint();
            }

            gui.OnGUI(GUICode, new Vector2(5f, 5f), id); // the vector is just padding (or border offsets). x coord is left, y is right
        }

        private void GUICode()
        {
            foreach (var member in members)
                gui.Member(
                    member, // the member that we're drawing
                    this,	// the unity target object, used for undo
                    this,	// the object that the members belong to, in this case its the same object
                    id,		// a unique id. used for foldout values and hash codes
                    false); // whether we want to ignore composite drawers for our members or not
        }

        public interface ITestInterface
        {
            List<GameObject> List { get; set; }
            Dictionary<string, Vector3> Dict { get; set; }
        }

        public class TestClass1 : ITestInterface
        {
            float x, y;
            public List<GameObject> List { get; set; }
            public Dictionary<string, Vector3> Dict { get; set; }
        }

        public class TestClass2 : ITestInterface
        {
            public int num;
            public string name;
            public List<GameObject> List { get; set; }
            public Dictionary<string, Vector3> Dict { get; set; }
        }

        public static class MenuItems
        {
            [MenuItem("Window/Vexe/Examples/DrawersInEditorWindow")]
            public static void ShowMyWindow()
            {
                EditorWindow.GetWindow<DrawersInEditorWindowExample>();
            }
        }
    }
}
