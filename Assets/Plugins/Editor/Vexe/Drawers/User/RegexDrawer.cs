using System.Text.RegularExpressions;
using Vexe.Runtime.Types;

namespace Vexe.Editor.Drawers
{
	public class RegexDrawer<T> : CompositeDrawer<string, T> where T : RegexAttribute
	{
		private string recentValid;

		public override void OnUpperGUI() // doesn't really matter much which section we override.. we're just modifying the member value and not drawing anything
		{
			string current = memberValue ?? string.Empty;

			if (Regex.IsMatch(current, attribute.pattern))
			{
				recentValid = current;
			}
			else
			{
				memberValue = recentValid;
			}
		}
	}
}