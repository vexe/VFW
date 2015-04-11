using Vexe.Runtime.Extensions;
using Vexe.Runtime.Types;

namespace Vexe.Editor.Drawers
{
	public abstract class AttributeDrawer<TObject, TAttribute> : ObjectDrawer<TObject> where TAttribute : DrawnAttribute
	{
		protected TAttribute attribute { private set; get; }

		protected sealed override void OnInternalInitialization()
		{
			attribute = attributes.GetAttribute<TAttribute>();
		}
	}
}