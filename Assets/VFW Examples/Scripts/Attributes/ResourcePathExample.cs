using UnityEngine;
using System.Collections;
using Vexe.Runtime.Types;

public class ResourcePathExample : BaseBehaviour {

	[Comment("Enter any object here. It's resource path will be saved to the string.")]
	[ResourcePath]
	public string generalResource;
	
	[Comment("Object types can be filtered by providing a type object in the attribute declaration.")]
	[ResourcePath(typeof(AudioClip))]
	public string audioClipResource;
	
	[Comment("Providing no type, or a non-asset type will default to the general Unity Object type.")]
	[ResourcePath(typeof(string))]
	public string attemptAtStringResource;
	
	[Comment("Enter any non-asset object here. It will provide a warning saying it doesn't save objects that aren't assets.")]
	[ResourcePath]
	public string nonAssetPath;
	
	[Comment("Enter any non-resource asset object here. It will provide a warning saying it doesn't save objects that aren't Resources.")]
	[ResourcePath]
	public string nonResourcePath;
	
	[Comment("If a Resource is deleted, the value will reset to None (null).")]
	[ResourcePath]
	public string missingResource;

}
