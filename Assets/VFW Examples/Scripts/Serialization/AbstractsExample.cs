using System;
using UnityEngine;

namespace Vexe.Runtime.Types.Examples
{
	
	public class AbstractsExample : BetterBehaviour
	{
		[Serialize]
		private BaseStrategy strategy;

		[Show]
		private void Perform()
		{
			strategy.Perform();
		}

		public abstract class BaseStrategy
		{
			public abstract void Perform();
		}

		public class Flank : BaseStrategy
		{
			[Serialize] private int x;

			public override void Perform()
			{
				sLogFormat("Flanking");
			}
		}

		public class Sweep : BaseStrategy
		{
			[Serialize] private int y;

			public override void Perform()
			{
				sLogFormat("Sweeping");
			}
		}
	}
}