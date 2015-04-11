using System;
using UnityEngine;
using Vexe.Runtime.Types;

namespace Vexe.Runtime.Types
{
    /// <summary>
    /// A shorter alternative to SerializeField - applicable to fields and properties
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SerializeAttribute : Attribute
    {
    }

    /// <summary>
    /// Fields/auto-properties annotated with this don't get serialized
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class DontSerializeAttribute : Attribute
    {
    }
}

namespace Vexe.Runtime.Serialization
{
    public class SerializationAttributes
    {
        public Type[] SerializeMember;
        public Type[] DontSerializeMember;
        public Type[] SerializableType;

        public static readonly SerializationAttributes Default = new SerializationAttributes()
        {
            SerializeMember = new[]
            {
                typeof(SerializeField),
                typeof(SerializeAttribute),
            },

            DontSerializeMember = new[]
            {
                typeof(NonSerializedAttribute),
                typeof(DontSerializeAttribute),
            },

            // since we're only using FullSerializer now, we don't need to annotate types with special attributes to serialize them
            SerializableType = new Type[0]
            {
            },
        };
    }
}