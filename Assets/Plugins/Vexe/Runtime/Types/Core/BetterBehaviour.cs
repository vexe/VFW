using UnityEngine;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Helpers;
using Vexe.Runtime.Serialization;

namespace Vexe.Runtime.Types
{
    [DefineCategory("", 0, MemberType = CategoryMemberType.All, Exclusive = false, AlwaysHideHeader = true)]
    [DefineCategory("Dbg", 3f, Pattern = "^dbg")]
    public abstract class BetterBehaviour : MonoBehaviour, ISerializationCallbackReceiver
    {
        [SerializeField]
        private SerializationData _serializationData;
        public SerializationData BehaviourData
        {
            get { return _serializationData ?? (_serializationData = new SerializationData()); }
            set { _serializationData = value; }
        }

        static SerializerBackend _serializer;
        public static SerializerBackend Serializer
        {
            get { return _serializer ?? (_serializer = new FullSerializerBackend()); }
        }

        /// <summary>
        /// A persistent identifier used primarly from editor scripts to have editor data persist
        /// Could be used at runtime as well if you have any usages of a unique id
        /// Note this is not the same as GetInstanceID, as it seems to change when you reload scenes
        /// This id gets assigned only once and then serialized.
        /// </summary>
        [SerializeField, HideInInspector]
        private int _id = -1;
        public int Id
        {
            get
            {
                if (_id == -1)
                    _id = GetInstanceID();
                return _id;
            }
        }

        public void OnBeforeSerialize()
        {
            if (RuntimeHelper.IsModified(this, Serializer, BehaviourData))
            {
                dLog("Serializing: " + GetType().Name);
                SerializeBehaviour();
            }
        }

        public void OnAfterDeserialize()
        {
            DeserializeBehaviour();
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

        public virtual void Reset()
        {
            RuntimeHelper.ResetTarget(this);
        }

        [ContextMenu("Load behaviour state")]
        public void DeserializeBehaviour()
        {
            Serializer.DeserializeDataIntoTarget(this, BehaviourData);
        }

        [ContextMenu("Save behaviour state")]
        public void SerializeBehaviour()
        {
            BehaviourData.Clear();
            Serializer.SerializeTargetIntoData(this, BehaviourData);
        }
    }
}
