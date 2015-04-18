using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Helpers;
using UnityObject = UnityEngine.Object;

namespace Vexe.Editor.Types
{
	public class EditorMember
	{
        public object RawTarget;
		public UnityObject UnityTarget;

		public readonly int Id;
        public readonly string Name;
        public readonly string NiceName;
        public readonly string TypeNiceName;
        public readonly Type Type;
        public readonly MemberInfo Info;
        public readonly Attribute[] Attributes;

        public object Value
        {
            get { return Get(); }
            set { Set(value); }
        }

        private Action<object> _set;
        private Func<object> _get;

        private MemberSetter<object, object> _memberSetter;
        private MemberGetter<object, object> _memberGetter;

		private IList _list;
		private int _index;

		private static BetterUndo _undo = new BetterUndo();
		private double _undoTimer, _undoLastTime;
        private const double kUndoTick = .5;
		private SetVarOp<object> _setVar;

        private EditorMember(MemberInfo memberInfo, Type memberType, string memberName, object rawTarget, UnityObject unityTarget, int targetId, Attribute[] attributes)
		{
            Info         = memberInfo;
            Type         = memberType;
            RawTarget    = rawTarget;
            Name         = memberName;
            NiceName     = Name.Replace("_", "").SplitPascalCase();
            TypeNiceName = memberType.GetNiceName();
			UnityTarget  = unityTarget;
			Id           = RuntimeHelper.CombineHashCodes(targetId, TypeNiceName, NiceName);
            Attributes   = attributes;
		}

        public object Get()
        {
           return _get();
        }

		public void Set(object value)
        {
			bool sameValue = value.GenericEqual(Get());
			if (sameValue)
				return;

			HandleUndoAndSet(value, _set);

			if (UnityTarget != null)
				EditorUtility.SetDirty(UnityTarget);
        }

        public T As<T>() where T : class
        {
            return Get() as T;
        }

		private void HandleUndoAndSet(object value, Action<object> set)
		{
			_undoTimer = EditorApplication.timeSinceStartup - _undoLastTime;
			if (_undoTimer > kUndoTick)
			{
				_undoTimer = 0f;
				_undoLastTime = EditorApplication.timeSinceStartup;
				BetterUndo.MakeCurrent(ref _undo);
				_setVar.ToValue = value;
				_undo.RegisterThenPerform(_setVar);
			}
			else set(value);

			if (UnityTarget != null)
				Undo.RecordObject(UnityTarget, "member set");
		}

        public override string ToString()
        {
            return TypeNiceName + " " + Name;
        }

		public override int GetHashCode()
		{
			return Id;
		}

		public override bool Equals(object obj)
		{
			var member = obj as EditorMember;
			return member != null && this.Id == member.Id;
		}

        public static EditorMember WrapMember(MemberInfo memberInfo, object rawTarget, UnityObject unityTarget, int id)
        {
            var field = memberInfo as FieldInfo;
            if (field != null)
            { 
                if (field.IsLiteral)
                    throw new InvalidOperationException("Field is const, this is not supported: " + field);

                var result = new EditorMember(field, field.FieldType, field.Name, rawTarget, unityTarget, id, field.GetAttributes());
                result.InitGetSet(result.GetWrappedMemberValue, result.SetWrappedMemberValue);
                result._memberGetter = field.DelegateForGet();
                result._memberSetter = field.DelegateForSet();
                return result;
            }
            else
            {
                var property = memberInfo as PropertyInfo;

                if (property == null)
                    throw new InvalidOperationException("Member " + memberInfo + " is not a field nor property.");

                if (!property.CanRead)
                    throw new InvalidOperationException("Property doesn't have a getter method: " + property);

                if(property.IsIndexer())
                    throw new InvalidOperationException("Property is an indexer, this is not supported: " + property);

                var result = new EditorMember(property, property.PropertyType, property.Name, rawTarget, unityTarget, id, property.GetAttributes());
                result.InitGetSet(result.GetWrappedMemberValue, result.SetWrappedMemberValue);

                result._memberGetter = property.DelegateForGet();

                if (property.CanWrite)
                    result._memberSetter = property.DelegateForSet();
                else
                    result._memberSetter = delegate(ref object obj, object value) { };

                return result;
            }
        }

        public static EditorMember WrapGetSet(Func<object> get, Action<object> set, object rawTarget, UnityObject unityTarget, Type dataType, string name, int id, Attribute[] attributes)
        {
            var result = new EditorMember(null, dataType, name, rawTarget, unityTarget, id, attributes);
            result.InitGetSet(get, set);
            return result;
        }
	
        public static EditorMember WrapIListElement(string elementName, Type elementType, int elementId, Attribute[] attributes)
        {
            var result = new EditorMember(null, elementType, elementName, null, null, elementId, attributes);
            result.InitGetSet(result.GetListElement, result.SetListElement);
            return result;
        }

		public void InitializeIList<T>(IList<T> list, int index, object rawTarget, UnityObject unityTarget)
		{
			_list = list as IList;
			_index = index;
			RawTarget = rawTarget;
			UnityTarget = unityTarget;
		}

        private void InitGetSet(Func<object> get, Action<object> set)
        {
            _get = get;
            _set = set;
            _setVar = new SetVarOp<object>();
			_setVar.GetCurrent = get;
			_setVar.SetValue = set;
        }

		private object GetListElement()
		{
			return _list[_index];
		}

		private void SetListElement(object value)
		{
			_list[_index] = value;
		}

        private object GetWrappedMemberValue()
        {
            return _memberGetter(RawTarget);
        }

        private void SetWrappedMemberValue(object value)
        {
            _memberSetter(ref RawTarget, value);
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
}
