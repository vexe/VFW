using UnityEngine;

namespace Vexe.Runtime.Types.Examples
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