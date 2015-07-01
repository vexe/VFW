using System;
using UnityEngine;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Helpers;
using Vexe.Runtime.Serialization;

namespace Vexe.Runtime.Types
{
    /// <summary>
    /// Inherit this class in your behaviours to get the full editor power of Vfw + custom serialization
    /// Note that it is better to use BaseBehaviour unless Unity's serialization doesn't work for you and
    /// you truely need custom serialization
    /// </summary>
    public abstract class BetterBehaviour : BaseBehaviour, ISerializationCallbackReceiver
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
#if UNITY_EDITOR
            if (RuntimeHelper.IsModified(this, Serializer, serializationData))
            {
                SerializeObject();
            }
#else
                SerializeObject();
#endif
        }

        public virtual void OnAfterDeserialize()
        {
#if UNITY_EDITOR
            if (_delayDeserialize)
            {
                _delayDeserialize = false;
                return;
            }
#endif
            DeserializeObject();
        }

        public virtual void Reset()
        {
            RuntimeHelper.ResetTarget(this);
        }

#if UNITY_EDITOR
        // this editor hack is needed to make it possible to let Unity Layout draw things after RabbitGUI.
        // For some reason, if I try to let Unity draw things via obj.Update(), PropertyField(...) and obj.ApplyModifiedProperties(),
        // it will send deserialization requests which will deserialize the behaviour overriding the new changes made in the property
        // which means that the property will not be modified. so we delay deserialization for a single editor frame
        private bool _delayDeserialize;

        public void DelayNextDeserialize()
        {
            _delayDeserialize = true;
        }
#endif

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
