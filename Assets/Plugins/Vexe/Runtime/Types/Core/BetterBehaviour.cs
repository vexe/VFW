//#define DBG

using UnityEngine;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Serialization;

namespace Vexe.Runtime.Types
{
    [DefineCategory("", 0, MemberType = MemberType.All, Exclusive = false, AlwaysHideHeader = true)]
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
        /// A unique identifier used primarly from editor scripts to have editor data persist
        /// Could be used at runtime as well if you have any usages of a unique id
        /// </summary>
        [SerializeField, HideInInspector]
        private int _id = -1;
        static int counter;
        public int Id
        {
            get
            {
                if (_id == -1)
                    _id = counter++;
                return _id;
            }
        }

        public void OnBeforeSerialize()
        {
#if DBG
            Log("Serializing " + GetType().Name);
#endif
            BehaviourData.Clear();
            Serializer.SerializeTargetIntoData(this, BehaviourData);
        }

        public void OnAfterDeserialize()
        {
#if DBG
            Log("Deserializing " + GetType().Name);
#endif
            Serializer.DeserializeDataIntoTarget(this, BehaviourData);
        }

        // Logging
        #region
        public bool dbg;

        protected void dbgLogFormat(string msg, params object[] args)
        {
            if (dbg) LogFormat(msg, args);
        }

        protected void dbgLog(object obj)
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
            LogFormat(obj.ToString(), null);
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
            RuntimeUtils.ResetTarget(this);
        }
    }
}
