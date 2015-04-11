using System;

namespace Vexe.Editor.Types
{
	/// <summary>
	/// Implement this interface to create your custom operations
	/// </summary>
	public interface IUndoOp
	{
		/// <summary>
		/// Defines what it is to be done
		/// </summary>
		void Perform();

		/// <summary>
		/// Defines how it is to be undone
		/// </summary>
		void Undo();

		/// <summary>
		/// A convenient delegate that gets executed after the operation has been performed
		/// </summary>
		Action OnPerformed { get; set; }

		/// <summary>
		/// A convenient delegate that gets executed after the operation has been undone
		/// </summary>
		Action OnUndone { get; set; }
	}
}