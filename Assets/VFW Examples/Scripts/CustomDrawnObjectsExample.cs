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

    [Serializable]
    public struct Index2D
    {
        [iMin(0), iMax(10), Comment("[0 -> 10]")]
        public int i;
        [iClamp(0, 10)]
        public readonly int j;
    }
}