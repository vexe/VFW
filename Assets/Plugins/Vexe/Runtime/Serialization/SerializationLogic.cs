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
    /// <summary>
    /// Defines a serialization logic interface that specifies whether a field, property or type is serializable or not
    /// </summary>
    public abstract class ISerializationLogic
    {
        public abstract bool IsSerializableField(FieldInfo field);
        public abstract bool IsSerializableProperty(PropertyInfo property);
        public abstract bool IsSerializableType(Type type);

        /// <summary>
        /// Returns a list of RuntimeMember wrapping all the serializable members in the specified type.
        /// Caches the result so that we only do the query once for a certain type.
        /// </summary>
        public readonly Func<Type, RuntimeMember[]> CachedGetSerializableMembers;

        public ISerializationLogic()
        {
            CachedGetSerializableMembers = new Func<Type, RuntimeMember[]>(type =>
                GetSerializableMembers(type, null).ToArray()).Memoize();
        }

        /// <summary>
        /// Returns a list of RuntimeMember wrapping all the serializable members in the specified type.
        /// </summary>
        private List<RuntimeMember> GetSerializableMembers(Type type, object target)
        {
            var members = ReflectionHelper.CachedGetMembers(type);
            var serializableMembers = members.Where(IsSerializableMember);
            var result = RuntimeMember.WrapMembers(serializableMembers, target);
            return result;
        }

        /// <summary>
        /// returns IsSerializableField if the member is a field,
        /// returns IsSerializableProperty if the member is a property,
        /// otherwise false
        /// </summary>
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

    /// <summary>
    /// The default serialization logic in VFW.
    /// </summary>
    public class VFWSerializationLogic : ISerializationLogic
    {
        /// <summary>
        /// The serialization attributes that this logic uses to determine whether or not a field, property or type is serialiable
        /// </summary>
        public readonly SerializationAttributes Attributes;

        /// <summary>
        /// The default attributes used in VFW
        /// - [Serialize] and [SerializeField] to tell that a field/property must be serialized
        /// - [DontSerialize] to tell that a field/property should not be serialized
        /// - no attributes required for a type to be serializable
        /// </summary>
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

        /// <summary>
        /// Types don't need to be annotated with any special attributes for them to be serialized
        /// A type is serialized if:
        /// - it's a primitive, enum or string
        /// - or a UnityEngine.Object
        /// - or a Unity struct
        /// - or a single-dimensional array and the element type is serializable
        /// - or an interface
        /// - or included in the 'SupportedTypes' array
        /// - it's not included in the 'NotSupportedTypes' array
        /// - it's generic and all its generic type arguments are serializable as well
        /// </summary>
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

        /// <summary>
        /// A field is serializable if:
        /// - it's not marked with any of the 'DontSerializeMember' attributes
        /// - nor literal (const)
        /// - it's public otherwise annotated with one of the attributes in the 'SerializeMember' array
        /// - its type is serializable
        /// - readonly fields that meet the previous requirements are serialized in Better[Behaviour|ScriptableObject]
        ///   and System.Objects as long as the used serializer supports it (FullSerializer does)
        /// - static fields that meet the previous  requirements are always serialized in  Better[Behaviour|ScriptableObject],
        /// and in System.Objects if the serializer of use supports it (FullSerialier doesn't)
        /// </summary>
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

        /// <summary>
        /// A property is serializable if:
        /// - it's not marked with any of the 'DontSerializeMember' attributes
        /// - it's an auto-property
        /// - has a public getter or setter, otherwise must be annotated with any of the 'SerializeMember' attributes
        /// - its type is serializable
        /// - static properties that meet the previous requirements are always serialized in Better[Behaviour|ScriptableObject],
        ///   and in System.Objects if the serializer of use supports it (FullSerialier doesn't)
        /// </summary>
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
