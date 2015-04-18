using System;

namespace Vexe.Runtime.Exceptions
{
	public class InvalidTypeException : Exception
	{
		public InvalidTypeException(string msg) : base(msg) { }
		public InvalidTypeException() { }
	}

	public class MemberNotFoundException : Exception
	{
		public MemberNotFoundException(string msg) : base(msg) { }
	}

	public class TypeMismatchException : Exception
	{
		public TypeMismatchException(string msg) : base(msg) { }
	}

	public class WTFException : Exception
	{
		public WTFException(string msg) : base(msg) { }
	}
}