using System;
using UnityObject = UnityEngine.Object;

namespace Vexe.Runtime.Types
{

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
	public class ResourcePathAttribute : DrawnAttribute 
    {

        public Type ResourceType { get; set; }

        public ResourcePathAttribute(Type objectType = null) 
        {
	        if (objectType == null || !typeof(UnityObject).IsAssignableFrom(objectType))
	            ResourceType = typeof(UnityObject);
	        else
	            ResourceType = objectType;
	    }

    }

}