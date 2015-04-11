namespace Vexe.Runtime.Types.Examples
{
	
	public class PathExample : BetterBehaviour
	{
		//Alternatively, you can hold Ctrl and do a middle mouse click on the field
		//to show a selection window with all the gameObjects in the scene
		[Path, Comment("Drag-drop a gameObject or asset to this field")]
		public string FullPath { get; set; }

		[Path(false)]
		public string simplePath;

		[Path(AbsoluteAssetPath = true)]
		public string someAssetPath;
	}
}