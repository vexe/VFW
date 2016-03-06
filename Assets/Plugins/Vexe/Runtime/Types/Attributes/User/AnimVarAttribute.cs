using System;
using UnityEngine;

namespace Vexe.Runtime.Types {
    [Flags]
    public enum ParameterType {
        Float = 1,
        Int = 2,
        Bool = 4,
        Trigger = 8,
        All = 15
    }

    /// <summary>
    /// Apply this to a string to get a popup of all the available variables in the Animator component that's attached to the owner's gameObject
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
    public class AnimVarAttribute : DrawnAttribute {
        
        private ParameterType _filter = ParameterType.All;

        /// <summary>
        /// A filter on the types of 
        /// </summary>
	    public ParameterType Filter 
        {
            get { return _filter;  }
            set { _filter = value; }
        }

        /// <summary>
        /// A method to get the animator in case it wasn't attached to the target's gameObject
        /// If this was left out, the drawer will use the target's gameObject to get the animator from
        /// The method should return an Animator and take no parameters
        /// </summary>
        public string GetAnimatorMethod { get; set; }

        /// <summary>
        /// Tries to auto assing the string field/property value to the matching variable name. ex:
        /// if you field name was "idleAnim" or "playerDeathAnim" etc and AutoMatch was "Anim",
        /// then it will try to find if "Idle" or "PlayerDeath" etc is a variable defined in the animator,
        /// if so then use it.
        /// Note though this will automatically assign your variables everytime the drawer is created 
        /// so make sure you use the right value otherwise you might get unexpected results
        /// </summary>
        public string AutoMatch { get; set; }
    }
}