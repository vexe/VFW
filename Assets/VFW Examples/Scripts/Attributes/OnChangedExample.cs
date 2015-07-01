using System.Collections.Generic;
using UnityEngine;
using Vexe.Runtime.Types;

namespace VFWExamples
{
	public class OnChangedExample : BetterBehaviour
	{
		// when this string changes, set the `tag` property to the new value and log it
		[Tags, OnChanged(Set = "tag", Call = "Log")] // could have also written OnChanged("Log", Set = "tag")
		public string playerTag;

		// if any vector of this array changes, we set the `position` property to that new vector
		[PerItem, OnChanged(Set = "position")]
		public Vector3[] vectors;

		// if any value of this dictionary changes, set our scale to that value
		// you could use PerKey to apply attributes on the dictionary's keys instead of values
		[PerValue, OnChanged(Set = "localScale")]
		public Dictionary<string, Vector3> dictionary;

		public Vector3 position
		{
			get { return transform.position; }
			set { transform.position = value; }
		}

		public Vector3 localScale
		{
			get { return transform.localScale; }
			set { transform.localScale = value; }
		}
	}
}
