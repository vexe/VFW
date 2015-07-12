using System.IO;
using UnityEngine;

namespace BX20Serializer
{
    public class ColorSerializer : StrongSerializer<Color>
    {
        public override void StrongSerialize(Stream stream, Color value)
        {
            Write(stream, value);
        }

        public override void StrongDeserialize(Stream stream, ref Color instance)
        {
            instance = Read(stream);
        }

        public static void Write(Stream stream, Color value)
        {
            stream.WriteFloat(value.r);
            stream.WriteFloat(value.g);
            stream.WriteFloat(value.b);
            stream.WriteFloat(value.a);
        }

        public static Color Read(Stream stream)
        {
            return new Color()
            {
                r = stream.ReadFloat(),
                g = stream.ReadFloat(),
                b = stream.ReadFloat(),
                a = stream.ReadFloat(),
            };
        }
    }

    public class V2Serializer : StrongSerializer<Vector2>
    {
        public override void StrongSerialize(Stream stream, Vector2 value)
        {
            stream.WriteFloat(value.x);
            stream.WriteFloat(value.y);
        }

        public override void StrongDeserialize(Stream stream, ref Vector2 instance)
        {
            instance = new Vector2()
            {
                x = stream.ReadFloat(),
                y = stream.ReadFloat(),
            };
        }
    }

    public class V3Serializer : StrongSerializer<Vector3>
    {
        public override void StrongSerialize(Stream stream, Vector3 value)
        {
            Write(stream, value);
        }

        public override void StrongDeserialize(Stream stream, ref Vector3 instance)
        {
            instance = Read(stream);
        }

        public static void Write(Stream stream, Vector3 value)
        {
            stream.WriteFloat(value.x);
            stream.WriteFloat(value.y);
            stream.WriteFloat(value.z);
        }

        public static Vector3 Read(Stream stream)
        {
            return new Vector3()
            {
                x = stream.ReadFloat(),
                y = stream.ReadFloat(),
                z = stream.ReadFloat(),
            };
        }
    }

    public class V4Serializer : StrongSerializer<Vector4>
    {
        public override void StrongSerialize(Stream stream, Vector4 value)
        {
            stream.WriteFloat(value.x);
            stream.WriteFloat(value.y);
            stream.WriteFloat(value.z);
            stream.WriteFloat(value.w);
        }

        public override void StrongDeserialize(Stream stream, ref Vector4 instance)
        {
            instance = new Vector4()
            {
                x = stream.ReadFloat(),
                y = stream.ReadFloat(),
                z = stream.ReadFloat(),
                w = stream.ReadFloat(),
            };
        }
    }

    public class BoundsSerializer : StrongSerializer<Bounds>
    {
        public override void StrongSerialize(Stream stream, Bounds value)
        {
            V3Serializer.Write(stream, value.center);
            V3Serializer.Write(stream, value.size);
        }

        public override void StrongDeserialize(Stream stream, ref Bounds instance)
        {
            instance = new Bounds()
            {
                center = V3Serializer.Read(stream),
                size = V3Serializer.Read(stream)
            };
        }
    }

    public class LayerMaskSerializer : StrongSerializer<LayerMask>
    {
        public override void StrongSerialize(Stream stream, LayerMask layer)
        {
            Write(stream, layer.value);
        }

        public override void StrongDeserialize(Stream stream, ref LayerMask layer)
        {
            layer = stream.ReadInt();
        }
    }

    public class RectSerializer : StrongSerializer<Rect>
    {
        public override void StrongSerialize(Stream stream, Rect rect)
        {
            Write(stream, rect.xMin);
            Write(stream, rect.xMax);
            Write(stream, rect.yMin);
            Write(stream, rect.yMax);
        }

        public override void StrongDeserialize(Stream stream, ref Rect rect)
        {
            rect = new Rect();
            rect.xMin = stream.ReadFloat();
            rect.xMax = stream.ReadFloat();
            rect.yMin = stream.ReadFloat();
            rect.yMax = stream.ReadFloat();
        }
    }

}
