using System.Collections.Generic;
using UnityEngine;
using Vexe.Runtime.Types;

namespace VFWExamples
{
	public class SequencesExample : BaseBehaviour
	{
		[Comment("Hello, I'm a readonly array, you can't modify me :D")]
		[Display(Seq.Readonly | Seq.GuiBox)] // or just [Readonly] if you don't want a GuiBox
		public Transform[] Transforms;

		// don't forget you attribute 'id' when you're composing
		// in this instance, WhiteSpace has an id of 0, and comment of 1
		// whitespace will be drawn before comment
		[Whitespace(0, Top = 100f), Comment(1, "I'm way down!"), Display(Seq.Advanced)]
		public List<GameObject> Gos;

		[Display(Seq.GuiBox | Seq.LineNumbers | Seq.PerItemRemove)]
		public List<string> Strings;

		// PerItem indicates that you want to apply your attributes on each element
		// and not the array itself
		[PerItem, BetterV3]
		public List<Vector3> BetterVectors;

        // Here, we're telling PerItem that we only want to apply 'Tags' per item
        // leaving Comment to be applied on the array itself
        // this gives you more control how you want your attributes to be applied
        // for more details see CollectionElementExample.cs
		[PerItem("Tags"), Tags, Comment("This comment is applied on the array")]
		public string[] Tags;

		// Notice here we're not using PerItem, which means any attribute we mention would be applied
		// on the sequence (array/list) itself, and not its elements
		// In this case however, BetterV3 is an attribute that you use to draw a Vector3,
		// and not sequences of Vector3 which means here we applied it somewhere we shouldn't
		// so we will get an error saying BetterV3 can't handle List<Vector3>.
        // After that we'll fallback to the drawer of List<Vector3> which is ListDrawer<T>
        // Uncomment to give it a try!
        //public List<Vector3> RegularVectors { get; set; }
	}
}
