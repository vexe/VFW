using UnityEngine;
using Vexe.Runtime.Types;

namespace Vexe.Editor.Drawers
{
	public abstract class ConstrainValueAttributeDrawer<TValue, TAttribute> : CompositeDrawer<TValue, TAttribute>
		where TAttribute : ConstrainValueAttribute
	{
		public override void OnLowerGUI()
		{
			memberValue = Constrain();
		}

		protected abstract TValue Constrain();
	}

	public class IntMinAttributeDrawer : ConstrainValueAttributeDrawer<int, MinAttribute>
	{
		protected override int Constrain()
		{
			return Mathf.Max(memberValue, attribute.intMin);
		}
	}

	public class FloatMinAttributeDrawer : ConstrainValueAttributeDrawer<float, MinAttribute>
	{
		protected override float Constrain()
		{
			return Mathf.Max(memberValue, attribute.floatMin);
		}
	}

	public class IntMaxAttributeDrawer : ConstrainValueAttributeDrawer<int, MaxAttribute>
	{
		protected override int Constrain()
		{
			return Mathf.Min(memberValue, attribute.intMax);
		}
	}

	public class FloatMaxAttributeDrawer : ConstrainValueAttributeDrawer<float, MaxAttribute>
	{
		protected override float Constrain()
		{
			return Mathf.Min(memberValue, attribute.floatMax);
		}
	}

	public class IntNumericClampAttributeDrawer : ConstrainValueAttributeDrawer<int, NumericClampAttribute>
	{
		protected override int Constrain()
		{
			return Mathf.Clamp(memberValue, attribute.intMin, attribute.intMax);
		}
	}

	public class FloatNumericClampAttributeDrawer : ConstrainValueAttributeDrawer<float, NumericClampAttribute>
	{
		protected override float Constrain()
		{
			return Mathf.Clamp(memberValue, attribute.floatMin, attribute.floatMax);
		}
	}
}