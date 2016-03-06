using System;
using UnityEngine;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Serialization;

namespace Vexe.Runtime.Types
{
    [Obsolete("Please use BaseScrtiptableObject instead")]
    public abstract class BetterScriptableObject : BaseScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField]
        private SerializationData _serializationData;
        private SerializationData serializationData
        {
            get { return _serializationData ?? (_serializationData = new SerializationData()); }
        }

        private SerializerBackend _serializer;
        public SerializerBackend Serializer
        {
            get
            {
                if (_serializer == null)
                {
                    var type = GetSerializerType();
                    _serializer = type.ActivatorInstance<SerializerBackend>();
                }
                return _serializer;
            }
        }

        public virtual void OnBeforeSerialize()
        {
            SerializeObject();
        }

        public virtual void OnAfterDeserialize()
        {
            DeserializeObject();
        }

        public virtual Type GetSerializerType()
        {
            return SerializerBackend.DefaultType;
        }

        [ContextMenu("Load behaviour state")]
        public virtual void DeserializeObject()
        {
            Serializer.DeserializeTargetFromData(this, serializationData);
        }

        [ContextMenu("Save behaviour state")]
        public virtual void SerializeObject()
        {
            Serializer.SerializeTargetIntoData(this, serializationData);
        }
    }
}
