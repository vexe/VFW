using System;
using System.Collections.Generic;
using Vexe.Runtime.Types;
using UnityObject = UnityEngine.Object;

namespace Vexe.Runtime.Serialization
{
	[Serializable]
	public class SerializationData
	{
		public List<UnityObject> serializedObjects = new List<UnityObject>();
		public StrStrDict serializedStrings        = new StrStrDict();

		public void Clear()
		{
			serializedObjects.Clear();
			serializedStrings.Clear();
		}

		public override string ToString()
		{
			return "Runtime Serialization Data";
		}
	}

	[Serializable]
	public class StrStrDict : KVPList<string, string>
	{
	}
}