using UnityEngine;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Types;
using System;
using System.Collections.Generic;

namespace Vexe.Editor.Drawers
{
	// TODO: Fix generic
	//public class ReadonlyAttributeDrawer<T> : CompositeDrawer<T, ReadonlyAttribute>
	[IgnoreOnTypes(typeof(Array), typeof(Dictionary<,>), typeof(List<>), typeof(Stack<>))]
	public class ReadonlyAttributeDrawer : CompositeDrawer<object, ReadonlyAttribute>
	{
		private object previous;

		protected override void OnSingleInitialization()
		{
			previous = memberValue;
		}

		public override void OnLowerGUI()
		{
			if (!Application.isPlaying && attribute.AssignAtEditTime)
				return;

			if (!memberValue.GenericEqual(previous))
				memberValue = previous;
		}
	}
}
