using UnityEngine;
using System.Collections;
using Vexe.Runtime.Types;
using System.Collections.Generic;

namespace VFWExamples
{
    public class ComplexDrawingExample : BetterBehaviour
    {
        // let's say we have an array of Lookup (custom dictionary of sorts) objects
        // we want to apply Display on the array to give it a custom format
        // but we also want to apply a different Display per item too!
        // in that case PerItem falls short, we need a custom drawer for this task!
        // the custom drawer will be applied on Lookup and will draw it just the way we like!
        // See how the drawer is implemented and mapped in RegisterCustomDrawerExample.cs
        [Display(Seq.LineNumbers | Seq.Filter), PerItem("Whitespace"), Whitespace(Left = 5f)]
        public Lookup[] ComplexArray;

        // although we haven't specified a FormatMethod, Display is smart enough to find this method
        // and use it to format the Lookups array cause it uses the naming convention: FormatX where X is the member name
        string FormatComplexArray() { return "Complex Array (" + ComplexArray.Length + ")"; }

        public class Lookup : Dictionary<string, int>
        {
            public new int this[string key]
            {
                set { base[key] = value; }
                get
                {
                    int value;
                    if (!TryGetValue(key, out value))
                        return 0;
                    return value;
                }
            }

            public override string ToString()
            {
                return "Lookup (" + Count + ")";
            }
        }
    }
}
