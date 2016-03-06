using System;
using System.Collections.Generic;
using FullSerializer;
using UnityObject = UnityEngine.Object;

namespace Vexe.Runtime.Serialization
{ 
    /// <summary>
    /// An adaptor/proxy to add support for the JSON serializer FullSerializer
    /// </summary>
    public class FullSerializerBackend : SerializerBackend
    {
        public static readonly fsSerializer Serializer;

        public FullSerializerBackend()
        {
            Logic = VFWSerializationLogic.Instance;
        }

        static FullSerializerBackend()
        {
            Serializer = new fsSerializer();
            Serializer.AddConverter(new UnityObjectConverter());
            Serializer.AddConverter(new MethodInfoConverter());

            fsConfig.SerializeAttributes = VFWSerializationLogic.Instance.SerializeMember;
            fsConfig.IgnoreSerializeAttributes = VFWSerializationLogic.Instance.DontSerializeMember;
        }

        public override string Serialize(Type type, object value, object context)
        {
            Serializer.Context.Set(context as List<UnityObject>);

            fsData data;
            var fail = Serializer.TrySerialize(type, value, out data);
            if (fail.Failed) throw new Exception(fail.FormattedMessages);

            return fsJsonPrinter.CompressedJson(data);
        }

        public override object Deserialize(Type type, string serializedState, object context)
        {
            fsData data;
            fsResult status = fsJsonParser.Parse(serializedState, out data);
            if (status.Failed) throw new Exception(status.FormattedMessages);

            Serializer.Context.Set(context as List<UnityObject>);

            object deserialized = null;
            status = Serializer.TryDeserialize(data, type, ref deserialized);
            if (status.Failed) throw new Exception(status.FormattedMessages);
            return deserialized;
        }
    }
}
