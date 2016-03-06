using System;
using UnityEngine;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Helpers;
using Vexe.Runtime.Serialization;
using UnityObject = UnityEngine.Object;

namespace Vexe.Runtime.Types
{
    [Obsolete("Please use BaseBehaviour instead. Custom serialization hacks in Unity is fundamentally broken and leads to numerous headaches.")]
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
            if (IsModified(this, Serializer, serializationData))
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
            var members = RuntimeMember.CachedWrapMembers(GetType());
            for (int i = 0; i < members.Count; i++)
			{
				var member = members[i];
				member.Target = this;
				var defAttr = member.Info.GetCustomAttribute<DefaultAttribute>();
                if (defAttr != null)
				{ 
					var value = defAttr.Value;
					if (value == null && !member.Type.IsAbstract) // null means to instantiate a new instance
						value = member.Type.ActivatorInstance();
					member.Value = value;
				}
			}
		}

        public static bool IsModified(UnityObject target, SerializerBackend serializer, SerializationData data)
        {
            var members = serializer.Logic.CachedGetSerializableMembers(target.GetType());
            for (int i = 0; i < members.Length; i++)
            {
                var member    = members[i];
                var memberKey = SerializerBackend.GetMemberKey(member);
                member.Target = target;
                var value = member.Value;

                string prevState;
                if (!data.serializedStrings.TryGetValue(memberKey, out prevState))
                    return true;

                if (value.IsObjectNull() && prevState == "null")
                    return true;

                string curState = serializer.Serialize(member.Type, value, data.serializedObjects);

                if (prevState != null && prevState != curState)
                    return true;
            }

            return false;
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
