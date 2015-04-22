using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Helpers;
using Vexe.Runtime.Types;
using UnityObject = UnityEngine.Object;

namespace Vexe.Runtime.Serialization
{
    public abstract class ISerializationLogic
    {
        public abstract bool IsSerializableField(FieldInfo field);
        public abstract bool IsSerializableProperty(PropertyInfo property);
        public abstract bool IsSerializableType(Type type);

        public readonly Func<Type, List<RuntimeMember>> CachedGetSerializableMembers;

        public ISerializationLogic()
        {
            CachedGetSerializableMembers = new Func<Type, List<RuntimeMember>>(type =>
                GetSerializableMembers(type, null).ToList()).Memoize();
        }

        private List<RuntimeMember> GetSerializableMembers(Type type, object target)
        {
            var members = ReflectionHelper.CachedGetMembers(type);
            var serializableMembers = members.Where(IsSerializableMember);
            var result = RuntimeMember.WrapMembers(serializableMembers, target);
            return result;
        }
        public bool IsSerializableMember(MemberInfo member)
        {
            var field = member as FieldInfo;
            if (field != null)
                return IsSerializableField(field);

            var property = member as PropertyInfo;
            if (property != null)
                return IsSerializableProperty(property);

            return false;
        }
    }

    public class VFWSerializationLogic : ISerializationLogic
    {
        public readonly SerializationAttributes Attributes;

        public static readonly SerializationAttributes DefaultAttributes = new SerializationAttributes
        (
            new[]
            {
                // Note: it's always better to annotate with [Serialize] instead of [SerializeField] (read the FAQ)
                typeof(SerializeField),
                typeof(SerializeAttribute),
            },

            new[]
            {
                // Didn't include NonSerializedAttribute cause it's only applicable to fields
                typeof(DontSerializeAttribute),
            },

            // We don't need to annotate types with special attributes to serialize them
            // since we're using serializers that don't require it
            Type.EmptyTypes
        );

        public static readonly VFWSerializationLogic Instance = new VFWSerializationLogic(DefaultAttributes);

        public VFWSerializationLogic(SerializationAttributes attributes)
        {
            this.Attributes = attributes;
        }

        public override bool IsSerializableType(Type type)
        {
            if (type.IsPrimitive || type.IsEnum || type == typeof(string)
                || type.IsA<UnityObject>()
                || UnityStructs.ContainsValue(type))
                return true;

            if (type.IsArray)
                return type.GetArrayRank() == 1 && IsSerializableType(type.GetElementType());

            if (type.IsInterface)
                return true;

            if (NotSupportedTypes.Any(type.IsA))
                return false;

            if (SupportedTypes.Any(type.IsA))
                return true;

            if (type.IsGenericType)
                return type.GetGenericArguments().All(IsSerializableType);

            return true;
        }

        public override bool IsSerializableField(FieldInfo field)
        {
            if (Attributes.DontSerializeMember.Any(field.IsDefined))
                return false;

            if (field.IsLiteral)
                return false;

            if (!(field.IsPublic || Attributes.SerializeMember.Any(field.IsDefined)))
                return false;

            bool serializable = IsSerializableType(field.FieldType);
            return serializable;
        }

        public override bool IsSerializableProperty(PropertyInfo property)
        {
            if (Attributes.DontSerializeMember.Any(property.IsDefined))
                return false;

            if (!property.IsAutoProperty())
                return false;

            if (!(property.GetGetMethod(true).IsPublic ||
                  property.GetSetMethod(true).IsPublic ||
                  Attributes.SerializeMember.Any(property.IsDefined)))
                return false;

            bool serializable = IsSerializableType(property.PropertyType);
            return serializable;
        }

        public static readonly Type[] UnityStructs =
        {
            typeof(Vector3),
            typeof(Vector2),
            typeof(Vector4),
            typeof(Rect),
            typeof(Quaternion),
            typeof(Matrix4x4),
            typeof(Color),
            typeof(Color32),
            typeof(LayerMask),
            typeof(Bounds)
        };

        public static readonly Type[] NotSupportedTypes =
        {
            typeof(Delegate)
        };

        public static readonly Type[] SupportedTypes =
        {
            typeof(Type)
        };
    }}
