using System.Collections.Generic;
using UnityEngine;

namespace Vexe.Runtime.Types.Examples
{
	
	public class FormatsExample : BetterBehaviour
	{
		[FormatMember("$name :: $type")]
		public GameObject[] GameObjects { get; set; }

		[FormatMember("$name"), FormatPair("[Key: $key, Value: $value]")]
		public Dictionary<string, Transform> Transforms { get; set; }
	}
}