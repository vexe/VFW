using System.Collections.Generic;
using UnityEngine;

namespace Vexe.Runtime.Extensions
{
	public static class LayerMaskExtensions
	{
		public static LayerMask Create(string[] layerNames)
		{
			return NamesToMask(layerNames);
		}

		public static LayerMask Create(int[] layerNumbers)
		{
			return LayerNumbersToMask(layerNumbers);
		}

		public static LayerMask NamesToMask(string[] layerNames)
		{
			var ret = (LayerMask)0;
			for (int i = 0; i < layerNames.Length; i++)
			{
                var name = layerNames[i];
				ret |= (1 << LayerMask.NameToLayer(name));
			}
			return ret;
		}

		public static LayerMask LayerNumbersToMask(int[] layerNumbers)
		{
			LayerMask ret = (LayerMask)0;
			for (int i = 0; i < layerNumbers.Length; i++)
			{
                var layer = layerNumbers[i];
				ret |= (1 << layer);
			}
			return ret;
		}

		public static LayerMask Inverse(this LayerMask original)
		{
			return ~original;
		}

		public static LayerMask AddToMask(this LayerMask original, string[] layerNames)
		{
			return original | NamesToMask(layerNames);
		}

		public static LayerMask RemoveFromMask(this LayerMask original, string[] layerNames)
		{
			LayerMask invertedOriginal = ~original;
			return ~(invertedOriginal | NamesToMask(layerNames));
		}

		public static LayerMask RemoveFromMask(this LayerMask original, int[] layers)
		{
			int len = layers.Length;
			var names = new string[len];
			for (int i = 0; i < len; i++)
			{
				names[i] = LayerMask.LayerToName(layers[i]);
			}
			return RemoveFromMask(original, names);
		}

		public static bool Contains(this LayerMask mask, LayerMask other)
		{
			// Convert the object's layer to a bitfield for comparison
			int bitMask = 1 << other;
			return (mask.value & bitMask) > 0;
		}

		public static string[] MaskToNames(this LayerMask original)
		{
			var output = new List<string>();

			for (int i = 0; i < 32; ++i)
			{
				int shifted = 1 << i;
				if ((original & shifted) == shifted)
				{
					string layerName = LayerMask.LayerToName(i);
					if (!string.IsNullOrEmpty(layerName))
					{
						output.Add(layerName);
					}
				}
			}
			return output.ToArray();
		}

		public static string MaskToString(this LayerMask original)
		{
			return MaskToString(original, ", ");
		}

		public static string MaskToString(this LayerMask original, string delimiter)
		{
			return string.Join(delimiter, MaskToNames(original));
		}
	}
}
