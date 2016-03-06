using UnityEngine;
using Vexe.Runtime.Types;

namespace VFWExamples
{
    public class AnimVarExample : BaseBehaviour
    {
        [AnimVar(AutoMatch = "Var")] // this will try to auto-assign "Is Dead" to this field
        public string isDeadVar;

        [PerItem, AnimVar]
        public string[] Variables;

        [AnimVar(GetAnimatorMethod = "GetAnim")]
        public string anotherVar;

        // Also works on ints, gets you the hash/id value of the variable instead
        [AnimVar]
        public int animHashVar;

        // Can also get you the AnimatorControllerParameter object itself if need be
        [AnimVar]
        public AnimatorControllerParameter ACPVar;

        // Can be filtered based on type
        [AnimVar(Filter = ParameterType.Bool)]
        public int boolVar;

        // The filter is a mask, so a group of types can be included
        [AnimVar(Filter = ParameterType.Float | ParameterType.Int)]
        public int floatOrIntVar;

        private Animator GetAnim()
        {
            // well this is pointless because it would attempt to get the animator from this gameObject
            // by default without having us to specify a method.
            // but imagine we're getting an animtor from somewhere else...
            return GetComponent<Animator>();
        }
    }
}
