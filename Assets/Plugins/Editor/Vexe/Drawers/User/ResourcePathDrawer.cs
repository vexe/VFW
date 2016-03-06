using System;
using UnityEditor;
using UnityEngine;
using Vexe.Runtime.Types;
using UnityObject = UnityEngine.Object;
using System.Text.RegularExpressions;

namespace Vexe.Editor.Drawers
{
	public class ResourcePathDrawer : AttributeDrawer<string, ResourcePathAttribute> {
        
	    private Type resourceType;
	    private UnityObject target;

	    protected override void Initialize() {
	        resourceType = attribute.ResourceType;
	        target = Resources.Load(memberValue);
	    }

	    public override void OnGUI() {

	        using (gui.Vertical()) {
                using (gui.Horizontal()) {
                    target = gui.Object(displayText, target, resourceType, false);

                    if (gui.ClearButton("Resource")) {
                        target = null;
                    }

                    if (target == null)
                        memberValue = "";
                    else
                        memberValue = Regex.Replace(AssetDatabase.GetAssetPath(target), ".*Resources/(.*)\\..*", "$1");
                }
	            if (target != null) {
	                using (gui.Indent()) {
                        if (!AssetDatabase.Contains(target))
                            gui.Label("Object is not an Asset, and will not be saved.");
                        else if (!IsResource(target))
                            gui.Label("Object is not a Resource, and will not be saved.");
                        else
                            gui.Label("Resource Path: " + memberValue);
                    }
	            }
            }
		}

		private bool IsResource(UnityObject input) {
		    return Regex.IsMatch(AssetDatabase.GetAssetPath(input), ".*Resources/.*");
		}
	}
}