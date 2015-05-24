using System;
using System.Linq;
using UnityEngine;

namespace Vexe.Runtime.Types
{
	/// <summary>
	/// Annotate a string with this attribute to have its value selected from a popup
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
	public class PopupAttribute : DrawnAttribute
	{
		/// <summary>
		/// Use this if you want to dynamically generate the popup values instead of having to hardcode them
		/// This could be a method with no parameters, and a return type of an array of string, float or int
		/// or a field/property with the right return type
		/// </summary>
		public string PopulateFrom;

		/// <summary>
		/// Is the 'PopulateFrom' member name case sensitive? (Defaults to false)
		/// </summary>
		public bool CaseSensitive;

        /// <summary>
        /// Use a text field dropdown/popup? (same one used setting up Mecanim's conditions)
        /// </summary>
        public bool TextField;

        /// <summary>
        /// Take the last entry in a path string? e.g. One/Two/Three -> Three
        /// </summary>
        public bool TakeLastPathItem;

        /// <summary>
        /// Hide update 'U' button?
        /// </summary>
        public bool HideUpdate;

        /// <summary>
        /// Show a search text box to filter values?
        /// </summary>
        public bool Filter;

		/// <summary>
		/// The popup values
		/// </summary>
		public readonly string[] values;

		public PopupAttribute(string populateFrom)
		{
			PopulateFrom = populateFrom;
		}

		public PopupAttribute(params string[] strings)
		{
			values = strings;
		}
	}
}
