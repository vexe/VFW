using UnityEngine;
using Vexe.Runtime.Types;

namespace Vexe.Editor.Drawers
{
	public abstract class ConstrainValueDrawer<TValue, TAttribute> : CompositeDrawer<TValue, TAttribute>
		where TAttribute : ConstrainValueAttribute
	{
		public override void OnLowerGUI()
		{
			memberValue = Constrain();
		}

		protected abstract TValue Constrain();
	}

	public class iMinDrawer : ConstrainValueDrawer<int, iMinAttribute>
	{
		protected override int Constrain()
		{
			return Mathf.Max(memberValue, attribute.min);
		}
	}

	public class fMinDrawer : ConstrainValueDrawer<float, fMinAttribute>
	{
		protected override float Constrain()
		{
			return Mathf.Max(memberValue, attribute.min);
		}
	}

	public class iMaxDrawer : ConstrainValueDrawer<int, iMaxAttribute>
	{
		protected override int Constrain()
		{
			return Mathf.Min(memberValue, attribute.max);
		}
	}

	public class fMaxDrawer : ConstrainValueDrawer<float, fMaxAttribute>
	{
		protected override float Constrain()
		{
			return Mathf.Min(memberValue, attribute.max);
		}
	}

	public class iClampDrawer : ConstrainValueDrawer<int, iClampAttribute>
	{
		protected override int Constrain()
		{
			return Mathf.Clamp(memberValue, attribute.min, attribute.max);
		}
	}

	public class fClampDrawer : ConstrainValueDrawer<float, fClampAttribute>
	{
		protected override float Constrain()
		{
			return Mathf.Clamp(memberValue, attribute.min, attribute.max);
		}
	}
}