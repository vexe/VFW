using UnityEngine;
using System.Collections;
using Vexe.Runtime.Types;
using System;

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
    }

    public class CustomObject
    {
        public string str;
    }

    public class OverrideAttribute : DrawnAttribute
    {
    }
}