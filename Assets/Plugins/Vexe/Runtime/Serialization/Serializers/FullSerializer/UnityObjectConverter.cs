using System;
using System.Collections.Generic;
using FullSerializer;
using UnityObject = UnityEngine.Object;

namespace Vexe.Runtime.Serialization
{
    /// <summary>
    /// The hack that's used to persist UnityEngine.Object references
    /// Whenever the serializer comes across a Unity object it stores it to a list
    /// of Unity objects (which Unity serializes) and serializes the index of where
    /// that storage took place.
    /// </summary>
	public class UnityObjectConverter : fsConverter
	{
		private List<UnityObject> serializedObjects
		{
			get { return Serializer.Context.Get<List<UnityObject>>(); }
		}

		public override bool CanProcess(Type type)
		{
			return typeof(UnityObject).IsAssignableFrom(type);
		}

        public override bool RequestCycleSupport(Type storageType)
        {
            return false;
        }

        public override bool RequestInheritanceSupport(Type storageType)
        {
            return false;
        }

		public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
		{
            var obj = instance as UnityObject;
            int idx = serializedObjects.IndexOf(obj);
            if (idx == -1)
            {
                Serializer.TrySerialize<int>(serializedObjects.Count, out serialized);
			    serializedObjects.Add(obj);
            }
            else 
                Serializer.TrySerialize<int>(idx, out serialized);

			return fsResult.Success;
		}

		public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
		{
            int index = -1;
            var result = Serializer.TryDeserialize<int>(data, ref index);
            if (index == -1)
                throw new InvalidOperationException("Error deserializing Unity object of type " + storageType + ". Index shouldn't be -1. Message: " + result.FormattedMessages);
            instance = serializedObjects[index];
			return fsResult.Success;
		}

		public override object CreateInstance(fsData data, Type storageType)
		{
            return storageType;
		}
	}
}