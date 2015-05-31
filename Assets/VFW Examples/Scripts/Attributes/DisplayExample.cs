using System.Collections.Generic;
using UnityEngine;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Types;

namespace VFWExamples
{
    /// <summary>
    /// An example showing how to display things differently
    /// </summary>
    public class DisplayExample : BetterBehaviour
    {
        [Display("Classified!!!")]
        public int secret;

        // to customize the display of an object field, just override ToString and return the displayed text
        public Health hp;

        // same applies to array/list elements
        public ItemInfo[] items;

		[Display("$name :: $type")]
		public GameObject[] gameObjs { get; set; }

		[Display("$name", FormatKVPair = "[Key: $key, Value: $value]")]
		public Dictionary<string, Transform> Transforms { get; set; }

        public DisplayOrderExample example = new DisplayOrderExample();

        [PerItem, Display(FormatMethod = "FormatArrayElement")]
        public Component[] array;

        string FormatArrayElement(Component value)
        {
            return value == null ? "Null" : value.GetType().GetNiceName();
        }
    }

    public class Health
    {
        public int Min, Max, Current;

        public override string ToString()
        {
            return "Min: {0}, Current: {1}, Max: {2}"
                .FormatWith(Min, Max, Current);
        }
    }

    public struct ItemInfo
    {
        public uint Cost;
        public string Description;

        public override string ToString()
        {
            return Description + " (Costs " + Cost + " gold)";
        }
    }

	/// <summary>
	/// An example showing how to explictly state the order that members are displayed in
	/// </summary>
	public struct DisplayOrderExample
	{
		[Display(2.5f)] public int three;
		[Display(-1)]   public int one;
		[Display(10f)]  public int four;
		[Display(-2)]   public int zero;
		[Display(0f)]   public int two;
		[Display(11f)]  public Five five;

		public struct Five
		{
			[Display(1f)] public float seven;
			[Display(0f)] public string six;
			[Display] public Color eight; // if no order is specified the member will be drawn in the remaining order
            public double nine;
            public string ten;
		}
	}
}
