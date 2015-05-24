using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Helpers;
using Vexe.Runtime.Types;
using UnityObject = UnityEngine.Object;

namespace Vexe.Editor.Types
{
    public class EditorMember
    {
        public object RawTarget;
        public UnityObject UnityTarget;
        public string DisplayText;

		public readonly int Id;
        public readonly string Name;
        public readonly string TypeNiceName;
        public readonly Type Type;
        public readonly MemberInfo Info;
        public readonly Attribute[] Attributes;

        public static readonly Dictionary<string, Func<EditorMember, string>> Formatters = new Dictionary<string, Func<EditorMember, string>>()
        {
            { @"\$type"    , x => x.Type.Name },
            { @"\$nicetype", x => x.TypeNiceName },
            { @"\$name"    , x => x.Name },
            { @"\$nicename", x =>
                {
                    var settings = VFWSettings.GetInstance();
                    var name = x.Name;
                    if (!settings.UseHungarianNotation)
                        return name.Replace("m_", "").Replace("_", "").SplitPascalCase();;

                    string result = name.Replace("m_", "").Replace("_", "");

                    if (name.Length > 1 && char.IsLower(result[0]) && char.IsUpper(result[1]))
                    {
                        var lowerResultInitial = char.ToLower(result[0]);
                        var nicetype = x.TypeNiceName;

                        // check for n as well, eg nSize
                        if ((nicetype == "int" && lowerResultInitial == 'i' || lowerResultInitial == 'n')
                            || char.ToLower(nicetype[0]) == lowerResultInitial)
                            return result.Remove(0, 1).SplitPascalCase();
                    }

                    return result.SplitPascalCase();
                }
            },
        };

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
        private static Attribute[] Empty = new Attribute[0];

        private EditorMember(MemberInfo memberInfo, Type memberType, string memberName,
            object rawTarget, UnityObject unityTarget, int targetId, Attribute[] attributes)
		{
            Info         = memberInfo;
            Type         = memberType;
            RawTarget    = rawTarget;
            Name         = memberName;
            TypeNiceName = memberType.GetNiceName();
            UnityTarget  = unityTarget;
            Attributes   = attributes ?? Empty;

            string displayFormat = null;

            var displayAttr = Attributes.GetAttribute<DisplayAttribute>();
            if (displayAttr != null)
                displayFormat = displayAttr.FormatLabel;

            var settings = VFWSettings.GetInstance();

            if (displayFormat == null)
            {
                if (Type.IsImplementerOfRawGeneric(typeof(IDictionary<,>)))
                    displayFormat = settings.DictionaryFormat;
                else if (Type.IsImplementerOfRawGeneric(typeof(IList<>)))
                    displayFormat = settings.SequenceFormat;
                else displayFormat = settings.MemberFormat;
            }

            var iter = Formatters.GetEnumerator();
            while(iter.MoveNext())
            {
                var pair = iter.Current;
                var pattern = pair.Key;
                var result = pair.Value(this);
                displayFormat = Regex.Replace(displayFormat, pattern, result, RegexOptions.IgnoreCase);
            }

            DisplayText = displayFormat;

            Id = RuntimeHelper.CombineHashCodes(targetId, TypeNiceName, DisplayText);
		}

        public object Get()
        {
           return _get();
        }

		public void Set(object value)
        {
			bool sameValue = value.GenericEquals(Get());
			if (sameValue)
				return;

			HandleUndoAndSet(value);

			if (UnityTarget != null)
				EditorUtility.SetDirty(UnityTarget);
        }

        public T As<T>() where T : class
        {
            return Get() as T;
        }

		private void HandleUndoAndSet(object value)
		{
			if (UnityTarget != null)
				Undo.RecordObject(UnityTarget, "Editor Member Modification");

			_undoTimer = EditorApplication.timeSinceStartup - _undoLastTime;
			if (_undoTimer > kUndoTick)
			{
				_undoTimer = 0f;
				_undoLastTime = EditorApplication.timeSinceStartup;
				BetterUndo.MakeCurrent(ref _undo);
				_setVar.ToValue = value;
				_undo.RegisterThenPerform(_setVar);
			}
			else _set(value);
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

        public static EditorMember WrapMember(string memberName, Type targetType, object rawTarget, UnityObject unityTarget, int id, Attribute[] attributes)
        {
            var member = GetMember(memberName, targetType);
            return WrapMember(member, rawTarget, unityTarget, id, attributes);
        }

        public static EditorMember WrapMember(string memberName, Type targetType, object rawTarget, UnityObject unityTarget, int id)
        {
            var member = GetMember(memberName, targetType);
            return WrapMember(member, rawTarget, unityTarget, id, member.GetAttributes());
        }

        private static MemberInfo GetMember(string memberName, Type targetType)
        {
            var members = targetType.GetMember(memberName, MemberTypes.Field | MemberTypes.Property, Flags.StaticInstanceAnyVisibility);
            if (members.IsNullOrEmpty())
                throw new vMemberNotFound(targetType, memberName);
            return members[0];
        }

        public static EditorMember WrapMember(MemberInfo memberInfo, object rawTarget, UnityObject unityTarget, int id)
        {
            return WrapMember(memberInfo, rawTarget, unityTarget, id, memberInfo.GetAttributes());
        }

        public static EditorMember WrapMember(MemberInfo memberInfo, object rawTarget, UnityObject unityTarget, int id, Attribute[] attributes)
        {
            var field = memberInfo as FieldInfo;
            if (field != null)
            {
                if (field.IsLiteral)
                    throw new InvalidOperationException("Field is const, this is not supported: " + field);

                var result = new EditorMember(field, field.FieldType, field.Name, rawTarget, unityTarget, id, attributes);
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

                var result = new EditorMember(property, property.PropertyType, property.Name, rawTarget, unityTarget, id, attributes);
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
            try
            {
                _memberSetter(ref RawTarget, value);
            }
            catch(InvalidCastException)
            {
                throw new vInvalidCast(value, TypeNiceName);
            }
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
