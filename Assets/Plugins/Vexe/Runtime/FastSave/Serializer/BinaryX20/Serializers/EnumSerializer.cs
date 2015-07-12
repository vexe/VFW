using System;
using System.IO;

namespace BX20Serializer
{
    public class EnumSerializer : BaseSerializer
    {
        public override bool Handles(Type type)
        {
            return type.IsEnum;
        }

        public override void Serialize(Stream stream, Type type, object value)
        {
            var underlyingType = Enum.GetUnderlyingType(type);
            PrimitiveSerializer.Write(stream, underlyingType, value);
        }

        public override void Deserialize(Stream stream, Type type, ref object value)
        {
            var underlyingType = Enum.GetUnderlyingType(type);
            PrimitiveSerializer.Read(stream, underlyingType, ref value);
        }
    }
}
