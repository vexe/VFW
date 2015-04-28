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
			get
            {
                try
                {
                    return (T)member.Value;
                }
                catch(InvalidCastException)
                {
                    throw new vInvalidCast(member.TypeNiceName, typeof(T).GetNiceName());
                }
            }
			set { member.Value = value; }
		}

		public void MemberField()
		{
			MemberField(member);
		}

		public void MemberField(EditorMember member)
		{
			gui.Member(member, false);
		}

        public override bool CanHandle(Type memberType)
        {
            return memberType.IsA<T>() || memberType.IsSubclassOrImplementerOfRawGeneric(typeof(T));
        }

        protected EditorMember FindRelativeMember(string memberName)
        {
            return EditorMember.WrapMember(memberName, typeof(T), memberValue, unityTarget, id);
        }

		public sealed override void OnLeftGUI() { }
		public sealed override void OnRightGUI() { }
		public sealed override void OnUpperGUI() { }
		public sealed override void OnLowerGUI() { }
		public sealed override void OnMemberDrawn(Rect area) { }
	}
}
