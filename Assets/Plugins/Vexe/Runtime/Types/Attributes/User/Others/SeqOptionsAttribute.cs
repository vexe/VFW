using System;

namespace Vexe.Runtime.Types
{
	/// <summary>
	/// Annotate sequences (list/array) with this to customize the way they're displayed
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
	public class SeqAttribute : Attribute
	{
		public readonly SeqOpt options;

		public SeqAttribute(SeqOpt options)
		{
			this.options = options;
		}
	}

	[Flags]
	public enum SeqOpt
	{
		/// <summary>
		/// No customization applied whatsoever.
		/// Attributes are taken into consideration when drawing elements
		/// This is the default option if you don't annotate with SeqOptions
		/// </summary>
		None = 0,

		/// <summary>
		/// Whether or not to show advanced controls
		/// </summary>
		Advanced = 1 << 0,

		/// <summary>
		/// Whether or not to show line numbers beside elements
		/// </summary>
		LineNumbers = 1 << 1,

		/// <summary>
		/// Whether or not the sequence is readonly
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