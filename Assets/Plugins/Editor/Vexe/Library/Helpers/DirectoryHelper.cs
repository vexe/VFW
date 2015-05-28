using System.IO;
using System.Linq;

namespace Vexe.Editor.Helpers
{
	public static class DirectoryHelper
	{
		/// <summary>
		/// Lazy-gets a directory path to a directory named 'dir'
		/// If 'current' exists it is returned otherwise it gets assigned to GetDirectoryPath(dir)
		/// and then returned
		/// </summary>
		public static string LazyGetDirectoryPath(ref string current, string dir)
		{
			return Directory.Exists(current) ? current : current = GetDirectoryPath(dir);
		}

		/// <summary>
		/// Searches the project's hierarchy (starting from "Assets") searching for the specified directory name
		/// Returns the directory's full path relative to the project if found
		/// Throwns a DirectoryNotFoundException otherwise
		/// </summary>
		public static string GetDirectoryPath(string dir)
		{
			var dirs = Directory.GetDirectories("Assets", "*" + dir + "*", SearchOption.AllDirectories);
			string path = dirs.FirstOrDefault(d => Directory.Exists(d));
			if (string.IsNullOrEmpty(path))
				throw new DirectoryNotFoundException(string.Format("`{0}` directory was not found - maybe you renamed it?", dir));
			return path;
		}
	}
}
