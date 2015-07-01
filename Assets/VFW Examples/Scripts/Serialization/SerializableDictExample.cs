using System;
using UnityEngine;
using Vexe.Runtime.Types;

// Use SerializableDictionary in BaseBehaviour - when you don't want custom serialization but
// you still want to be able to serialize dictionaries
public class SerializableDictExample : BaseBehaviour
{
    public Lookup lookup = new Lookup();

    [Serializable]
    public class Lookup : SerializableDictionary<string, GameObject> { }
}
