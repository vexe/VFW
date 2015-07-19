using System;
using UnityEditor;
using UnityEngine;
using Vexe.Editor.Helpers;
using Vexe.Editor.Windows;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Helpers;
using Vexe.Runtime.Types;
using UnityObject = UnityEngine.Object;
using System.IO;

namespace Vexe.Editor.Drawers
{
	public class ResourcePathDrawer : AttributeDrawer<string, ResourcePathAttribute>
	{
		public override void OnGUI()
		{
			using (gui.Horizontal()) {
				gui.Label(displayText);


				UnityObject obj = null;
				if(memberValue != null) {
					obj = Resources.Load (memberValue);
				}

				bool isNull = memberValue == null;
				bool isResourceMissing = (obj == null && !isNull);
				gui.TextLabel(isNull ? "null" : (isResourceMissing? "(missing resource "+ memberValue +")" : memberValue));

				var fieldRect = gui.LastRect;
				{
					GUIHelper.PingField(fieldRect, obj, false);
				}
				if (gui.ClearButton("Clear resource"))
				{
					obj = null;
				}

				var drop = gui.RegisterFieldForDrop<UnityObject>(fieldRect,objects => objects[0],AcceptInsideResourcesFolder);
				if (drop != null)
				{
					memberValue = GetPath(drop);
				}
				else if(obj != null) {
					memberValue = GetPath(obj);
				}
				else if(!isResourceMissing){
					memberValue = null;
				}
			}


		}

		private bool AcceptInsideResourcesFolder(UnityObject[] gameObjects) {
			if (gameObjects.Length != 1) {
				return false;
			}

			UnityObject gameObject = gameObjects [0];
			var fullPath = AssetDatabase.GetAssetPath(gameObject);
			return fullPath.Contains ("/Resources/");
		}

		private string GetPath(UnityObject input)
		{
			var fullPath = AssetDatabase.GetAssetPath(input);
			var resourcesIndex = fullPath.IndexOf ("/Resources/");
			var relativePath = fullPath.Substring (resourcesIndex + 11);
			var result = PathWithoutExtension (relativePath);

			return result;
		}

		private string PathRelativeToResources(string path) {
			return path.Substring (17);
		}

		private string ResourcesBasePath() {
			return "Assets/Resources/";
		}

		private string PathWithoutExtension(string path) {
			string extension = Path.GetExtension(path);
			return path.Substring(0, path.Length - extension.Length);
		}
	}
}