using UnityEngine;

namespace Vexe.Runtime.Types.Examples
{ 
	/// <summary>
	/// Use DisplayOrderAttribute to explictly state the order of how you wish your members to appear in
	/// </summary>
	
	public class DisplayOrderExample : BetterBehaviour
	{
		[DisplayOrder(2.5f)] public int three;
		[DisplayOrder(0f)]   public int one;
		[DisplayOrder(10f)]  public int four;
		[DisplayOrder]       public int zero; // if no order is explicitly specified then -1 is used
		[DisplayOrder(2f)]   public int two;
		[DisplayOrder(11)]   public SomeClass someObj;

		public class SomeClass
		{
			[DisplayOrder(0)] public string str;
			[DisplayOrder(1)] public float value;
			[DisplayOrder(2)] public Color color;
		}
	}
}