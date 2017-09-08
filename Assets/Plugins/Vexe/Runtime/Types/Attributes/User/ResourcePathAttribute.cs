using System;
using UnityObject = UnityEngine.Object;
using System.Reflection;

namespace Vexe.Runtime.Types
{

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
	public class ResourcePathAttribute : DrawnAttribute 
    {

        public Type ResourceType { get; set; }

        public ResourcePathAttribute(Type objectType = null) 
        {
	        if (objectType == null || !typeof(UnityObject).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo()))
	            ResourceType = typeof(UnityObject);
	        else
	            ResourceType = objectType;
	    }

    }

}