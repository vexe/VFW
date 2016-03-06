using UnityEngine;
using Vexe.Runtime.Extensions;

namespace Vexe.Runtime.Types
{
    /// <summary>
    /// Inherit this class in your scriptable objects to get the full editor power of Vfw
    /// This class does not define any custom serialization so it has no extra runtime overhead
    /// </summary>
    [DefineCategory("", 0, MemberType = CategoryMemberType.All, Exclusive = false, AlwaysHideHeader = true)]
    [DefineCategory("Dbg", 3f, Pattern = "^dbg")]
    public abstract class BaseScriptableObject : ScriptableObject
    {
        /// <summary>
        /// A persistent identifier used primarly from editor scripts to have editor data persist
        /// Could be used at runtime as well if you have any usages of a unique id
        /// Note this is not the same as GetInstanceID, as it seems to change when you reload scenes
        /// This id gets assigned only once and then serialized.
        /// </summary>
        [SerializeField, HideInInspector]
        private int _id = -1;

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

        // Logging
        #region
        [HideInInspector] public bool dbg;

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
            Debug.Log(string.Format(msg, args));
        }

        protected void Log(object obj)
        {
            Debug.Log(obj);
        }

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
        public EditorRecord Prefs = new EditorRecord();
#endif
    }
}