using System;
using System.Collections.Generic;
using UnityEditor;
using Vexe.Runtime.Extensions;
using UnityObject = UnityEngine.Object;

namespace Vexe.Editor.Others
{
	/// <summary>
	/// A simple utility class that memorizes object selection
	/// Press Ctrl+Shift+- to go back, Ctrl+Shift+= to go forward
	/// </summary>
	[InitializeOnLoad]
	public static class SelectionMemorizer
	{
		private static bool _isRunning;
		private static UnityObject[] _previous = new UnityObject[1] { null };
		private static Stack<SelOp> _undo = new Stack<SelOp>();
		private static Stack<SelOp> _redo = new Stack<SelOp>();

		const string MenuPath = "Tools/Vexe/SelectionMemorizer";

		static SelectionMemorizer()
		{
			ToggleActive();
		}

		[MenuItem(MenuPath + "/Toggle StartStop")]
		public static void ToggleActive()
		{
			if (_isRunning)
				EditorApplication.update -= Update;
			else
				EditorApplication.update += Update;
			_isRunning = !_isRunning;
		}

		[MenuItem(MenuPath + "/Select Last Object (Back) %#-")]
		public static void Back()
		{
			if (_undo.Count == 0)
				return;

			var op = _undo.Pop();
			op.Undo();
			_redo.Push(op);
		}

		[MenuItem(MenuPath + "/Forward %#=")]
		public static void Forward()
		{
			if (_redo.Count == 0)
				return;

			var op = _redo.Pop();
			op.Perform();
			_undo.Push(op);
		}

		static private void Update()
		{
			var current = Selection.objects;
			if (current != null && !current.IsEqualTo(_previous))
			{
				Action a = () => _previous = Selection.objects;

				var so = new SelOp
				{
					ToSelect = current,
					ToGoBackTo = _previous,
					OnPerformed = a,
					OnUndone = a
				};

				_undo.Push(so);
				_redo.Clear();
				_previous = current;
			}
		}

		public struct SelOp
		{
			public UnityObject[] ToSelect, ToGoBackTo;
			public Action OnPerformed, OnUndone;

			public void Perform()
			{
				Selection.objects = ToSelect;
				OnPerformed();
			}

			public void Undo()
			{
				Selection.objects = ToGoBackTo;
				OnUndone();
			}
		}
	}
}