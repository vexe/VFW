using System;
using System.Collections.Generic;
using UnityEngine;
using Vexe.Runtime.Types;

namespace VFWExamples
{
	
	public class InterfacesExample : BetterBehaviour
	{
		public ITest test0;
		public ITest[] test1;
		public List<ITest> test2;
	}

	public interface IAnother
	{
		TestObject2 Test { get; set; }
	}

	public interface ITest
	{
		Vector3 Vector { get; set; }
	}

	[Serializable]
	public class TestObject2 : ITest
	{
		public Vector3 Vector { get; set; }
	}

	[Serializable]
	public class TestObject1 : ITest, IAnother
	{
		public int x, y;

		public Vector3 Vector   { get; set; }
		public TestObject2 Test { get; set; }
	}

	
	public class UnityImplementor1 : BetterBehaviour, ITest
	{
		public Vector3 Vector { get; set; }
		[Show] public void Print() { print("hello 1"); }
	}

	
	public class UnityImplementor2 : BetterBehaviour, ITest
	{
		public Vector3 Vector { get; set; }
		[Show] public void Print() { print("hello 2"); }
	}
}