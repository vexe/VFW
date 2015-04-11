using System;
using UnityEngine;
using Vexe.Editor.Types;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Types;
using Random = UnityEngine.Random;

namespace Vexe.Editor.Drawers
{
	public abstract class BetterVectorDrawer<TVector> : AttributeDrawer<TVector, BetterVectorAttribute>
	{
		private const string kBtnReset     = "0";
		private const string kBtnNormalize = "1";
		private const string kBtnRandomize = "r";

        protected static float rand()
        {
            return Random.Range(-100, 100);
        }

		public override void OnGUI()
		{
			using (gui.Horizontal())
			{
				var current = memberValue;
				var newValue = GetField()(niceName, current);
				{
					if (!VectorEquals(current, newValue))
					{
						memberValue = newValue;
					}
				}

				gui.Space(12f);
				Foldout();
				gui.Space(-10f);
			}

			if (foldout)
			{
				using (gui.Horizontal())
				{
					DoButtons();
					gui.Space(25f);
				}
			}
		}

		protected abstract Func<string, TVector, TVector> GetField();
		protected abstract bool VectorEquals(TVector left, TVector right);

		private void DoButtons()
		{
			gui.FlexibleSpace();
			var option = Layout.sHeight(13f);
			if (gui.MiniButton("Copy", "Copy vector value", option))
				Copy();
			if (gui.MiniButton("Paste", "Paste vector value", option))
				memberValue = Paste();
			if (gui.MiniButton(kBtnRandomize, "Randomize values between [-100, 100]", MiniButtonStyle.ModMid, option))
				memberValue = Randomize();
			if (gui.MiniButton(kBtnNormalize, "Normalize", MiniButtonStyle.ModMid, option))
				memberValue = Normalize();
			if (gui.MiniButton(kBtnReset, "Reset", MiniButtonStyle.ModRight, option))
				memberValue = Reset();
		}

		protected abstract TVector Reset();
		protected abstract TVector Normalize();
		protected abstract TVector Randomize();
		protected abstract TVector Paste();
		protected abstract void Copy();
	}

	public class BetterVector2Drawer : BetterVectorDrawer<Vector2>
	{
		protected override void Copy()         { Clipboard.GetInstance().V2Value = memberValue; }
		protected override Vector2 Paste()     { return Clipboard.GetInstance().V2Value; }
		protected override Vector2 Randomize() { return new Vector2(rand(), rand()); }
		protected override Vector2 Normalize() { return Vector2.one; }
		protected override Vector2 Reset()     { return Vector2.zero; }

		protected override Func<string, Vector2, Vector2> GetField()
		{
			return gui.Vector2;
		}

		protected override bool VectorEquals(Vector2 left, Vector2 right)
		{
			return left.ApproxEqual(right);
		}
	}

	public class BetterVector3Drawer : BetterVectorDrawer<Vector3>
	{
		protected override void Copy()         { Clipboard.GetInstance().V3Value = memberValue; }
		protected override Vector3 Paste()     { return Clipboard.GetInstance().V3Value; }
		protected override Vector3 Randomize() { return new Vector3(rand(), rand(), rand()); }
		protected override Vector3 Normalize() { return Vector3.one; }
		protected override Vector3 Reset()     { return Vector3.zero; }

		protected override Func<string, Vector3, Vector3> GetField()
		{
			return gui.Vector3;
		}

		protected override bool VectorEquals(Vector3 left, Vector3 right)
		{
			return left.ApproxEqual(right);
		}
	}
}
