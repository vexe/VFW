using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Serialization;
using Vexe.Runtime.Types;

namespace VFWExamples
{
    /// <summary>
    /// An exapmle showing how to override the default serialization logic
    /// </summary>
    public class SerializationLogicExample : BetterBehaviour
    {
        [CustomSerialize] int serializedAndShown;

        // Since our custom logic doesn't use [Serialize], this field will not be serialized
        [Serialize, Show] int notSerializedButShown;

        public int AutoProperty { get; set; }

        private int _backingField;

        // Our custom logic allows the serialization of properties with custom getters/setters
        // The default logic in VFW disallows that because you might have side effects that calls some APIs
        // that are not allowed in the serialization thread (e.g. Unity API, ToString etc)
        public int PropertyWithSideEffects
        {
            get { return _backingField; }
            set
            {
                // Let's put a log so we get feedback when deserializing
                // I chose to print the call stack for fun!
                var stack = string.Join(" -> ", new StackTrace().GetFrames()
                                                                .Select(x => x.GetMethod().Name)
                                                                .ToArray());
                print("Property set call stack: " + stack);

                _backingField = value;
            }
        }

        public SerializedClass serializedObject = new SerializedClass();

        // according to our logic, this object is not serialized so to expose it
        // (just to see that it didn't serialize), we have to use [Show]
        [Show] public NotSerializedClass notSerializedObject = new NotSerializedClass();

        public override ISerializationLogic GetSerializationLogic()
        {
            return CustomLogic.Instance;
        }
    }

    // an example simplified custom serialization logic
    public class CustomLogic : ISerializationLogic
    {
        public static readonly CustomLogic Instance = new CustomLogic();

        // accept non-constant public fields or annotated with CustomSerialize
        public override bool IsSerializableField(FieldInfo field)
        {
            if (field.IsLiteral)
                return false;

            if (!IsSerializableType(field.FieldType))
                return false;

            return field.IsPublic || field.IsDefined<CustomSerializeAttribute>();
        }

        // accept readable/writing properties with public get/set or annotated with CustomSerialize
        public override bool IsSerializableProperty(PropertyInfo property)
        {
            if (!property.CanReadWrite())
                return false;

            if (!IsSerializableType(property.PropertyType))
                return false;

            return (property.GetGetMethod().IsPublic && property.GetSetMethod().IsPublic)
                 || property.IsDefined<CustomSerializeAttribute>();
        }

        public override bool IsSerializableType(Type type)
        {
            if (type == typeof(int) || type == typeof(float) || type == typeof(string))
                return true;

            // Note that the default VFW logic doesn't require types to have any attribute for them to be serialized
            return type.IsDefined<CustomSerializeAttribute>();
        }
    }

    public class CustomSerializeAttribute : Attribute
    {
    }

    [CustomSerialize]
    public class SerializedClass
    {
        public int value;
    }

    public class NotSerializedClass
    {
        public int value;
    }
}
