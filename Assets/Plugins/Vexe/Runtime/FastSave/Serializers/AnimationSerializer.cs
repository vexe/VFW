using System.IO;
using BX20Serializer;
using UnityEngine;

namespace Vexe.FastSave.Serializers
{ 
    public class AnimationSerializer : StrongSerializer<Animation>
    {
        public override void StrongSerialize(Stream stream, Animation value)
        {
            Write(stream, value.playAutomatically);
            Write(stream, (int)value.wrapMode);

            var clipCount = value.GetClipCount();
            Write(stream, clipCount);

            foreach(AnimationState anim in value)
                AssetReferenceSerializer.Write(stream, anim.clip);
        }

        public override void StrongDeserialize(Stream stream, ref Animation instance)
        {
            instance.playAutomatically = stream.ReadBool();
            instance.wrapMode          = (WrapMode)stream.ReadInt();

            int clipCount = stream.ReadInt();

            for (int i = 0; i < clipCount; i++)
            {
                var clip = AssetReferenceSerializer.Read(stream) as AnimationClip;
                if (instance.GetClip(clip.name) == null)
                    instance.AddClip(clip, clip.name);
            }
        }
    }
}
