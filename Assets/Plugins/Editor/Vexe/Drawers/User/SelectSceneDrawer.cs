using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using Vexe.Editor.Windows;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Types;

namespace Vexe.Editor.Drawers
{
	public class SelectSceneDrawer : CompositeDrawer<string, SelectSceneAttribute>
	{
		protected override void Initialize()
		{
			if (memberValue == null)
				memberValue = string.Empty;
		}

		public override void OnRightGUI()
		{
			if (gui.SelectionButton())
			{
				Func<string[]> getScenes = () =>
						Directory.GetFiles("Assets", "*.unity", SearchOption.AllDirectories)
								 .Select(f => f.Substring(f.IndexOf("Assets") + 6).RemoveExtension())
								 .ToArray();

				Func<string, string> getSceneName = path =>
					path.Substring(path.Replace('\\', '/').LastIndexOf('/') + 1);

				var dictionary = new KVPList<string, string>();
				var allScenes  = getScenes();
				foreach(var s in allScenes)
					dictionary.Add(getSceneName(s), s);

				Func<Func<string[]>, string, Tab<string>> sceneTab = (scenes, title) =>
					new Tab<string>(
						@getValues    : scenes,
						@getCurrent   : () => dictionary.ContainsKey(memberValue) ? dictionary[memberValue] : memberValue,
						@setTarget    : s => memberValue = getSceneName(s),
						@getValueName : s => s,
						@title        : title
					);

				var buildScenes = EditorBuildSettings.scenes.Select(s => s.path);

				SelectionWindow.Show("Select scene",
					sceneTab(getScenes, "All"),
					sceneTab(getScenes().Where(s => buildScenes.Any(bs => Regex.Replace(bs, "/", "\\").Contains(s))).ToArray, "Build")
				);
			}
		}
	}
}