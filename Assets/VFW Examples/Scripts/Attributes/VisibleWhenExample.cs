using System;
using UnityEngine;
using Vexe.Runtime.Types;

namespace VFWExamples
{
	public class VisibleWhenExample : BaseBehaviour
	{
        public int value;

        [Comment("// visible when Flag1 == true")]
        [VisibleWhen("Flag1")]
        public int member1;

        [Comment("// visible when Flag1 == true && Flag2 == false")]
        [VisibleWhen("Flag1", "!Flag2")]
        public int member2;

        [Comment("// visible when Flag1 == true || Flag2 == true || Flag3 == true")]
        [VisibleWhen('|', "Flag1", "Flag2", "Flag3")]
        public int member3;

        public bool Flag1;
        public bool Flag2 { get { return value > 10; } }
        public bool Flag3() { return value < 5; }

        // just to make sure visibility works on system objects too and not just behaviours
        public NestedObject[] array;
	}

    [Serializable]
    public struct NestedObject
    {
        [VisibleWhen("Flag4")]
        public int member4;

        public bool Flag4;
    }
}
