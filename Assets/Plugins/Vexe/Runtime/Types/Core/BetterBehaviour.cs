using System;
using UnityEngine;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Helpers;
using Vexe.Runtime.Serialization;

namespace Vexe.Runtime.Types
{
    [DefineCategory("", 0, MemberType = CategoryMemberType.All, Exclusive = false, AlwaysHideHeader = true)]
    [DefineCategory("Dbg", 3f, Pattern = "^dbg")]
    public abstract class BetterBehaviour : MonoBehaviour, IVFWObject, ISerializationCallbackReceiver
    {
        [SerializeField]
        private SerializationData _serializationData;

        [SerializeField]
        private SerializableType _serializerType;

        /// <summary>
        /// A persistent identifier used primarly from editor scripts to have editor data persist
        /// Could be used at runtime as well if you have any usages of a unique id
        /// Note this is not the same as GetInstanceID, as it seems to change when you reload scenes
        /// This id gets assigned only once and then serialized.
        /// </summary>
        [SerializeField, HideInInspector]
        private int _id = -1;

        [Display("Serializer Backend"), ShowType(typeof(SerializerBackend))]
        private Type SerializerType
        {
            get
            {
                var type = GetSerializerType();
                if (_serializerType == null || !_serializerType.HasValidName() || _serializerType.Value != type)
                {
                    if (!type.IsA<SerializerBackend>())
                    {
                        Debug.LogError("Serializer type must inherit BackendSerializer: " + type.Name);
                        type = SerializerBackend.DefaultType;
                    }
                    _serializerType = new SerializableType(type);
                }

                var result = _serializerType.Value;
                if (result == null)
                { 
                    result = type;
                    _serializerType = new SerializableType(result);
                }
                return result;
            }
            set
            {
                if (_serializerType.Value != value && value != null)
                {
                    _serializerType.Value = value;
                    _serializer = value.ActivatorInstance<SerializerBackend>();
                }
            }
        }

        private SerializerBackend _serializer;
        public SerializerBackend Serializer
        {
            get { return _serializer ?? (_serializer = SerializerType.ActivatorInstance<SerializerBackend>()); }
        }

        public virtual void OnBeforeSerialize()
        {
            bool serialize;
            if (SemiAutomaticSerialization())
                serialize = _dirty++ < 2;
            else serialize = RuntimeHelper.IsModified(this, Serializer, GetSerializationData());

            if (serialize)
            {
                dLog("Serializing: " + GetType().Name);
                SerializeObject();
            }
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

        // Logging
        #region
        public bool dbg;

        protected void dLogFormat(string msg, params object[] args)
        {
            if (dbg) LogFormat(msg, args);
        }

        protected void dLog(object obj)
        {
            if (dbg) Log(obj);
        }

        protected void LogFormat(string msg, params object[] args)
        {
            if (args.IsNullOrEmpty()) args = new object[0];
            Debug.Log(string.Format(msg, args), gameObject); // passing gameObject as context will ping the gameObject that we logged from when we click the log entry in the console!
        }

        protected void Log(object obj)
        {
            Debug.Log(obj, gameObject);
        }

        // static logs are useful when logging in nested system.object classes
        protected static void sLogFormat(string msg, params object[] args)
        {
            if (args.IsNullOrEmpty()) args = new object[0];
            Debug.Log(string.Format(msg, args));
        }

        protected static void sLog(object obj)
        {
            Debug.Log(obj);
        }

        #endregion

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

        // IVFWObject implementation
        #region
        public virtual Type GetSerializerType()
        {
            return SerializerBackend.DefaultType;
        }

        public virtual ISerializationLogic GetSerializationLogic()
        {
            return VFWSerializationLogic.Instance;
        }

        public virtual RuntimeMember[] GetSerializedMembers()
        {
            var logic = GetSerializationLogic();
            var members = logic.CachedGetSerializableMembers(GetType());
            return members;
        }

        public SerializationData GetSerializationData()
        {
            return _serializationData ?? (_serializationData = new SerializationData());
        }

        public virtual int GetPersistentId()
        {
            if (_id == -1)
                _id = GetInstanceID();
            return _id;
        }

        public virtual CategoryDisplay GetDisplayOptions()
        {
            return CategoryDisplay.BoxedMembersArea | CategoryDisplay.Headers | CategoryDisplay.BoxedHeaders;
        }

        [ContextMenu("Load behaviour state")]
        public virtual void DeserializeObject()
        {
            Serializer.DeserializeTargetFromData(this);
        }

        [ContextMenu("Save behaviour state")]
        public virtual void SerializeObject()
        {
            Serializer.SerializeTargetIntoData(this);
        }
        #endregion


        [SerializeField, HideInInspector] int _dirty = 0;

        public void MarkChanged()
        {
            dLog("Marked: " + name);
            _dirty = 0;
        }

        public virtual bool SemiAutomaticSerialization()
        {
            return false;
        }
    }
}
