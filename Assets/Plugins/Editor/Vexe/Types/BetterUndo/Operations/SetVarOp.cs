using System;

namespace Vexe.Editor.Types
{
	/// <summary>
	/// Defines a class to change the value of a variable
	/// </summary>
	public class SetVarOp<T> : BasicOp
	{
		/// <summary>
		/// The variable's setter to the new value
		/// </summary>
		public Action<T> SetValue { get; set; }

		/// <summary>
		/// The variable's getter to its current value
		/// </summary>
		public Func<T> GetCurrent { get; set; }

		/// <summary>
		/// The value that we're setting to
		/// </summary>
		public T ToValue { get; set; }

		private T previous;

		/// <summary>
		/// Performs the set operation by calling SetValue passing it ToValue and executes OnPerform afterwards
		/// </summary>
		public override void Perform()
		{
			previous = GetCurrent();
			SetValue(ToValue);
			base.Perform();
		}

		/// <summary>
		/// Undoes the set - setting it back to its previous value and calls OnUndone afterwards
		/// </summary>
		public override void Undo()
		{
			SetValue(previous);
			base.Undo();
		}
	}
}