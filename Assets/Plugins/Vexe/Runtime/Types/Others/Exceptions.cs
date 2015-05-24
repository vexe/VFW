using System;
using System.Collections;

namespace System
{
	public class vInvalidType : Exception
	{
		public vInvalidType(string msg) : base(msg) { }
		public vInvalidType() { }
	}

	public class vMemberNotFound : Exception
	{
		public vMemberNotFound(Type type, string name) : base(string.Format("Member {0} not found in type {1}", name, type)) { }
	}

	public class vTypeMismatch : Exception
	{
		public vTypeMismatch(Type expected, Type got) :
            base("Expected type: `" + expected.Name + "` but got: `" + got.Name + "`") { }
	}

    public class vInvalidCast : Exception
    {
        public vInvalidCast(string fromTypeName, string toTypeName)
            : base("Cannot cast from `" + fromTypeName + "` to `" + toTypeName + "`")
        {
        }

        public vInvalidCast(Type fromType, Type toType)
            : this(fromType.Name, toType.Name)
        {
        }

        public vInvalidCast(object value, Type toType)
            : this(value.GetType().Name, toType.Name)
        {
        }

        public vInvalidCast(object value, string toTypeName)
            : this(value.GetType().Name, toTypeName)
        {
        }
    }

    public class vIndexOutOfRange : Exception
    {
        public vIndexOutOfRange(int outOfRangeIndex, int totalCount)
            : base("Index `" + outOfRangeIndex + "` should be greater or equal to zero and less than the total count of `" + totalCount + "`")
        {
        }

        public vIndexOutOfRange(int outOfRangeIndex, IList list) : this(outOfRangeIndex, list.Count)
        {
        }
    }
}
