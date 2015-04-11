//#define DBG

using System;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Vexe.Editor.Drawers
{
    public abstract class BasicDrawer<T> : ObjectDrawer<T>
    {
        protected virtual T DoField(string text, T value)
        {
            throw new NotImplementedException();
        }

        public override void OnGUI()
        {
            memberValue = DoField(niceName, memberValue);
#if DBG
            var curValue = memberValue;
            var newValue = DoField(niceName, curValue);
            memberValue = newValue;
            if(!newValue.Equals(curValue))
                Debug.Log(newValue);
#endif
        }
    }

    public class IntDrawer : BasicDrawer<int>
    {
        protected override int DoField(string text, int value)
        {
            return gui.Int(text, value);
        }
    }

    public class DoubleDrawer : BasicDrawer<double>
    {
        protected override double DoField(string text, double value)
        {
            return gui.Float(text, (float)value);
        }
    }

    public class FloatDrawer : BasicDrawer<float>
    {
        protected override float DoField(string text, float value)
        {
            return gui.Float(text, value);
        }
    }

    public class StringDrawer : BasicDrawer<string>
    {
        protected override string DoField(string text, string value)
        {
            return gui.Text(text, value);
        }
    }

    public class Vector2Drawer : BasicDrawer<Vector2>
    {
        protected override Vector2 DoField(string text, Vector2 value)
        {
            return gui.Vector2(text, value);
        }
    }

    public class Vector3Drawer : BasicDrawer<Vector3>
    {
        protected override Vector3 DoField(string text, Vector3 value)
        {
          return gui.Vector3(text, value);
        }
    }

    public class BoolDrawer : BasicDrawer<bool>
    {
        protected override bool DoField(string text, bool value)
        {
            return gui.Toggle(text, value);
        }
    }

    public class ColorDrawer : BasicDrawer<Color>
    {
        protected override Color DoField(string text, Color value)
        {
            return gui.Color(text, value);
        }
    }

    public class BoundsDrawer : BasicDrawer<Bounds>
    {
        protected override Bounds DoField(string text, Bounds value)
        {
            return gui.BoundsField(text, value);
        }
    }

    public class RectDrawer : BasicDrawer<Rect>
    {
        protected override Rect DoField(string text, Rect value)
        {
            return gui.Rect(text, value);
        }
    }

    public class QuaternionDrawer : BasicDrawer<Quaternion>
    {
        protected override Quaternion DoField(string text, Quaternion value)
        {
            return gui.Quaternion(text, value);
        }
    }

    public class UnityObjectDrawer : BasicDrawer<UnityObject>
    {
        public override void OnGUI()
        {
            memberValue = gui.Object(niceName, memberValue, memberType, !AssetDatabase.Contains(unityTarget));
        }
    }

    public class LayerMaskDrawer : BasicDrawer<LayerMask>
    {
        protected override LayerMask DoField(string text, LayerMask value)
        {
            return gui.Layer(text, value);
        }
    }

    public class EnumDrawer : BasicDrawer<Enum>
    {
        protected override Enum DoField(string text, Enum value)
        {
            return gui.EnumPopup(text, value);
        }
    }
}
