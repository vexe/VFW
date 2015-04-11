using System;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Types;

namespace Vexe.Editor.Drawers
{
	public abstract class CompositeDrawer<T, A> : BaseDrawer where A : CompositeAttribute
	{
		protected A attribute { private set; get; }

		protected T memberValue
		{
			get { return (T)member.Value; }
			set { member.Value = value; }
		}

		protected sealed override void OnInternalInitialization()
		{
			attribute = attributes.GetAttribute<A>();
		}

		public sealed override void OnGUI()
		{
		}
	}
}