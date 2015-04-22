using UnityEngine;
using Vexe.Runtime.Types;

namespace VFWExamples
{
	public class FiltersExample : BetterBehaviour
	{
		[FilterEnum]
		public KeyCode jumpKey;

		[FilterTags]
		public string playerTag;

		[Tags, FilterTags]
		public string enemyTag;
	}
}