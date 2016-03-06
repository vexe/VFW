using UnityEngine;
using System.Collections;
using Vexe.Runtime.Types;
using System.Collections.Generic;

namespace VFWExamples
{ 
    public class CollectionElementExample : BaseBehaviour
    {
        // apply Comment on array
        [Comment("Array 1: Just a comment")]
        public string[] array1;

        // apply Tags on each element
        [PerItem, Tags]
        public string[] array2;

        // apply Tags on each element, and Comment on array
        [PerItem("Tags"), Tags, Comment("Array 3: Comments and stuff!")]
        public string[] array3;

        // apply SelectEnum and FilterEnum on each element, and Comment and Display on list
        [PerItem("SelectEnum", "FilterEnum"), SelectEnum, FilterEnum,
        Comment("List: Comments and more stuff!"), Display("Keys")]
        public List<KeyCode> list;

        // apply Popup on each key, iClamp and Comment on each value and Display on dictionary
        [PerKey("Popup"), Popup("GetNames", TextField = true),
        PerValue("iClamp", "Comment"), iClamp(0, 10), Comment("0 -> 10"),
        Display(Dict.ForceExpand)]
        public Dictionary<string, int> dictionary;

        string[] GetNames()
        {
            return new string[] { "Jon", "Alex", "Ali", "Nic" };
        }
    }
}
