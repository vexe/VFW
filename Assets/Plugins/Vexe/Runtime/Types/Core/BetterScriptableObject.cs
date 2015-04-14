//#define DBG

using System;
using UnityEngine;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Serialization;

namespace Vexe.Runtime.Types
{
    [DefineCategory("", 0, MemberType = MemberType.All, Exclusive = false, AlwaysHideHeader = true)]
    [DefineCategory("Dbg", 3f, Pattern = "^dbg")]
    public abstract class BetterScriptableObject : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField]
        private SerializationData _serializationData;
        public SerializationData ObjectData
        {
            get { return _serializationData ?? (_serializationData = new SerializationData()); }
        }

        private static SerializerBackend _serializer;
        public static SerializerBackend Serializer
        {
            get { return _serializer ?? (_serializer = new FullSerializerBackend()); }
        }

        static int counter;
        [SerializeField, HideInInspector]
        private int id = -1;
        public int Id
        {
            get
            {
                if (id == -1)
                    id = counter++;
                return id;
            }
        }

        public void OnBeforeSerialize()
        {
#if DBG
            Log("Saving " + GetType().Name);
#endif
            ObjectData.Clear();
            Serializer.SerializeTargetIntoData(this, ObjectData);
        }

        public void OnAfterDeserialize()
        {
#if DBG
            Log("Loading " + GetType().Name);
#endif
            Serializer.DeserializeDataIntoTarget(this, ObjectData);
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
            Debug.Log(string.Format(msg, args));
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