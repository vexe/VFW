using System;
using Vexe.Runtime.Types;

namespace VFWExamples
{
    /// <summary>
    /// In this example, we use the drawers defined and mapped in RegisterCustomDrawerExample.cs
    /// to customly draw the fields in this behaviour
    /// </summary>
    public class CustomDrawnObjectsExample : BetterBehaviour
    {
        public CustomObject drawn1;

        [Override]
        public CustomObject drawn2;

        public Index2D drawn3;
    }

    public class CustomObject
    {
        public string str;
    }

    public class OverrideAttribute : DrawnAttribute
    {
    }

    // annotating with Serializable is only required because we're using this struct
    // to demonstrate how to draw things in Unity's layout system in DrawnByUnityExample.cs
    // as you probably know, unity's serialization requires the Serializable attribute to be present
    // on custom classes/structs otherwise it won't be able to draw them via its SerializedProperties
    [Serializable]
    public struct Index2D
    {
        [iMin(0), iMax(10), Comment("[0 -> 10]")]
        public int i;
        [iClamp(0, 10)]
        public readonly int j;
    }
}