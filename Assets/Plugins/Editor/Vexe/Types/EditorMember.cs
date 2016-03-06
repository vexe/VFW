using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
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
        public Action Write = () => { };

        // These are set when the member is an element of a collection (list, array, dictionary)
        // So you can use these to query whether or not an element is a collection element (Elemnts != null or ElementIndex != -1)
		public IList Elements;
		public int ElementIndex = -1;

        // Set when the member is a collection (list, array, dictionary)
        // Use this to query whether an element is a collection (CollectionCoun != -1)
        public int CollectionCount = -1;

		public readonly int Id;
        public readonly string Name;
        public readonly string NiceName;
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
                    string name = x.Name;
                    string result = null;

                    if (name.IsPrefix("m_"))
                        result = name.Remove(0, 1);
                    else result = name;

                    result = result.ToTitleCase();

                    if (!VFWSettings.UseHungarianNotation)
                        return result.SplitPascalCase();;

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

        public bool IsCollection
        {
            get { return ElementIndex != -1; }
        }

        private Action<object> _set;
        private Func<object> _get;

        private MemberSetter<object, object> _memberSetter;
        private MemberGetter<object, object> _memberGetter;

		private static BetterUndo _undo = new BetterUndo();
		private double _undoTimer, _undoLastTime;
        private const double kUndoTick = .5;
		private SetVarOp<object> _setVar;
        private static Attribute[] Empty = new Attribute[0];

        public readonly HashSet<Attribute> InitializedComposites = new HashSet<Attribute>();

        private EditorMember(MemberInfo memberInfo, Type memberType, string memberName,
            object rawTarget, UnityObject unityTarget, int targetId, Attribute[] attributes)
		{
            if (attributes == null)
                attributes = Empty;
            else ResolveUsing(ref attributes);

            Info         = memberInfo;
            Type         = memberType;
            RawTarget    = rawTarget;
            TypeNiceName = memberType.GetNiceName();
            Name         = memberName;
            NiceName     = Formatters[@"\$nicename"].Invoke(this);
            UnityTarget  = unityTarget;
            Attributes   = attributes;

            string displayFormat = null;

            var displayAttr = attributes.GetAttribute<DisplayAttribute>();
            if (displayAttr != null && MemberDrawersHandler.IsApplicableAttribute(memberType, displayAttr, attributes))
                displayFormat = displayAttr.FormatLabel;

            if (displayFormat == null)
            {
                if (Type.IsImplementerOfRawGeneric(typeof(IDictionary<,>)))
                    displayFormat = VFWSettings.DefaultDictionaryFormat;
                else if (Type.IsImplementerOfRawGeneric(typeof(IList<>)))
                    displayFormat = VFWSettings.DefaultSequenceFormat;
                else displayFormat = VFWSettings.DefaultMemberFormat;
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

        private void ResolveUsing(ref Attribute[] attributes)
        {
            var idx = attributes.IndexOf(x => x.GetType() == typeof(UsingAttribute));
            if (idx == -1)
                return;

            var usingAttr = attributes[idx] as UsingAttribute;

            FieldInfo field;
            var path = usingAttr.Path;
            var period = path.IndexOf('.');
            if (period == -1) // find field in [AttributeContainer] classes
            {
                field = ReflectionHelper.GetAllTypes()
                                        .Where(t => t.IsDefined<AttributeContainer>())
                                        .SelectMany(t => t.GetFields(Flags.StaticAnyVisibility))
                                        .FirstOrDefault(f => f.Name == path);
                if (field == null)
                {
                    Debug.Log("Couldn't find field: " + path + " in any [AttributeContainer] marked class");
                    return;
                }
            }
            else
            {
                var typeName = path.Substring(0, period);
                var container = ReflectionHelper.GetAllTypes().FirstOrDefault(x => x.Name == typeName);
                if (container == null)
                {
                    Debug.Log("Couldn't find type: " + typeName);
                    return;
                }

                var fieldName = path.Substring(period + 1);
                field = container.GetField(fieldName, Flags.StaticAnyVisibility);
                if (field == null)
                {
                    Debug.Log("Couldn't find static field: " + fieldName + " inside: " + typeName);
                    return;
                }
            }

            if (field.FieldType != typeof(Attribute[]))
            {
                Debug.Log("Field must be of type Attribute[] " + field.Name);
                return;
            }

            var imported = field.GetValue(null) as Attribute[];

            var display1 = attributes.GetAttribute<DisplayAttribute>();
            if (display1 != null)
            {
                var display2 = imported.GetAttribute<DisplayAttribute>();
                if (display2 != null)
                    CombineDisplays(src: display1, dest: display2);
            }

            var tmp = attributes.ToList();
            tmp.AddRange(imported);
            tmp.RemoveAt(idx);
            if (display1 != null)
                tmp.Remove(display1);

            attributes = tmp.ToArray();
        }

        public static void CombineDisplays(DisplayAttribute src, DisplayAttribute dest)
        {
            // combine seq/dict options
            dest.DictOpt |= src.DictOpt;
            dest.SeqOpt |= src.SeqOpt;

            // display1 overrides display2 if any formatting/order values are specified
            if (src.DisplayOrder.HasValue)
                dest.Order = src.Order;
            if (!string.IsNullOrEmpty(src.FormatMethod))
                dest.FormatMethod = src.FormatMethod;
            if (!string.IsNullOrEmpty(src.FormatKVPair))
                dest.FormatKVPair = src.FormatKVPair;
        }

        public static DisplayAttribute CombineDisplays(DisplayAttribute[] attributes)
        {
            var result = new DisplayAttribute();
            for (int i = 0; i < attributes.Length; i++)
            {
                var display = attributes[i];
                CombineDisplays(src: display, dest: result);
            }
            return result;
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
                ErrorHelper.MemberNotFound(targetType, memberName);
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
			Elements = list as IList;
			ElementIndex = index;
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
            if (ElementIndex < 0 || ElementIndex >= Elements.Count)
                return null;
			return Elements[ElementIndex];
		}

		private void SetListElement(object value)
		{
			Elements[ElementIndex] = value;
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
                ErrorHelper.InvalidCast(value, TypeNiceName);
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
