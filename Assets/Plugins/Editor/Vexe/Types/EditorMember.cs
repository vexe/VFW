using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using Vexe.Editor.Types;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Helpers;
using Vexe.Runtime.Types;
using UnityObject = UnityEngine.Object;

namespace Vexe.Editor.Types
{
	public class EditorMember : RuntimeMember
	{
		public UnityObject UnityTarget;
		public int Id;

		public EditorMember(MemberInfo member, object rawTarget, UnityObject unityTarget, int id) : base(member, rawTarget)
		{
			this.UnityTarget = unityTarget;
			if (id != -1)
				this.Id = RTHelper.CombineHashCodes(id, TypeNiceName, NiceName);

			setVar = new SetVarOp<object>();
			setVar.GetCurrent = () => this.Value;
			setVar.SetValue = base.Set;
		}

		protected static BetterUndo undo = new BetterUndo();
		protected double undoTimer, undoLastTime, undoTick = .5f;
		protected SetVarOp<object> setVar;

		public override void Set(object value)
		{
			bool sameValue = value.GenericEqual(this.Value);
			if (sameValue)
				return;

			HandleUndoAndSet(value, base.Set);

			if (UnityTarget != null)
				EditorUtility.SetDirty(UnityTarget);
		}

		protected void HandleUndoAndSet(object value, Action<object> set)
		{
			undoTimer = EditorApplication.timeSinceStartup - undoLastTime;
			if (undoTimer > undoTick)
			{
				//Debug.Log("Registered undo");
				undoTimer = 0f;
				undoLastTime = EditorApplication.timeSinceStartup;
				BetterUndo.MakeCurrent(ref undo);
				setVar.ToValue = value;
				undo.RegisterThenPerform(setVar);
			}
			else set(value);

			if (UnityTarget != null)
				Undo.RecordObject(UnityTarget, "member set");
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			var member = obj as EditorMember;
			return member != null && member.Id == Id;
		}
	}

	public static class EditorMemberExtensions
	{
		public static bool IsNull(this EditorMember member)
		{
			object value;
			return (member == null || member.Equals(null)) || ((value = member.Value) == null || value.Equals(null));
		}
	}

	public class ArgMember : EditorMember
	{
		private readonly Func<object> getter;
		private readonly Action<object> setter;

		public ArgMember(Func<object> getter, Action<object> setter, object target, UnityObject unityTarget, Type dataType, Attribute[] attributes, string name, int id)
			: base(null, null, unityTarget, -1)
		{
			this.getter      = getter;
			this.setter      = setter;
			this.Target      = target;
			this.attributes  = attributes;
			this.Name        = name;
			this.Type        = dataType;
			this.Id			 = RTHelper.CombineHashCodes(id, name);

			setVar.GetCurrent = Get;
			setVar.SetValue = Set;
		}

		public override object Get()
		{
			return getter();
		}

		public override void Set(object value)
		{
			if (!value.GenericEqual(Get()))
				HandleUndoAndSet(value, setter);
		}
	}

	public class ElementMember<T> : ArgMember
	{
		public IList<T> List;
		public int Index;
		public bool Readonly;

		public ElementMember(Attribute[] attributes, string name, int id)
			: base(null, null, null, null, typeof(T), attributes, name, id)
		{
			setVar.GetCurrent = Get;
			setVar.SetValue = x => List[Index] = (T)x;
		}

		public void Initialize(IList<T> list, int idx, object rawTarget, UnityObject unityTarget)
		{
			this.List = list;
			this.Index = idx;
			this.Target = rawTarget;
			this.UnityTarget = unityTarget;
		}

		public override object Get()
		{
			return List[Index];
		}

		private void SetInternal(object value)
		{
			List[Index] = (T)(value);
		}

		public override void Set(object value)
		{
			if (!Readonly && !value.GenericEqual(Get()))
				HandleUndoAndSet(value, SetInternal);
		}

		public override string ToString()
		{
			return base.ToString() + Index;
		}
	}
}
