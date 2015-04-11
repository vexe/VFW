using System;
using Vexe.Runtime.Extensions;

namespace Vexe.Runtime.Types.Examples
{
	/// <summary>
	/// A demo for ShowTypeAttribute showing how we can overcome the fact that Unity doesn't support
	/// polymorphic serialization for objects (other than UnityEngine.Objects) by having a visible type
	/// object in the inspector to choose the type of our strategy object, and then when our behaviour is created,
	/// we instantiate a strategy based on what type we chose!
	/// ------
	/// UPDATE:
	/// Polymorphic serialization is now supported when using BetterBehaviour so this example is
	/// somewhat obselete - you could just have a TacticStrategy reference and pick an impelemnter (see AbstractsExample.cs)
	/// but I still wanted to demonstrate ShowType usage and that System.Types are serializable...
	/// </summary>
	
	public class ShowTypeExample : BetterBehaviour
	{
		// when the type changes, we call SetStrategy passing the new strategy type
		[ShowType(typeof(TacticStrategy)), OnChanged("SetStrategy")]
		public Type strategyType;

		[Serialize, Hide]
		private TacticStrategy strategy;

		[Show]
		private void Perform()
		{
			strategy.Perform();
		}

		private void SetStrategy(Type type)
		{
			if (type != null)
			{
				LogFormat("Setting strategy to type {0}", type.Name);
				strategy = type.ActivatorInstance<TacticStrategy>();
			}
		}

		[Serializable]
		public abstract class TacticStrategy
		{
			public abstract void Perform();
		}

		[Serializable]
		public class DivideAndConquer : TacticStrategy
		{
			public override void Perform()
			{
				sLogFormat("divide and conquer");
			}
		}

		[Serializable]
		public class FireAtWill : TacticStrategy
		{
			public override void Perform()
			{
				sLogFormat("firing at will");
			}
		}

		[Serializable]
		public class HoldFire : TacticStrategy
		{
			public override void Perform()
			{
				sLogFormat("hodling fire");
			}
		}
	}
}