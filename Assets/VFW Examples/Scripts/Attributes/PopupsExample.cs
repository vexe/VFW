using System.Linq;

namespace Vexe.Runtime.Types.Examples
{
	public class PopupsExample : BetterBehaviour
	{
		[Popup("String1", "String2", "String3")]
		public string strings;

		// populate from the method GetFactors - which should return a string[]
		// (access modifier on the method doesn't matter)
		// also use a filter to quickly select values
		[Popup("GetFactors")]
		public string Factor { get; set; }

		[Popup("Counts")]
		public int count;

		// PerItem indicates that the attributes are applied per element, and not on the array
		// in this case, Tags and OnChanged will be applied on each element
		// if the value of any element changes, LogFactor is calling with the new value
		[PerItem, Tags, OnChanged("Log")]
		public string[] EnemyTags { get; set; }

		[Tags]
		public string playerTag;

		// get values from static member from the type 'ItemsDB'
		[PerItem, Popup("ItemsDB.getItemNames", CaseSensitive = false)]
		public string[] items;

		public SomeStruct someObj;

		private string[] GetFactors()
		{
			return new[] { "Ork", "Human", "Undead", "Elves" };
		}

		private int[] Counts
		{
			get { return GetFactors().Select(f => f.Length).ToArray(); }
		}
	}

	public static class ItemsDB // doesn't have to be static
	{
		public static string[] GetItemNames()
		{
			// code that loads items from disk for ex
			// ...
			return new string [] { "Handgun", "LodgeF2Key", "ManholeOpener", "etc" };
		}
	}

	public struct SomeStruct
	{
		[Popup("target.GetFactors")] // get it from the UnityEngine.Object target (in this case, PopupsExample script)
		public string factor;
	}
}