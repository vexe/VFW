using Vexe.Runtime.Types;

namespace VFWExamples
{
	public class PathExample : BaseBehaviour
	{
		//Alternatively, you can hold Ctrl and do a middle mouse click on the field
		//to show a selection window with all the gameObjects in the scene
		[Path, Comment("Drag-drop a gameObject or asset to this field")]
		public string fullPath;

		[Path(false)]
		public string simplePath;

		[Path(AbsoluteAssetPath = true)]
		public string someAssetPath;
	}
}