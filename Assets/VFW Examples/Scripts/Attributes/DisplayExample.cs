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

        [Display(FormatMethod = "FormatArray")]
        public string[] array;

        // gets called from editor
        string FormatArray()
        {
            return "Array of string (" + array.Length + ")";
        }

        [Display(FormatMethod = "FormatDictionary")]
        public Dictionary<string, float> dictionary;

        string FormatDictionary()
        {
            return "Lookup (Count: " + dictionary.Count + ")";
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
	public class DisplayOrderExample
	{
		[Display(2.5f)] public int three;
		[Display]       public int one; // if no order is explicitly specified then 0 is used
		[Display(10f)]  public int four;
		[Display(-1)]   public int zero;
		[Display(2f)]   public int two;
		[Display(11)]   public Five five;

		public struct Five
		{
			[Display(1f)] public float seven;
			[Display] public string six;
			[Display(2f)] public Color eight;
		}
	}
}
