using UnityEngine;

namespace Vexe.Editor.GUIs
{
    public abstract partial class BaseGUI
    {
        public int LayerField(int layer)
        {
            return LayerField(string.Empty, layer);
        }

        public int LayerField(string label, int layer)
        {
            return LayerField(label, layer, null);
        }

        public LayerMask LayerField(string content, LayerMask layer)
        {
            return LayerField(content, (int)layer);
        }

        public int LayerField(string label, int layer, Layout layout)
        {
            return LayerField(label, layer, GUIStyles.Popup, layout);
        }

        public int LayerField(string label, int layer, GUIStyle style, Layout layout)
        {
            return LayerField(GetContent(label), layer, style, layout);
        }

        public abstract int LayerField(GUIContent label, int layer, GUIStyle style, Layout layout);
    }
}