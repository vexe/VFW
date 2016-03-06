using System;
using UnityEngine;
using Vexe.Runtime.Helpers;

namespace Vexe.Runtime.Types
{
    [Obsolete("Was used previously by BetterBehaviour which is now deprecated. " + 
              "Attribute is not reliable because there's no reliable place to " + 
              "initialize the field/property to the default value specified in the attribute")]
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
	public class DefaultAttribute : Attribute
	{
		public readonly int? intValue;
		public readonly float? floatValue;
		public readonly bool? boolValue;
		public readonly Vector3? vector3Value;
		public readonly Vector2? vector2Value;
		public readonly string stringValue;
		public int[] intArray;
		public float[] floatArray;
		public bool[] boolArray;
		public string[] stringArray;
		public int Enum { get; set; }
		public bool Instantiate { get; set; }

		public DefaultAttribute(bool[] boolArray)
		{
			this.boolArray = boolArray;
		}

		public DefaultAttribute(string[] stringArray)
		{
			this.stringArray = stringArray;
		}

		public DefaultAttribute(float[] floatArray)
		{
			this.floatArray = floatArray;
		}

		public DefaultAttribute(int[] intArray)
		{
			this.intArray = intArray;
		}

		public DefaultAttribute(int intValue)
		{
			this.intValue = intValue;
		}

		public DefaultAttribute(float floatValue)
		{
			this.floatValue = floatValue;
		}

		public DefaultAttribute(string stringValue)
		{
			this.stringValue = stringValue;
		}

		public DefaultAttribute(bool boolValue)
		{
			this.boolValue = boolValue;
		}

		public DefaultAttribute(float x, float y, float z)
		{
			vector3Value = new Vector3(x, y, z);
		}

		public DefaultAttribute(float x, float y)
		{
			vector2Value = new Vector2(x, y);
		}

		public DefaultAttribute()
		{
			Enum = -1;
		}

		public object Value
		{
			get
			{
				if (intValue.HasValue)		return intValue.Value;
				if (floatValue.HasValue)	return floatValue.Value;
				if (boolValue.HasValue)		return boolValue.Value;
				if (vector2Value.HasValue)  return vector2Value.Value;
				if (vector3Value.HasValue)  return vector3Value.Value;
				if (stringValue != null)	return stringValue;
				if (intArray != null)		return intArray;
				if (floatArray != null)		return floatArray;
				if (boolArray != null)		return boolArray;
				if (stringArray != null)	return stringArray;
				if (Enum != -1)				return Enum;
				if (Instantiate)			return null;
				throw new NotSupportedException("Value type not supported");
			}
		}
	}
}