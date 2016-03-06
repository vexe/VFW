using System;

namespace Vexe.Runtime.Types
{
    [Obsolete("Please stick to Unity's seiralization")]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SerializeAttribute : Attribute
    {
    }

    [Obsolete("Please stick to Unity's seiralization")]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class DontSerializeAttribute : Attribute
    {
    }
}