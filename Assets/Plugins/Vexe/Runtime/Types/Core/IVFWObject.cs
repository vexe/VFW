using System;
using Vexe.Runtime.Serialization;

namespace Vexe.Runtime.Types
{ 
    public interface IVFWObject
    {
        Type GetSerializerType();
        ISerializationLogic GetSerializationLogic();
        RuntimeMember[] GetSerializedMembers();
        SerializationData GetSerializationData();
        int GetPersistentId();
        CategoryDisplay GetDisplayOptions();
        void SerializeObject();
        void DeserializeObject();
        void MarkChanged();
        bool SemiAutomaticSerialization();
    }
}
