using System;
using UnityEngine;
using Vexe.Runtime.Types;

namespace VFWExamples
{
	public class EnumsExample : BaseBehaviour
	{
		// you can use the 'id' number to determine which drawer to draw first, the filter or the selection button
		// a drawer with a lower number gets drawn first
		// if no id is specified a value of -1 is used
		[FilterEnum(0), SelectEnum(1)]
		public KeyCode actionKey;

		[EnumMask]
		public Permissions permission;

		// Remark: For bit-wise operations on the enum to give you the results you'd expect,
		// the enums have to have values of power of 2. Here's why http://stackoverflow.com/questions/9811114/why-do-enum-permissions-often-have-0-1-2-4-values
		[Flags]
		public enum Permissions
		{
			Read    = 1 << 0, // 0001
			Write   = 1 << 1, // 0010
			Execute = 1 << 2, // 0100

			/*
			 * same as:
			 * Read =		1,
			 * Write =		2,
			 * Execute =	4,
			 */
		}
	}
}