using System;
using UnityEngine;

namespace Vexe.Runtime.Types
{
	/// <summary>
	/// An Animator wrapper that provides easy access to the underlying animator variables and functions
	/// </summary>
	[RequireComponent(typeof(Animator))]
	public class BetterAnimator : BetterBehaviour
	{
		// the first RequireComponent assures that the component is going to be available/attached
		// this requirement will make sure that the added animator is assigned to this field
		// so we don't have to do the assignment in OnAwake etc
		[Serialize, RequiredFromThis]
		private Animator animator;

		[Serialize, Hide] private IndexedBoolean bools;
		[Serialize, Hide] private IndexedFloat floats;
		[Serialize, Hide] private IndexedInteger ints;

		/// <summary>
		/// A quick access to the animator's integer variables
		/// ex:
		/// anim.Ints["Counter"]++;
		/// ...
		/// if (anim.Ints["SomeInt"] == someValue)
		/// ...
		/// </summary>
		public IndexedInteger Ints { get { return ints; } }

		/// <summary>
		/// A quick access to the animator's boolean variables
		/// ex:
		/// anim.Bools["IsDead"] = health <= 0;
		/// ...
		/// if (anim.Bools["IsShooting"])
		/// ...
		/// </summary>
		public IndexedBoolean Bools { get { return bools; } }

		/// <summary>
		/// A quick access to the animator's float variables
		/// ex:
		/// anim.Floats["Speed"] = 10;
		/// ...
		/// if (anim.Floats["Direction"] > 0)
		/// ...
		/// </summary>
		public IndexedFloat Floats { get { return floats; } }

		/// <summary>
		/// The underlying animator's animation playback speed
		/// </summary>
		public float speed
		{
			get { return animator.speed; }
			set { animator.speed = value; }
		}

		private void Awake()
		{
			if (bools.IsNull()) bools   = new IndexedBoolean() { animator = animator };
			if (ints.IsNull()) ints     = new IndexedInteger() { animator = animator };
			if (floats.IsNull()) floats = new IndexedFloat()   { animator = animator };
		}

		/// <summary>
		/// Sets the trigger parameter to active
		/// </summary>
		public void SetTrigger(string name)
		{
			animator.SetTrigger(name);
		}

		/// <summary>
		/// Sets the trigger parameter to active
		/// </summary>
		public void SetTrigger(int id)
		{
			animator.SetTrigger(id);
		}

		/// <summary>
		/// Resets the trigger parameter to false
		/// </summary>
		public void ResetTrigger(string id)
		{
			animator.ResetTrigger(name);
		}

		/// <summary>
		/// Resets the trigger parameter to false
		/// </summary>
		public void ResetTrigger(int id)
		{
			animator.ResetTrigger(id);
		}

		/// <summary>
		/// Is the animator in the state specified by 'stateName' and 'layerIndex'?
		/// </summary>
		public bool IsInState(int layerIndex, string stateName)
		{
			return animator.GetCurrentAnimatorStateInfo(layerIndex).IsName(stateName);
		}

		/// <summary>
		/// Is the animator in the state specified by 'stateHash' and 'layerIndex'?
		/// </summary>
		public bool IsInState(int layerIndex, int stateHash)
		{
			return animator.GetCurrentAnimatorStateInfo(layerIndex).fullPathHash == stateHash;
		}

		/// <summary>
		/// Is the animator in the state specified by 'stateName' and a layer index of 0?
		/// </summary>
		public bool IsInState(string stateName)
		{
			return IsInState(0, stateName);
		}

		/// <summary>
		/// Is the animator in the state specified by 'stateHash' and a layer index of 0?
		/// </summary>
		public bool IsInState(int stateHash)
		{
			return IsInState(0, stateHash);
		}

		public abstract class IndexedMecanimVariable<T>
		{
			public Animator animator;
			public abstract T this[string key] { get; set; }
		}

		public class IndexedBoolean : IndexedMecanimVariable<bool>
		{
			public override bool this[string key]
			{
				get { return animator.GetBool(key); }
				set { animator.SetBool(key, value); }
			}
		}

		public class IndexedInteger : IndexedMecanimVariable<int>
		{
			public override int this[string key]
			{
				get { return animator.GetInteger(key); }
				set { animator.SetInteger(key, value); }
			}
		}

		public class IndexedFloat : IndexedMecanimVariable<float>
		{
			public override float this[string key]
			{
				get { return animator.GetFloat(key); }
				set { animator.SetFloat(key, value); }
			}
		}
	}

	public static class IndexedMecanimVariablesExtensions
	{
		public static bool IsNull<T>(this BetterAnimator.IndexedMecanimVariable<T> variable)
		{
			return variable == null || variable.animator == null || variable.animator.Equals(null);
		}
	}
}
