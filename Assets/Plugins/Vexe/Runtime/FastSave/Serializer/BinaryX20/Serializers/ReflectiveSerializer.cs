using System;
using System.IO;
using System.Reflection;
using Vexe.Runtime.Extensions;

namespace BX20Serializer
{
    public class ReflectiveSerializer : BaseSerializer
    {
        public override bool Handles(Type type)
        {
            return true;
        }

        public override object GetInstance(Stream stream, Type type)
        {
            var ctor = type.DelegateForCtor(Type.EmptyTypes);
            return ctor(null);
        }

        public override void Serialize(Stream stream, Type type, object value)
        {
            var members = GetMembers(type);
            for (int i = 0; i < members.Length; i++)
            {
                var member = X20Member.WrapMember(members[i], value);
                ctx.Serializer.Serialize_Main(stream, member.Type, member.Value);
            }
        }

        public override void Deserialize(Stream stream, Type type, ref object value)
        {
            var members = GetMembers(type);
            for (int i = 0; i < members.Length; i++)
            {
                var member = X20Member.WrapMember(members[i], value);
                var memberValue = member.Value;
                ctx.Serializer.Deserialize_Main(stream, member.Type, ref memberValue);
                member.Value = memberValue;
                value = member.Target; // this bit is needed for structs!
            }
        }

        public virtual MemberInfo[] GetMembers(Type type)
        {
            return X20Logic.CachedGetSerializableMembers(type);
        }
    }

    /// <summary>
    /// The idea is that we specify at compile time a target type, member(s) type(s) and their writers/readers.
    /// Should give at least a x2 performance boost over the regular reflective stuff.
    /// Makes sense to use if you have a class with a lot of value-types and you want it to written/read fast.
    /// This class only supports a single member just to test the waters, obviously you'll have more than that.
    /// </summary>
    public class StrongReflectiveSerializer<TTarget, TMember0> : ReflectiveSerializer
    {
        private readonly MemberSetter<TTarget, TMember0> _setter0;
        private readonly MemberGetter<TTarget, TMember0> _getter0;
        private readonly StrongReader<TMember0> _reader0;
        private readonly StrongWriter<TMember0> _writer0;

        public StrongReflectiveSerializer(string member0, StrongWriter<TMember0> writer0, StrongReader<TMember0> reader0)
        {
            var flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
            var member = typeof(TTarget).GetMemberFromAll(member0, flags);
            _setter0 = DelegateForSet<TTarget, TMember0>(member);
            _getter0 = DelegateForGet<TTarget, TMember0>(member);
            _writer0 = writer0;
            _reader0 = reader0;
        }

        public override bool Handles(Type type)
        {
            return type == typeof(TTarget);
        }

        public override void Serialize(Stream stream, Type type, object value)
        {
            SerializeMember0(stream, value);
        }

        public override void Deserialize(Stream stream, Type type, ref object value)
        {
            DeserializeMember0(stream, ref value);
        }

        protected void SerializeMember0(Stream stream, object value)
        {
            _writer0(stream, _getter0((TTarget)value));
        }

        protected void DeserializeMember0(Stream stream, ref object value)
        {
            TMember0 result = _getter0((TTarget)value);
            _reader0(stream, ref result);
            var target = (TTarget)value;
            _setter0(ref target, result);
            value = target;
        }

        protected static MemberGetter<T, V> DelegateForGet<T, V>(MemberInfo member)
        {
            var field = member as FieldInfo;
            if (field != null)
                return field.DelegateForGet<T, V>();
            var property = member as PropertyInfo;
            if (property == null)
                throw new NotSupportedException(member.Name);
            return property.DelegateForGet<T, V>();
        }

        protected static MemberSetter<T, V> DelegateForSet<T, V>(MemberInfo member)
        {
            var field = member as FieldInfo;
            if (field != null)
                return field.DelegateForSet<T, V>();
            var property = member as PropertyInfo;
            if (property == null)
                throw new NotSupportedException(member.Name);
            return property.DelegateForSet<T, V>();
        }
    }
}
