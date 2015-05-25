using UnityEngine;
using Vexe.Runtime.Types;

namespace VFWExamples
{
    /// <summary>
    /// An example showing how to explicitly specify what members to serialize
    /// This will ignore any serialization logic and just serialize the specified members
    /// </summary>
    public class SerializeExplicitMembersExample : BetterBehaviour
    {
        private int _backingField;

        // even though our default logic only allows the serialization of auto-properties,
        // we can still serialize this property by specifying it in GetSerializableMembers
        // remember: all logic is ignored, WYSIWYG (What you specify is what you get)
        [Display(0f)] public int serialized1
        {
            get { return _backingField; }
            set { _backingField = value; }
        }

        public string serialized2;

        public GameObject serialized3;

        // [Serialize] here has no effect because this member is not specified in GetSerializableMembers
        [Show, Serialize] public float nonSerialized1;

        [Show] public double nonSerialized2;

        public override RuntimeMember[] GetSerializedMembers()
        {
            return RuntimeMember.WrapMembers(GetType(),
                "serialized1", "serialized2", "serialized3");
        }
    }
}
