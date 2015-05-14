using System;
using System.Collections.Generic;
using UnityEngine;
using Vexe.Runtime.Types;

namespace VFWExamples
{
	
	public class GenericsExample : BetterBehaviour
	{
		[Comment("This might not serialize, depending on your serializer", 2)]
		public Tuple<Transform, Tuple<string, GameObject>> Tuple { get; set; }

		[Comment("This might not serialize, depending on your serializer", 2)]
		public List<List<List<Dictionary<string, IOwner<Vector3>>>>> nasty;

		public IOwner<GameObject> GoOwner { get; set; }
		public IOwner<Vector3> VecOwner   { get; set; }

        public Dictionary<string, GameObject> Dict;
	}

	public interface IOwner<T>
	{
		T Value { get; set; }
	}

	[Serializable]
	public class Vector3Owner : IOwner<Vector3>
	{
		public Vector3 Value { get; set; }
	}

	[Serializable]
	public class GameObjectOwner : IOwner<GameObject>
	{
		public GameObject Value { get; set; }
	}

	[Serializable]
	public class Tuple<TKey, TValue>
	{
		public TKey Key { get; set; }

		public TValue Value { get; set; }

		public override string ToString()
		{
			return string.Format("[{0}, {1}]", Key, Value);
		}
	}
}