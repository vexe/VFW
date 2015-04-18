//#define PROFILE

using System;
using UnityEngine;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Types;

namespace Vexe.Runtime.Serialization
{
    public abstract class SerializerBackend
    {
        public VFWSerializationLogic Logic;

        public void SerializeTargetIntoData(object target, SerializationData data)
        {
#if PROFILE
            Profiler.BeginSample("Getting members for: " + target);
#endif
            var members = Logic.GetCachedSerializableMembers(target.GetType());
#if PROFILE
            Profiler.EndSample();
#endif
            for (int i = 0; i < members.Count; i++)
            {
                var member    = members[i];
                member.Target = target;
                var value     = member.Value;

                if (value.IsObjectNull())
                    continue;

                try
                {
                    string memberKey = GetMemberKey(member);
#if PROFILE
                    Profiler.BeginSample("Serializing: " + member.Name);
#endif
                    string serializedState = Serialize(member.Type, value, data.serializedObjects);
#if PROFILE
                    Profiler.EndSample();
#endif
                    data.serializedStrings[memberKey] = serializedState;
                }
                catch (Exception e)
                {
                    Debug.LogError("Error serializing member {0} ({1}) in {2}: {3}. Stacktrace: {4}"
                         .FormatWith(member.Name, member.Type.GetNiceName(), target, e.Message, e.StackTrace));
                }
            }
        }

        public void DeserializeDataIntoTarget(object target, SerializationData data)
        {
            var members = Logic.GetCachedSerializableMembers(target.GetType());
            for(int i = 0; i < members.Count; i++)
            {
                var member    = members[i];
                var memberKey = GetMemberKey(member);
                member.Target = target;

                try
                {
                    string result;
                    if (data.serializedStrings.TryGetValue(memberKey, out result))
                    {
                        var value = Deserialize(member.Type, result, data.serializedObjects);
                        member.Value = value;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("Error deserializing member {0} ({1}) in {2}: {3}. Stacktrace: {4}"
                         .FormatWith(member.Name, member.Type.GetNiceName(), target, e.Message, e.StackTrace));
                }
            }
        }

        private Func<RuntimeMember, string> _getMemberKey;
        private string GetMemberKey(RuntimeMember member)
        {
            if (_getMemberKey == null)
            {
                _getMemberKey = new Func<RuntimeMember, string>(x =>
                    string.Format("{0}: {1} {2}", x.Info.MemberType.ToString(), x.TypeNiceName, x.Name)
                ).Memoize();
            }
            return _getMemberKey(member);
        }

        public abstract string Serialize(Type type, object graph, object context);

        public string Serialize(object graph, object context)
        {
            if (graph == null)
                return null;
            return Serialize(graph.GetType(), graph, context);
        }
        
        public string Serialize(object graph)
        {
            return Serialize(graph, null);
        }

        public abstract object Deserialize(Type type, string serializedState, object context);

        public T Deserialize<T>(string serializedState, object context)
        {
            return (T)Deserialize(typeof(T), serializedState, context);
        }

        public T Deserialize<T>(string serializedState)
        {
            return Deserialize<T>(serializedState, null);
        }
    }
}
