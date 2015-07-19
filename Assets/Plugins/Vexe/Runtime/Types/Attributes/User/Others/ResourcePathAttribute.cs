using UnityEngine;
using System.Collections;
using Vexe.Runtime.Types;
using System;

namespace Vexe.Runtime.Types
{

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
	public class ResourcePathAttribute : DrawnAttribute
{
	public ResourcePathAttribute()
	{
	}
}

}