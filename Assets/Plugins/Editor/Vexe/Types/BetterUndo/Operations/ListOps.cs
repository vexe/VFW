using System;
using System.Collections.Generic;

namespace Vexe.Editor.Types
{
	/// <summary>
	/// A base for generic list operations
	/// </summary>
	public abstract class ListOperation<T> : BasicOp
	{
		/// <summary>
		/// The list getter/setter
		/// </summary>
		public Func<List<T>> GetList { get; set; }

		protected List<T> List
		{
			set { GetList = () => value; }
			get { return GetList(); }
		}
	}

	/// <summary>
	/// List element insertion operation
	/// </summary>
	public class InsertToList<T> : ListOperation<T>
	{
		protected int? index;
		protected T element;

		/// <summary>
		/// The index of which to insert the element at
		/// </summary>
		public virtual int Index { get { return index.Value; } set { index = value; } }

		/// <summary>
		/// The element to insert value
		/// </summary>
		public virtual T Value { get { return element; } set { element = value; } }

		/// <summary>
		/// Performs the insertion operation and executes OnPerformed
		/// </summary>
		public override void Perform()
		{
			List.Insert(Index, Value);
			base.Perform();
		}

		/// <summary>
		/// Undoes the insertion (removes the element) and executes OnUndone
		/// </summary>
		public override void Undo()
		{
			List.RemoveAt(Index - (Index == List.Count ? 1 : 0));
			base.Undo();
		}
	}

	/// <summary>
	/// Acts like InsertToList. Inserts the element at the end of the list
	/// </summary>
	public class AddToList<T> : InsertToList<T>
	{
		public override int Index
		{
			get { return List.Count; }
			set { throw new InvalidOperationException("Can't set index. You only add to the end of the list. If you want to specify an index, use InsertToList instead"); }
		}
	}

	/// <summary>
	/// Removes a single element from a list by means of an index or value
	/// </summary>
	public class RemoveFromList<T> : InsertToList<T>
	{
		/// <summary>
		/// Performs the removal
		/// </summary>
		public override void Perform()
		{
			base.Undo();
		}

		/// <summary>
		/// Undoes the removal (inserts back the element)
		/// </summary>
		public override void Undo()
		{
			base.Perform();
		}

		/// <summary>
		/// The value to remove
		/// </summary>
		public override T Value
		{
			get { return base.Value; }
			set
			{
				if (!index.HasValue)
					index = List.IndexOf(value);
				base.Value = value;
			}
		}

		/// <summary>
		/// The index of the value to remove
		/// </summary>
		public override int Index
		{
			get { return base.Index; }
			set
			{
				if (element == null)
					element = List[value];
				base.Index = value;
			}
		}
	}

	/// <summary>
	/// A list operations that clears a certain range within that list by means of a start index and a count or an end index
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class ClearRange<T> : ListOperation<T>
	{
		private BetterUndo internalUndo = new BetterUndo();

		/// <summary>
		/// The start index
		/// </summary>
		public int Start { get; set; }

		/// <summary>
		/// How many elements to go starting from 'Start'
		/// </summary>
		public int? Count { get; set; }

		/// <summary>
		/// The end index. If Count is defined, it is used and the end index is ignored
		/// </summary>
		public int? End { get; set; }

		private int Length
		{
			get
			{
				if (Count.HasValue)
					return Count.Value;
				if (End.HasValue)
					return (End - Start).Value;
				throw new InvalidOperationException("Couldn't determine the length of the range to clear. Either a Count or an End index has to be specified");
			}
		}

		/// <summary>
		/// Performs the clearing of the range and executes OnPerformed afterwards
		/// </summary>
		public override void Perform()
		{
			for (int i = 0; i < Length; i++)
			{
				internalUndo.RecordRemoveFromList(GetList, Start);
			}
			base.Perform();
		}

		/// <summary>
		/// Undoes the clearning of the range and executes OnUndone afterwards
		/// </summary>
		public override void Undo()
		{
			internalUndo.Undo(Length);
			base.Undo();
		}
	}

	/// <summary>
	/// Clears a whole list - ClearList just inherits ClearRange and sets Count to List.Count when it performs the operation
	/// </summary>
	public class ClearList<T> : ClearRange<T>
	{
		/// <summary>
		/// Performs the clearning and executes OnPerformed afterwards
		/// </summary>
		public override void Perform()
		{
			Count = List.Count;
			base.Perform();
		}
	}
}