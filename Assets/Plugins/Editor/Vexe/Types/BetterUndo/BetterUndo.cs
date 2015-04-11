using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Vexe.Editor.Types
{
	/// <summary>
	/// A better undo system implemented using the command-pattern http://www.oodesign.com/command-pattern.html
	/// With it, you could specify exactly 'what' is it to be done, and 'how' is it to be undone!
	/// If you want to use it in your editor scirpts and be able to undo by a menu item (Undo = Ctrl+Alt+u, Redo = Ctrl+Alt+r)
	/// you have to make the BetterUndo instance that you're using the current instance, that way it could be accessed statically
	/// from the menu item.
	/// ex:
	/// BetterUndo _undo = new BetterUndo();
	/// BetterUndo undo { get { return BetterUndo.MakeCurrent(ref _undo); } }
	/// From this point on, you could use 'undo.Whatever(...);' and if you press the undo/redo menu items, undo/redo will occur
	/// 
	/// One might ask, why not use SerializedProperties since they automatically handle undo/redo?
	/// That's true, but a lot of the time using SerializedProperties isn't convenient.
	/// Consider the case where you have a List of `T` where T is _not_ a UnityEngine.Object,
	/// first of all, now you can't access the list's elements via a SerializedProperty cause they're not UnityEngine.Objects
	/// most the times you'd end up using reflection (in case of PropertyDrawers)
	/// or accessing the list directly from 'target' (in case of CustomEditors)
	/// and manipulating the list directly... But then you'd have to handle undo yourself.
	/// For that you'd normally use UnityEditor.Undo which is not bad, but it fails short when you have a complex operation
	/// Say each element in that list, had an innert list too, so when you want to change an element of your list
	/// you'd want to clear out that inner list! - Using UE.Undo, might cut it or not - depending on the quality of its mood =))
	/// 
	/// UE.Undo is the reason why I couldn't provide good Undo support for uFAction in the inital release,
	/// with BetterUndo, uFAction now has excellent undo support!
	/// 
	/// And not just that, BetterUndo is available for you at runtime as well, so you could use it in your games too!
	/// 
	/// Only down side currently is that the stacks don't serialize which mean a BetterUndo instance will only last one editor session.
	/// </summary>
	public class BetterUndo
	{
		private Stack<IUndoOp> undoStack;
		private Stack<IUndoOp> redoStack;

		private static BetterUndo current;

		/// <summary>
		/// Returns the current active BetterUndo instance
		/// </summary>
		public static BetterUndo Current { get { return current ?? (current = new BetterUndo()); } }

		/// <summary>
		/// Returns an array of all the operations currently in the undo stack
		/// </summary>
		public IUndoOp[] UndoStackOps { get { return undoStack.ToArray(); } }

		/// <summary>
		/// Returns an array of all the operations currently in the redo stack
		/// </summary>
		public IUndoOp[] RedoStackOps { get { return redoStack.ToArray(); } }

		/// <summary>
		/// Returns how many operations are currently in the undo stack
		/// </summary>
		public int UndoStackLength { get { return undoStack.Count; } }

		/// <summary>
		/// Returns how many operations are currently in the redo stack
		/// </summary>
		public int RedoStackLength { get { return redoStack.Count; } }

		/// <summary>
		/// Makes the passed ref undo object the current undo if it wasn't, and returns it afterwards.
		/// Use this in your editor scripts to be able to undo/redo via the menu items (Ctrl+Alt+u, Ctrl+Alt+r)
		/// ex:
		/// private BetterUndo _undo;
		/// public BetterUndo undo { get { return BetterUndo.MakeCurrent(ref _undo); } }
		/// And then from this point on, you just use 'undo' - that way this instace becomes the current undo,
		/// and thus available to be accessed from the menu items.
		/// </summary>
		public static BetterUndo MakeCurrent(ref BetterUndo undo)
		{
			if (current != undo)
				current = undo;
			return undo;
		}

		/// <summary>
		/// Creates a BetterUndo instance. Allocates new memory for the undo/redo stacks.
		/// </summary>
		public BetterUndo()
		{
			undoStack = new Stack<IUndoOp>();
			redoStack = new Stack<IUndoOp>();
		}

		/// <summary>
		/// Registers the specified operation (pushes it to the undo stack, and clears the redo stack)
		/// </summary>
		public void Register(IUndoOp op)
		{
			undoStack.Push(op);
			redoStack.Clear();
		}

		/// <summary>
		/// Registers the specified operation and performs it right after
		/// </summary>
		public void RegisterThenPerform(IUndoOp op)
		{
			Register(op);
			op.Perform();
		}

		/// <summary>
		/// Performs a single undo operation
		/// </summary>
		public void Undo()
		{
			Undo(1);
		}

		/// <summary>
		/// Performs a specified number of undo operations
		/// </summary>
		public void Undo(int nTimes)
		{
			if (!AssertStack(nTimes, undoStack, "undo")) return;

			for (int i = 0; i < nTimes; i++)
			{
				var op = undoStack.Pop();
				op.Undo();
				redoStack.Push(op);
			}
		}

		/// <summary>
		/// Performs the specified number of redo operations
		/// </summary>
		public void Redo(int nTimes)
		{
			if (!AssertStack(nTimes, redoStack, "redo")) return;

			for (int i = 0; i < nTimes; i++)
			{
				var op = redoStack.Pop();
				op.Perform();
				undoStack.Push(op);
			}
		}

		/// <summary>
		/// Performs a single redo operation
		/// </summary>
		public void Redo()
		{
			Redo(1);
		}

		private bool AssertStack(int nTimes, Stack<IUndoOp> stack, string stackName)
		{
			if (nTimes > stack.Count)
			{
				Debug.Log(string.Format("Can't {0}: {1}",
					stackName, stack.Count == 0 ?
								string.Format("{0} stack is empty", stackName) :
								string.Format("Number of times to {0} is higher than the current length of the {0} stack",
								stackName)));
				return false;
			}
			return true;
		}

		// Helper methods
		#region

		/// <summary>
		/// Records (registers then performs) a ClearRange operation on the given list getter from 'start' counting up 'count' number of times
		/// </summary>
		public void RecordClearRangeFromBy<T>(Func<List<T>> getList, int start, int count, Action onPerformed = null, Action onUndone = null)
		{
			RegisterThenPerform(new ClearRange<T> { GetList = getList, Start = start, Count = count, OnPerformed = onPerformed, OnUndone = onUndone });
		}

		/// <summary>
		/// Records (registers then performs) a ClearRange operation on the given list getter from 'start' to 'end'
		/// </summary>
		public void RecordClearRangeFromTill<T>(Func<List<T>> getList, int start, int end, Action onPerformed = null, Action onUndone = null)
		{
			RecordClearRangeFromBy(getList, start, end - start, onPerformed, onUndone);
		}

		/// <summary>
		/// Records (registers then performs) a ClearRange operation on the given list getter from 'start' to the end of the list
		/// </summary>
		public void RecordClearRangeFromTillEnd<T>(Func<List<T>> getList, int start, Action onPerformed = null, Action onUndone = null)
		{
			RecordClearRangeFromTill(getList, start, getList().Count, onPerformed, onUndone);
		}

		/// <summary>
		/// Records (registers then performs) a ClearList operation on the given list getter
		/// </summary>
		public void RecordClearList<T>(Func<List<T>> getList, Action onPerformed = null, Action onUndone = null)
		{
			// Or could have just used the range overloads above...
			RegisterThenPerform(new ClearList<T> { GetList = getList, OnPerformed = onPerformed, OnUndone = onUndone });
		}

		/// <summary>
		/// Records (registers then performs) a SetVariable operation on the given variable getter/setter to set it the specified value 'toValue'
		/// </summary>
		public void RecordSetVariable<T>(Func<T> get, Action<T> set, T toValue, Action onPerformed = null, Action onUndone = null)
		{
			RegisterThenPerform(new SetVarOp<T> { GetCurrent = get, SetValue = set, ToValue = toValue, OnPerformed = onPerformed, OnUndone = onUndone });
		}

		/// <summary>
		/// Records (registers then performs) a SetVariable operation on the given variable whose value is denoted by 'current' to set it via the specified setter to the specified value 'toValue'
		/// </summary>
		public void RecordSetVariable<T>(T current, Action<T> set, T toValue, Action onPerformed = null, Action onUndone = null)
		{
			RecordSetVariable(() => current, set, toValue, onPerformed, onUndone);
		}

		/// <summary>
		/// Records (registers then performs) a RemoveFromList operation on the specified list getter at the specified index
		/// </summary>
		public void RecordRemoveFromList<T>(Func<List<T>> getList, int index, Action onPerformed = null, Action onUndone = null)
		{
			RegisterThenPerform(new RemoveFromList<T> { GetList = getList, Index = index, OnPerformed = onPerformed, OnUndone = onUndone });
		}

		/// <summary>
		/// Records (registers then performs) a RemoveFromList operation on the specified list getter, removing 'element'
		/// </summary>
		public void RecordRemoveFromList<T>(Func<List<T>> getList, T element, Action onPerformed = null, Action onUndone = null)
		{
			RecordRemoveFromList(getList, getList().IndexOf(element), onPerformed, onUndone);
		}

		/// <summary>
		/// Records (registers then performs) an InsertToList operation on the specified list getter at the specified index inserting 'value'
		/// </summary>
		public void RecordInsertToList<T>(Func<List<T>> getList, int atIndex, T value, Action onPerformed = null, Action onUndone = null)
		{
			RegisterThenPerform(new InsertToList<T> { GetList = getList, Index = atIndex, Value = value, OnPerformed = onPerformed, OnUndone = onUndone });
		}

		/// <summary>
		/// Records (registers then performs) an AddToList operation on the specified list getter, adding (inserting to last) 'value'
		/// </summary>
		public void RecordAddToList<T>(Func<List<T>> getList, T value, Action onPerformed = null, Action onUndone = null)
		{
			RegisterThenPerform(new AddToList<T> { GetList = getList, Value = value, OnPerformed = onPerformed, OnUndone = onUndone });
		}

		/// <summary>
		/// Records (registers then performs) a BasicOperation operation using the specified actions
		/// </summary>
		public void RecordBasicOp(Action onPerformed, Action onUndone)
		{
			RegisterThenPerform(new BasicOp { OnPerformed = onPerformed, OnUndone = onUndone });
		}

		public void RecordSelection(UnityObject[] toSelect, UnityObject[] toGoBackTo, Action onPerformed = null, Action onUndone = null)
		{
			RegisterThenPerform(new SelectionOp { ToSelect = toSelect, ToGoBackTo = toGoBackTo, OnPerformed = onPerformed, OnUndone = onUndone });
		}
		
		public void RecordSelection(UnityObject toSelect, UnityObject toGoBackTo, Action onPerformed = null, Action onUndone = null)
		{
			RegisterThenPerform(new SelectionOp { ToSelect = new UnityObject[] { toSelect }, ToGoBackTo = new UnityObject[] { toGoBackTo }, OnPerformed = onPerformed, OnUndone = onUndone });
		}
		#endregion

		public static class MenuItems
		{
			private const string MenuPath = "Tools/Vexe/BetterUndo";
			private static BetterUndo current { get { return BetterUndo.Current; } }

			[MenuItem(MenuPath + "/Undo %&u")]
			public static void Undo()
			{
				current.Undo();
			}

			[MenuItem(MenuPath + "/Redo %&r")]
			public static void Redo()
			{
				current.Redo();
			}

			[MenuItem(MenuPath + "/Print Undo stack length")]
			public static void PrintUndoStackLength()
			{
				Debug.Log(current.UndoStackLength);
			}
		}
	}
}