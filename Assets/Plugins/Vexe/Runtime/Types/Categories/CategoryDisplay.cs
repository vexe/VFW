using System;

namespace Vexe.Runtime.Types
{
    [Flags]
	public enum CategoryDisplay
	{
	   MemberSplitter   = 1 << 0, // split line between members
	   CategorySplitter = 1 << 1, // split line between categories
	   BoxedMembersArea = 1 << 2, // a gui box wrapping categories
	   Headers          = 1 << 3, // show category header?
	   BoxedHeaders     = 1 << 4, // show headers in a gui box
	};
}
