using System;
using UnityEngine;
using Vexe.Editor.Types;
using Vexe.Runtime.Extensions;

namespace Vexe.Editor.Drawers
{
	public abstract class ObjectDrawer<T> : BaseDrawer
	{
		protected T memberValue
		{
			get { return (T)member.Value; }
			set { member.Value = value; }
		}

		public void MemberField()
		{
			MemberField(member);
		}

		public void MemberField(EditorMember dm)
		{
			gui.Member(dm, false);
		}

		public sealed override void OnLeftGUI() { }
		public sealed override void OnRightGUI() { }
		public sealed override void OnUpperGUI() { }
		public sealed override void OnLowerGUI() { }
		public sealed override void OnMemberDrawn(Rect area) { }
	}
}