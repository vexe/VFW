using System.Collections.Generic;
using UnityEngine;
using Vexe.Runtime.Types;

namespace VFWExamples
{
	
	public class SequencesExample : BetterBehaviour
	{
		[Comment("Hello, I'm a readonly array, you can't modify me :D")]
		[Seq(SeqOpt.Readonly | SeqOpt.GuiBox)] // or just [Readonly] if you don't want a GuiBox
		public Transform[] transforms;

		// don't forget you attribute 'id' when you're composing
		// in this instance, WhiteSpace has an id of 0, and comment of 1
		// whitespace will be drawn before comment
		[Whitespace(0, Top = 100f), Comment(1, "I'm way down!"), Seq(SeqOpt.Advanced)]
		public List<GameObject> Gos { get; set; }

		[Seq(SeqOpt.GuiBox | SeqOpt.LineNumbers | SeqOpt.PerItemRemove)]
		public List<string> Strings { get; set; }

		// PerItem indicates that you want to apply your attributes on each element
		// and not the array itself
		[PerItem, Tags]
		public string[] tags;

		[PerItem, BetterVector]
		public List<Vector3> BetterVectors { get; set; }

		// Notice here we're not using PerItem, which means any attribute we mention would be applied
		// on the sequence (array/list) itself, and not its elements
		// In this case however, BetterVectorAttribute is an attribute that you use to draw Vector2/3,
		// and not sequences of Vector2/3 which means here, we applied it somewhere we shouldn't
		// so it will get ignored and the sequence will use the regular vector drawer to draw its elements
		[BetterVector]
		public List<Vector3> RegularVectors { get; set; }
	}
}