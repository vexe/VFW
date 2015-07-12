using System;
using System.Collections.Generic;
using System.IO;

namespace BX20Serializer
{
    public class PrimitiveSerializer : BaseSerializer
    {
        public static readonly PrimitiveSerializer Instance = new PrimitiveSerializer();

        public static readonly Type[] SupportedTypes = new Type[]
        {
            typeof(byte),
            typeof(sbyte),
            typeof(bool),
            typeof(int),
            typeof(uint),
            typeof(short),
            typeof(ushort),
            typeof(long),
            typeof(ulong),
            typeof(float),
            typeof(double),
            typeof(char),
            typeof(DateTime),
        };

        static readonly Dictionary<Type, Action<Stream, object>> _WriteLookup =
            new Dictionary<Type, Action<Stream, object>>();

        static readonly Dictionary<Type, Func<Stream, object>> _ReadLookup =
            new Dictionary<Type, Func<Stream, object>>();

        static PrimitiveSerializer()
        {
            _WriteLookup[typeof(byte)]     = (stream, value) => stream.WriteByte((byte)value);
            _WriteLookup[typeof(sbyte)]    = (stream, value) => stream.WriteSByte((sbyte)value);
            _WriteLookup[typeof(bool)]     = (stream, value) => stream.WriteBool((bool)value);
            _WriteLookup[typeof(int)]      = (stream, value) => stream.WriteInt((int)value);
            _WriteLookup[typeof(uint)]     = (stream, value) => stream.WriteUInt((uint)value);
            _WriteLookup[typeof(short)]    = (stream, value) => stream.WriteShort((short)value);
            _WriteLookup[typeof(ushort)]   = (stream, value) => stream.WriteUShort((ushort)value);
            _WriteLookup[typeof(long)]     = (stream, value) => stream.WriteLong((long)value);
            _WriteLookup[typeof(ulong)]    = (stream, value) => stream.WriteULong((ulong)value);
            _WriteLookup[typeof(float)]    = (stream, value) => stream.WriteFloat((float)value);
            _WriteLookup[typeof(double)]   = (stream, value) => stream.WriteDouble((double)value);
            _WriteLookup[typeof(char)]     = (stream, value) => stream.WriteChar((char)value);
            _WriteLookup[typeof(DateTime)] = (stream, value) => stream.WriteDateTime((DateTime)value);

            _ReadLookup[typeof(byte)]     = stream => stream.ReadByte();
            _ReadLookup[typeof(sbyte)]    = stream => stream.ReadSByte();
            _ReadLookup[typeof(bool)]     = stream => stream.ReadBool();
            _ReadLookup[typeof(int)]      = stream => stream.ReadInt();
            _ReadLookup[typeof(uint)]     = stream => stream.ReadUInt();
            _ReadLookup[typeof(short)]    = stream => stream.ReadShort();
            _ReadLookup[typeof(ushort)]   = stream => stream.ReadUShort();
            _ReadLookup[typeof(long)]     = stream => stream.ReadLong();
            _ReadLookup[typeof(ulong)]    = stream => stream.ReadULong();
            _ReadLookup[typeof(float)]    = stream => stream.ReadFloat();
            _ReadLookup[typeof(double)]   = stream => stream.ReadDouble();
            _ReadLookup[typeof(char)]     = stream => stream.ReadChar();
            _ReadLookup[typeof(DateTime)] = stream => stream.ReadDateTime();
        }

        public override bool Handles(Type type)
        {
            for (int i = 0; i < SupportedTypes.Length; i++)
                if (SupportedTypes[i] == type)
                    return true;
            return false;
        }

        public override void Serialize(Stream stream, Type type, object value)
        {
            _WriteLookup[type](stream, value);
        }

        public override void Deserialize(Stream stream, Type type, ref object value)
        {
            value = _ReadLookup[type](stream);
        }

        public static void Write(Stream stream, Type type, object value)
        {
            _WriteLookup[type](stream, value);
        }

        public static void Read(Stream stream, Type type, ref object value)
        {
            value = _ReadLookup[type](stream);
        }
    }
}
