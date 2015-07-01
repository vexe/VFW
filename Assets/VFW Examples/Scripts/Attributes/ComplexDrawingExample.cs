using System.Collections.Generic;
using UnityEngine;
using Vexe.Runtime.Types;

namespace VFWExamples
{
    public class ComplexDrawingExample : BaseBehaviour
    {
        // let's say we have an array of Lookup (custom dictionary of sorts) objects
        // we want to apply Display on the array to give it a custom format
        // but we also want to apply a different Display per item too!
        // in that case PerItem falls short, we need a custom drawer for this task!
        // the custom drawer will be applied on Lookup and will draw it just the way we like!
        // See how the drawer is implemented and mapped in RegisterCustomDrawerExample.cs
        [Display(Seq.LineNumbers | Seq.Filter), PerItem("Whitespace"), Whitespace(Left = 5f)]
        public ItemsLookup[] ComplexArray;
    }

    public class ItemsLookup : SerializableDictionary<string, int>
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

        static string[] GetValues()
        {
            return new string[]
            {
                "Weapons/Berreta", "Weapons/Shotgun", "Weapons/Grenade Launcher", "Weapons/Barry's Gun!", 
                "Key Item/Manhole Opener", "Key Item/Blue Gem", "Key Item/Jill Sandwich", "Key Item/ Golden Emblem",
                "Health/Green Herb", "Health/First Aid Spray", "Health/First Aid Kit", "Health/Syringe", 
                "Ammo/Handgun Ammo", "Ammo/Shotgun Shells", "Ammo/Grenade Rounds", "Ammo/Magnum Ammo", 
            };
        }
    }
}
