using UnityEngine;
using Vexe.Runtime.Types;

namespace VFWExamples
{
	
	public class VisibleWhenExample : BetterBehaviour
	{
		[DisplayOrder(0), Comment(
			"The visibility of the other members are determined by this value. " +
			"< 5 SetBig and smallColor will be visible. " + 
			"> 10 SetSmall and bigColor will be visible. " +
			"]5, 10[ nothing will be visible")]
		public int number;

		[VisibleWhen("IsBig")]
		public Color bigColor;

		[VisibleWhen("IsSmall")]
		public Color smallColor;

		[Show, VisibleWhen("IsSmall")]
		void SetBig()
		{
			number = 100;
		}

		[Show, VisibleWhen("IsBig")]
		void SetSmall()
		{
			number = 0;
		}

		bool IsSmall() { return number < 5; }
		bool IsBig() { return number > 10; }
	}
}
