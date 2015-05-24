using UnityEditor;
using UnityEngine;
using Vexe.Runtime.Extensions;

namespace Vexe.Editor
{
    public static class GUIStyles
    {
        public static readonly GUIStyle None                     = GUIStyle.none;
        public static readonly GUIStyle Button                   = GUI.skin.button;
        public static readonly GUIStyle HorizontalSlider         = GUI.skin.horizontalSlider;
        public static readonly GUIStyle Box                      = GUI.skin.box;
        public static readonly GUIStyle HorizontalScrollbar      = GUI.skin.horizontalScrollbar;
        public static readonly GUIStyle VerticalScrollbar        = GUI.skin.verticalScrollbar;
        public static readonly GUIStyle ScrollView               = GUI.skin.scrollView;
        public static readonly GUIStyle Label                    = EditorStyles.label;
        public static readonly GUIStyle BoldLabel                = EditorStyles.boldLabel;
        public static readonly GUIStyle Foldout                  = EditorStyles.foldout;
        public static readonly GUIStyle MiniLeft                 = EditorStyles.miniButtonLeft;
        public static readonly GUIStyle MiniRight                = EditorStyles.miniButtonRight;
        public static readonly GUIStyle MiniMid                  = EditorStyles.miniButtonMid;
        public static readonly GUIStyle Mini                     = EditorStyles.miniButton;
        public static readonly GUIStyle TextField                = EditorStyles.textField;
        public static readonly GUIStyle TextArea                 = EditorStyles.textArea;
        public static readonly GUIStyle ObjectField              = EditorStyles.objectField;
        public static readonly GUIStyle Popup                    = EditorStyles.popup;
        public static readonly GUIStyle Toggle                   = EditorStyles.toggle;
        public static readonly GUIStyle ColorField               = EditorStyles.colorField;
        public static readonly GUIStyle NumberField              = EditorStyles.numberField;
        public static readonly GUIStyle Toolbar                  = EditorStyles.toolbar;
        public static readonly GUIStyle ToolbarButton            = EditorStyles.toolbarButton;
        public static readonly GUIStyle HelpBox                  = EditorStyles.helpBox;
        public static readonly GUIStyle TextFieldDropDown        = typeof(EditorStyles).GetProperty("textFieldDropDown", Flags.StaticAnyVisibility).GetValue(null, null) as GUIStyle;
        public static readonly GUIStyle TextFieldDropDownText    = typeof(EditorStyles).GetProperty("textFieldDropDownText", Flags.StaticAnyVisibility).GetValue(null, null) as GUIStyle;

    }
}