using UnityEditor;
using UnityEngine;
using Vexe.Runtime.Types;

namespace Vexe.Editor.Types
{
	public class SelectionOp : BasicOp
	{
		public Object[] ToSelect { get; set; }
		public Object[] ToGoBackTo { get; set; }

		public override void Perform()
		{
			SelectObjects(ToSelect);
			base.Perform();
		}

		public override void Undo()
		{
			SelectObjects(ToGoBackTo);
			base.Undo();
		}

		private void SelectObjects(Object[] objects)
		{
			Selection.objects = objects;
		}
	}
}