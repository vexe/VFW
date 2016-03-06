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
	}
}
