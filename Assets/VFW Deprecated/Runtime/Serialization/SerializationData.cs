using System;
using System.Collections.Generic;
using Vexe.Runtime.Types;
using UnityObject = UnityEngine.Object;

namespace Vexe.Runtime.Serialization
{
    /// <summary>
    /// A serializable (by Unity) class that holds the serialization data for a BetterBehaviour/BetterScriptableObject
    /// </summary>
    [Serializable]
    public class SerializationData
    {
        /// We let Unity handle the serialization of its own types so whenever we come across a Unity object we store it
        /// in the 'serializedObjects' list and serialize the index of where that storage took place.
        [Display(Seq.Readonly)]
        public List<UnityObject> serializedObjects = new List<UnityObject>();

        /// A serializable key-value pair list data structure that holds the serialized values of members
        /// in a BetterBehaviour/BetterScriptableObject.  Let's say we had a int field called 'x' = 10,
        /// then we'd have an entry with "int x" as key and '10' as value.
        [Display(Dict.Readonly)]
        public StrStrDict serializedStrings = new StrStrDict();

        public void Clear()
        {
            serializedObjects.Clear();
            serializedStrings.Clear();
        }

        public override string ToString()
        {
            return "Runtime Serialization Data";
        }
    }

    [Serializable]
    public class StrStrDict : KVPList<string, string>
    {
    }
}
