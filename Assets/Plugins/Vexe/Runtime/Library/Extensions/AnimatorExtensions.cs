using System;
using UnityEngine;

namespace Vexe.Runtime.Extensions
{
	public static class AnimatorExtensions
	{
        public static void Bools(this Animator animator, string name, bool value) { animator.SetBool(name, value); }
        public static bool Bools(this Animator animator, string name) { return animator.GetBool(name); }

        public static void Floats(this Animator animator, string name, float value) { animator.SetFloat(name, value); }
        public static float Floats(this Animator animator, string name) { return animator.GetFloat(name); }

        public static void Ints(this Animator animator, string name, int value) { animator.SetInteger(name, value); }
        public static int Ints(this Animator animator, string name) { return animator.GetInteger(name); }

		public static bool IsInState(this Animator animator, int layerIndex, string stateName) { return animator.GetCurrentAnimatorStateInfo(layerIndex).IsName(stateName); }
		public static bool IsInState(this Animator animator, int layerIndex, int stateHash) { return animator.GetCurrentAnimatorStateInfo(layerIndex).fullPathHash == stateHash; }
		public static bool IsInState(this Animator animator, string stateName) { return IsInState(animator, 0, stateName); }
		public static bool IsInState(this Animator animator, int stateHash) { return IsInState(animator, 0, stateHash); }
	}
}