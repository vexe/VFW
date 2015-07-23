using UnityEngine;
using System.Collections;
using Vexe.Runtime.Types;

public class ResourcePathExample : BaseBehaviour {

	[Comment("Drag and drop a file here. Only objects inside the Resources folder will be accepted")]
	[ResourcePath]
	public string topLevelResource;

	[Comment("Nested paths are also supported. Note that the file extension is note store so you can use Resources.Load() directly")]
	[ResourcePath]
	public string nestedResource;

	[ResourcePath]
	public string missingResource;

}
