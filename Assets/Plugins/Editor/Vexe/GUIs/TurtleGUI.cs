using System;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using Vexe.Editor.Helpers;
using UnityObject = UnityEngine.Object;

namespace Vexe.Editor.GUIs
{
    public class TurtleGUI : BaseGUI
    {
        private static HorizontalBlock horizontal;
        private static VerticalBlock vertical;

		private static MethodInfo gradientFieldMethod;

        public override Rect LastRect
        {
            get { return GUILayoutUtility.GetLastRect(); }
        }

        public override void OnGUI(Action code, Vector4 padding, int targetId)
        {
            code();
        }

        public override Bounds BoundsField(GUIContent content, Bounds value, Layout option)
        {
            return EditorGUILayout.BoundsField(content, value, option);
        }

        public override void Box(GUIContent content, GUIStyle style, Layout option)
        {
            GUILayout.Box(content, style, option);
        }

        public override void HelpBox(string message, MessageType type)
        {
            EditorGUILayout.HelpBox(message, type);
        }

        public override bool Button(GUIContent content, GUIStyle style, Layout option, ControlType buttonType)
        {
            return GUILayout.Button(content, style, option);
        }

        public override Color Color(GUIContent content, Color value, Layout option)
        {
            return EditorGUILayout.ColorField(content, value, option);
        }

        public override Enum EnumPopup(GUIContent content, System.Enum selected, GUIStyle style, Layout option)
        {
            return EditorGUILayout.EnumPopup(content, selected, style, option);
        }

        public override float Float(GUIContent content, float value, Layout option)
        {
            return EditorGUILayout.FloatField(content, value, option);
        }

        public override int Int(GUIContent content, int value, Layout option)
        {
            return EditorGUILayout.IntField(content, value, option);
        }

        public override bool Foldout(GUIContent content, bool value, GUIStyle style, Layout option)
        {
            var rect = GUILayoutUtility.GetRect(content, style, option);
            return EditorGUI.Foldout(rect, value, content, true, style);
        }

        public override void Label(GUIContent content, GUIStyle style, Layout option)
        {
            GUILayout.Label(content, style, option);
        }

        public override int MaskField(GUIContent content, int mask, string[] displayedOptions, GUIStyle style, Layout option)
        {
            return EditorGUILayout.MaskField(content, mask, displayedOptions, style, option);
        }

        public override UnityObject Object(GUIContent content, UnityObject value, System.Type type, bool allowSceneObjects, Layout option)
        {
            // If we pass an empty content, ObjectField will still reserve space for an empty label ~__~
            return string.IsNullOrEmpty(content.text) ?
                EditorGUILayout.ObjectField(value, type, allowSceneObjects, option) :
                EditorGUILayout.ObjectField(content, value, type, allowSceneObjects, option);
        }

        public override int Popup(string text, int selectedIndex, string[] displayedOptions, GUIStyle style, Layout option)
        {
            return EditorGUILayout.Popup(text, selectedIndex, displayedOptions, style, option);
        }

        public override Rect Rect(GUIContent content, Rect value, Layout option)
        {
            return EditorGUILayout.RectField(content, value, option);
        }

		public override AnimationCurve Curve (GUIContent content, AnimationCurve value, Layout option)
		{
			return EditorGUILayout.CurveField (content, value, option);
		}

		public override Gradient GradientField (GUIContent content, Gradient value, Layout option)
		{
			if (value == null)
				value = new Gradient ();
			return (Gradient)gradientFieldMethod.Invoke (null, new object[] { content, value, option });
		}

        protected override void BeginScrollView(ref Vector2 pos, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, GUIStyle background, Layout option)
        {
            pos = GUILayout.BeginScrollView(pos, alwaysShowHorizontal, alwaysShowVertical, horizontalScrollbar, verticalScrollbar, background, option);
        }

        protected override void EndScrollView()
        {
            GUILayout.EndScrollView();
        }

        public override float FloatSlider(GUIContent content, float value, float leftValue, float rightValue, Layout option)
        {
            return EditorGUILayout.Slider(content, value, leftValue, rightValue, option);
        }

        public override void Space(float pixels)
        {
            GUILayout.Space(pixels);
        }

        public override void FlexibleSpace()
        {
            GUILayout.FlexibleSpace();
        }

        public override string Text(GUIContent content, string value, GUIStyle style, Layout option)
        {
            return EditorGUILayout.TextField(content, value, style, option);
        }

        public override string ToolbarSearch(string value, Layout option)
        {
            return GUIHelper.ToolbarSearchField_GL(null, new object[] { value, option.ToGLOptions() }) as string;
        }

        public override bool Toggle(GUIContent content, bool value, GUIStyle style, Layout option)
        {
            return EditorGUILayout.Toggle(content, value, style, option);
        }

        public override bool ToggleLeft(GUIContent content, bool value, GUIStyle labelStyle, Layout option)
        {
            return EditorGUILayout.ToggleLeft(content, value, labelStyle, option);
        }

        static TurtleGUI()
        {
            horizontal = new HorizontalBlock();
            vertical = new VerticalBlock();
			Type tyEditorGUILayout = typeof(EditorGUILayout);
			gradientFieldMethod = tyEditorGUILayout.GetMethod("GradientField", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { typeof(GUIContent), typeof(Gradient), typeof(GUILayoutOption[]) }, null);
		}

        public TurtleGUI()
        {
            horizontal.onDisposed = EndHorizontal;
            vertical.onDisposed = EndVertical;
        }

        protected override HorizontalBlock BeginHorizontal(GUIStyle style)
        {
            GUILayout.BeginHorizontal(style);
            return horizontal;
        }

        protected override VerticalBlock BeginVertical(GUIStyle style)
        {
            GUILayout.BeginVertical(style);
            return vertical;
        }

        protected override void EndHorizontal()
        {
            GUILayout.EndHorizontal();
        }

        protected override void EndVertical()
        {
            GUILayout.EndVertical();
        }

        public override string TextArea(string value, Layout option)
        {
            return EditorGUILayout.TextArea(value, option);
        }

        public override bool InspectorTitlebar(bool foldout, UnityObject target)
        {
            return EditorGUILayout.InspectorTitlebar(foldout, target);
        }

        public override string Tag(GUIContent content, string tag, GUIStyle style, Layout layout)
        {
            return EditorGUILayout.TagField(content, tag, style, layout);
        }

        public override int LayerField(GUIContent label, int layer, GUIStyle style, Layout layout)
        {
            return EditorGUILayout.LayerField(label, layer, style, layout);
        }

        public override void Prefix(string label)
        {
            EditorGUILayout.PrefixLabel(label);
        }

        public override string ScrollableTextArea(string value, ref Vector2 scrollPos, GUIStyle style, Layout option)
        {
            throw new NotImplementedException();
        }

        public override string TextFieldDropDown(GUIContent label, string text, string[] displayedOptions, Layout option)
        {
            throw new NotImplementedException();
        }

        public override double Double(GUIContent content, double value, Layout option)
        {
            return EditorGUILayout.DoubleField(content, value, option);
        }

        public override long Long(GUIContent content, long value, Layout option)
        {
            return EditorGUILayout.LongField(content, value, option);
        }

        public override IDisposable If(bool condition, IDisposable body)
        {
            throw new NotImplementedException();
        }

        public override void MinMaxSlider(GUIContent label, ref float minValue, ref float maxValue, float minLimit, float maxLimit, Layout option)
        {
            throw new NotImplementedException();
        }
    }
}
