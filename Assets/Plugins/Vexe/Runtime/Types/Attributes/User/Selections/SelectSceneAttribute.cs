namespace Vexe.Runtime.Types
{
	/// <summary>
	/// Annotate strings with this to display a selection button on the right
	/// to have you able to select a scene from all the available scenes in the project
	/// </summary>
	public class SelectSceneAttribute : CompositeAttribute
	{
		public SelectSceneAttribute()
			: this(-1)
		{
		}

		public SelectSceneAttribute(int id)
			: base(id)
		{
		}
	}
}