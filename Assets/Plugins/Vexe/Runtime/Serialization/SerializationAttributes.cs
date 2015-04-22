using System;
using UnityEngine;
using Vexe.Runtime.Types;

namespace Vexe.Runtime.Types
{
    /// <summary>
    /// Anntoate non-public fields/auto-properties with this attribute to tell that they must be serialized
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SerializeAttribute : Attribute
    {
    }

    /// <summary>
    /// Anntoate fields/auto-properties with this attribute to tell that they will *not* be serialized
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class DontSerializeAttribute : Attribute
    {
    }
}

namespace Vexe.Runtime.Serialization
{
    /// <summary>
    /// Contains Type arrays for attributes that help determine if a member or type is serializable or not
    /// </summary>
    public class SerializationAttributes
    {
        /// <summary>
        /// Tells that a member (fields, auto-properties) must be serialized
        /// </summary>
        public readonly Type[] SerializeMember;

        /// <summary>
        /// Tells that a member (fields, auto-properties) will not be serialized
        /// </summary>
        public readonly Type[] DontSerializeMember;

        /// <summary>
        /// Tells that a type is serializable
        /// </summary>
        public readonly Type[] SerializableType;

        public SerializationAttributes(Type[] serializeMember, Type[] dontSerializeMember, Type[] serializableType)
        {
            this.SerializeMember     = serializeMember;
            this.DontSerializeMember = dontSerializeMember;
            this.SerializableType    = serializableType;
        }
    }
}
