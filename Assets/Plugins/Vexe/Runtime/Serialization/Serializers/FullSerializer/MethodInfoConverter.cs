using System;
using System.Reflection;
using FullSerializer;

namespace Vexe.Runtime.Serialization
{
	public class MethodInfoConverter : fsConverter
	{
        public override bool RequestCycleSupport(Type storageType)
        {
            return false;
        }

		public override bool CanProcess(Type type)
		{
			return typeof(MethodInfo).IsAssignableFrom(type);
		}

		public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
		{
            var method = instance as MethodInfo;
            serialized = fsData.CreateList();
            var list = serialized.AsList;

            fsData declTypeData;
            Serializer.TrySerialize(method.DeclaringType, out declTypeData);

            list.Add(declTypeData);
            list.Add(new fsData(method.Name));

            var args = method.GetParameters();
            list.Add(new fsData(args.Length));

            for(int i = 0; i < args.Length; i++)
            { 
                fsData argData;
                Serializer.TrySerialize(args[i].ParameterType, out argData);
                list.Add(argData);
            }

			return fsResult.Success;
		}

		public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
		{
            var list = data.AsList;

            Type declaringType = null;
            Serializer.TryDeserialize(list[0], ref declaringType);

            string methodName = list[1].AsString;
            int argCount = (int)list[2].AsInt64;
            var argTypes = new Type[argCount];
            for(int i = 0; i < argCount; i++)
            { 
                var argData = list[i + 3];
                Serializer.TryDeserialize(argData, ref argTypes[i]);
            }

            var flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            instance = declaringType.GetMethod(methodName, flags, null, argTypes, null);
			return fsResult.Success;
		}
	}
}