using System;

namespace Vexe.Runtime.Types
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class DisplayAttribute : Attribute
    {
        /// <summary>
        /// Specifies the drawing/display order of a member
        /// </summary>
        public float Order;

        /// <summary>
        /// Specifies a pattern to format the text display of a member
        /// Possible patterns (case-sensitive): "$type", "$nicetype", "$name", "$nicename"
        /// For ex, given an integer named 'myInt' and a pattern of "$nicename ($nicetype)"
        /// the resulting display text would be: "My Int (int)"
        /// You can set default formatting values to apply to all future members in VFWSettings
        /// (Click Tools | Vexe | VFWSettings and that will take you to the settings asset)
        /// </summary>
        public string FormatLabel;

        /// <summary>
        /// Specifies a pattern to format the text display of a key-value pair
        /// Possible patterns (case-sensitive): "$key", "$keytype", "$value", "$valuetype"
        /// See FormatsExample.cs for an example
        /// </summary>
        public string FormatKVPair;

        /// <summary>
        /// Customizes the display of sequences (arrays/lists)
        /// </summary>
        public Seq SeqOpt;

        /// <summary>
        /// Customizes the display of dictionaries
        /// </summary>
        public Dict DictOpt;

        public DisplayAttribute()
        {
        }

        public DisplayAttribute(Seq seqOpt)
        {
            this.SeqOpt = seqOpt;
        }

        public DisplayAttribute(Dict dictOpt)
        {
            this.DictOpt = dictOpt;
        }

        public DisplayAttribute(float order)
        {
            this.Order = order;
        }

        public DisplayAttribute(string formatLabel)
        {
            this.FormatLabel = formatLabel;
        }
    }

    [Flags]
    public enum Dict
    {
        None = 0,
        Readonly = 1,
        ForceExpand = 1 << 1,
        HideHeader = 1 << 2,
    }

	[Flags]
	public enum Seq
	{
		/// <summary>
		/// No customization applied whatsoever.
		/// </summary>
		None = 0,

		/// <summary>
		/// Whether or not to show advanced controls (shuffle, randomize, shift, set new size etc)
		/// </summary>
		Advanced = 1,

		/// <summary>
		/// Whether or not to show line numbers beside elements
		/// </summary>
		LineNumbers = 1 << 1,

		/// <summary>
		/// Whether or not the sequence is readonly (non-editable in the inspector)
		/// </summary>
		Readonly = 1 << 2,

		/// <summary>
		/// Whether to show only one remove button that removes the last element or one foreach element
		/// </summary>
		PerItemRemove = 1 << 3,

		/// <summary>
		/// Whether or not to draw elements in a GUI box
		/// </summary>
		GuiBox = 1 << 4,

		/// <summary>
		/// Whether or not to allow adding duplicate items
		/// </summary>
		UniqueItems = 1 << 5,
	}
}