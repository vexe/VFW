using System.IO;
using BX20Serializer;
using UnityEngine;

namespace Vexe.FastSave.Serializers
{
    public class LightSerializer : StrongSerializer<Light>
    {
        public override void StrongSerialize(Stream stream, Light value)
        {
            LightType type = value.type;
            Write(stream, (int)type);

            switch(type)
            {
                case LightType.Point:
                    Write(stream, value.range);
                    break;
                case LightType.Directional:
                    Write(stream, value.cookieSize);
                    Write(stream, value.shadowStrength);
                    Write(stream, value.shadowBias);
                    break;
                case LightType.Spot:
                    Write(stream, value.spotAngle);
                    Write(stream, value.range);
                    break;
            }

            ColorSerializer.Write(stream, value.color);
            Write(stream, value.intensity);

            if (type != LightType.Area)
            {
                AssetReferenceSerializer.Write(stream, value.cookie);
                AssetReferenceSerializer.Write(stream, value.flare);
                Write(stream, value.cullingMask);
                Write(stream, (int)value.renderMode);
            }
        }

        public override void StrongDeserialize(Stream stream, ref Light instance)
        {
            var type = (LightType)stream.ReadInt();

            switch (type)
            {
                case LightType.Point:
                    instance.range = stream.ReadFloat();
                    break;
                case LightType.Directional:
                    instance.cookieSize     = stream.ReadFloat();
                    instance.shadowStrength = stream.ReadFloat();
                    instance.shadowBias     = stream.ReadFloat();
                    break;
                case LightType.Spot:
                    instance.spotAngle = stream.ReadFloat();
                    instance.range     = stream.ReadFloat();
                    break;
            }

            instance.color = ColorSerializer.Read(stream);
            instance.intensity = stream.ReadFloat();

            if (type != LightType.Area)
            {
                instance.cookie = AssetReferenceSerializer.Read(stream) as Texture;
                instance.flare = AssetReferenceSerializer.Read(stream) as Flare;
                instance.cullingMask = stream.ReadInt();
                instance.renderMode = (LightRenderMode)stream.ReadInt();
            }
        }
    }
}
