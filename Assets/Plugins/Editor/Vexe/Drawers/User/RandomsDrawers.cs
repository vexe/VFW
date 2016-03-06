using Vexe.Editor.GUIs;
using Vexe.Runtime.Types;
using Random = UnityEngine.Random;

namespace Vexe.Editor.Drawers
{
	public abstract class RandomDrawer<T, A> : CompositeDrawer<T, A> where A : CompositeAttribute
	{
		protected override void Initialize()
		{
			// Randomize once when we're initialized
			if (!foldout)
			{
				foldout = true;
				Randomize();
			}
		}

		public override void OnRightGUI()
		{
			if (gui.MiniButton("R", "Randomize", MiniButtonStyle.Right))
				Randomize();
		}

		protected abstract void Randomize();
	}

	public class fRandDrawer : RandomDrawer<float, fRandAttribute>
	{
		protected override void Randomize()
		{
			memberValue = Random.Range(attribute.min, attribute.max);
		}
	}

	public class iRandDrawer : RandomDrawer<int, iRandAttribute>
	{
		protected override void Randomize()
		{
			memberValue = Random.Range(attribute.min, attribute.max);
		}
	}
}