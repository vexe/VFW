using System;
using Vexe.Runtime.Types;

namespace VFWExamples
{
	public class PopupsExample : BaseBehaviour
	{
        [Popup("String1", "String2", "String3")]
		public string strings;

		// populate from the property 'Factors' - which returns a string[]
		// (access modifier on the method doesn't matter)
		// also use a filter to quickly select values
        // we also tell it to use a text field so we can input values that are not
        // in the popup should we want that
        [Popup("Factors", TextField = true)]
		public string Factor;

		// PerItem indicates that the attributes are applied per element, and not on the array
		// in this case, Tags and OnChanged will be applied on each element
		// if the value of any element changes, LogFactor is calling with the new value
		[PerItem, Tags, OnChanged("Log")]
		public string[] EnemyTags;

		[Tags]
		public string playerTag;

		// get values from static member from the type 'ItemsDB'
        [PerItem, Popup("ItemsDB.getItemNames", CaseSensitive = false)]
		public string[] items;

		public SomeStruct someObj;

		private string[] Factors
		{
			get { return new[] { "Ork", "Human", "Undead", "Elves" }; }
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

    [Serializable]
	public struct SomeStruct
	{
		[Popup("target.Factors")] // get it from the UnityEngine.Object target (in this case, PopupsExample script)
		public string factor;
	}
}
