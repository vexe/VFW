using System;

namespace Vexe.Runtime.Types
{
	/// <summary>
	/// Annotate a member with this attribute to add vertical space above it by a specified about of pixels
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Parameter)]
	public class WhitespaceAttribute : CompositeAttribute
	{
		/// <summary>
		/// How many space pixels to insert at the top
		/// </summary>
		public float Top    { get; set; }

		/// <summary>
		/// How many space pixels to insert from the left
		/// </summary>
		public float Left   { get; set; }

		/// <summary>
		/// How many space pixels to insert from the right
		/// </summary>
		public float Right  { get; set; }

		/// <summary>
		/// How many space pixels to insert at the bottom
		/// </summary>
		public float Bottom { get; set; }

		public WhitespaceAttribute(int id) : base(id)
		{
		}

		public WhitespaceAttribute() : this(-1)
		{
		}
	}
}