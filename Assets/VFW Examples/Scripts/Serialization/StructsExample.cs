using System;
using System.Collections.Generic;
using UnityEngine;

namespace Vexe.Runtime.Types.Examples
{
	
	public class StructsExample : BetterBehaviour
	{
		public MyStruct0 structField;
		public MyStruct1[] StructArrayProperty { get; set; }
		public Dictionary<string, MyStruct1> dict;
	}

	public struct MyStruct0
	{
		[Serialize]
		private GameObject Go;
		public MyStruct1 struct1;
	}

	public struct MyStruct1
	{
		[Serialize]
		private float value;
		public Vector3 vector;
	}
}