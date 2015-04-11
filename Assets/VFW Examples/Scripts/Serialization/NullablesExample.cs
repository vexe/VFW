using System.Collections.Generic;
using UnityEngine;

namespace Vexe.Runtime.Types.Examples
{
	
	public class NullablesExample : BetterBehaviour
	{
		[Whitespace(Top = 20f)]
		public int? nullableIntField;

		[Whitespace(Left = 40f)]
		public bool? nullableBoolProperty { get; set; }

		[Comment("this is an array with nullable floats")]
		public float?[] nullableFloatArray;

		[PerValue, OnChanged(Set = "position")]
		public Dictionary<string, Vector3?> nullableDictionary;
	}
}