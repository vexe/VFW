using System;

namespace Vexe.Runtime.Types
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class DisplayAttribute : ShowAttribute
    {
        private float _order;

        /// <summary>
        /// Specifies the drawing/display order of a member
        /// </summary>
        public float Order
        {
            get { return _order; }
            set
            {
                _order = value;
                DisplayOrder = value;
            }
        }

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
        /// Specifies a method to use that returns a string representing the format label of the annotated member
        /// Just like FormatLabel but gives you more control. See DisplayExample.cs
        /// The method must return string and take a single parameter of the same member type
        /// Note this doesn't take any patterns into consideration ($name etc).
        /// The exact return value is used as display text
        /// </summary>
        public string FormatMethod;

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

        public float? DisplayOrder;

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

        /// <summary>
        /// Whether or not the dictionary is readonly (non-editable in the inspector)
        /// </summary>
        Readonly = 1,

        /// <summary>
        /// Forceed expansion of the dictionary header foldout?
        /// </summary>
        ForceExpand = 1 << 1,

        /// <summary>
        /// Hide the dictionary header?
        /// </summary>
        HideHeader = 1 << 2,

        /// <summary>
        /// Display pairs in a single horizontal block?
        /// </summary>
        HorizontalPairs = 1 << 5,

        /// <summary>
        /// Show a search box to filter elements? (uses element.ToString() when matching)
        /// </summary>
        Filter = 1 << 6,

        /// <summary>
        /// Should new pairs be added to the end of the dictionary as opposed to inserting them at the beginning?
        /// (same applies to removing them as well)
        /// </summary>
        AddToLast = 1 << 7,

        /// <summary>
        /// Show a temporary adding area for keys? (enter key and hit return to add a new pair with that key value)
        /// </summary>
        TempKey = 1 << 8,
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

        /// <summary>
        /// Show a search box to filter elements? (uses element.ToString() when matching)
        /// </summary>
        Filter = 1 << 6,
    }
}
