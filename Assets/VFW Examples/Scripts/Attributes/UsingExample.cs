using System;
using System.Collections.Generic;
using UnityEngine;
using Vexe.Runtime.Types;

namespace VFWExamples
{
    [Serializable] public class Lookup1 : SerializableDictionary<string, int> { }
    [Serializable] public class Lookup2 : SerializableDictionary<string, KeyCode> { }

    public class UsingExample : BaseBehaviour
    {
        // if we don't mention the class name, we will look
        // inside classes marked with [AttributeContainer]
        [PerItem, Using("MyAttributes")]
        public List<Lookup1> muchLess;

        [Using("MyContainer.MyAttributes")]
        public Lookup2 Clutter;

        [AttributeContainer]
        public static class MyContainer
        {
            public static Attribute[] MyAttributes = new Attribute[]
            {
                new DisplayAttribute(Dict.HorizontalPairs | Dict.Filter),
                new PerKeyAttribute("Popup"),
                new PopupAttribute("MyContainer.GetValues")
                {
                    TextField = true,
                    TakeLastPathItem = true,
                    HideUpdate = true,
                    Filter = true
                },
            };

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
}
