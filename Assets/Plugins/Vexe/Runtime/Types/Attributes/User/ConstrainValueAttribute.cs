using UnityEngine;

namespace Vexe.Runtime.Types
{
	public abstract class ConstrainValueAttribute : CompositeAttribute
	{
		public ConstrainValueAttribute()
		{
		}

		public ConstrainValueAttribute(int id) : base(id)
		{
		}
	}
}