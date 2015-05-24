using System;

namespace Vexe.Runtime.Types
{
	/// <summary>
	/// Annotate a Component or GameObject reference with this to draw it in an inline fashion
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
	public class InlineAttribute : CompositeAttribute
	{
		/// <summary>
		/// Whether or not to show a "Hide" or "Show" button to hide or show the inlined Component/GameObject
		/// Defaults to: False
		/// </summary>
		public bool HideButton;

		/// <summary>
		/// Inline inside a Gui box?
		/// Default: True
		/// </summary>
		public bool GuiBox;

		public InlineAttribute() : this(-1)
		{
		}

		public InlineAttribute(int id) : base(id)
		{
		}
	}
}