using System.Collections.Generic;
using UnityEngine;
using Vexe.Runtime.Types;

namespace VFWExamples
{
	
	public class FormatsExample : BetterBehaviour
	{
		[FormatMember("$name :: $type")]
		public GameObject[] GameObjects { get; set; }

		[FormatMember("$name"), FormatPair("[Key: $key, Value: $value]")]
		public Dictionary<string, Transform> Transforms { get; set; }
	}
}