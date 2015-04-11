
namespace Vexe.Editor.GUIs
{
	public abstract partial class BaseGUI
	{
		public abstract void Space(float pixels);

		public abstract void FlexibleSpace();

		public void Space(float? pixels)
		{
			if (pixels.HasValue)
				Space(pixels.Value);
		}
	}
}