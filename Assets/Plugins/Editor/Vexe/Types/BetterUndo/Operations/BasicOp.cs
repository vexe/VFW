using System;

namespace Vexe.Editor.Types
{
	/// <summary>
	/// The most basic forms of operations - all it has is two delegates:
	/// an OnPerformed that gets fired after the operation is performed,
	/// and on OnUndone get gets fired after the operation is undone
	/// </summary>
	public class BasicOp : IUndoOp
	{
		/// <summary>
		/// A delegate to invoke after the performing of the operation (in case of a BaseOperation instance, this is all that is ever executed in Perform)
		/// </summary>
		public Action OnPerformed { get; set; }

		/// <summary>
		/// The delegate to invoke after the undoing of the operation (in case of a BaseOperation instance, this is all that is ever executed in Undo)
		/// </summary>
		public Action OnUndone { get; set; }

		/// <summary>
		/// Performs the operation
		/// </summary>
		public virtual void Perform()
		{
			if (OnPerformed != null)
				OnPerformed();
		}

		/// <summary>
		/// Undoes the operation
		/// </summary>
		public virtual void Undo()
		{
			if (OnUndone != null)
				OnUndone();
		}
	}
}