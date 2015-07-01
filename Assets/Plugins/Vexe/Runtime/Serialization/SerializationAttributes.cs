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