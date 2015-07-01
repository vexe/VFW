using System;

namespace Vexe.Runtime.Types
{
	/// <summary>
	/// Defines multiple categories at once using their full paths
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class DefineCategoriesAttribute : Attribute
	{
		public readonly string[] names;

		public DefineCategoriesAttribute(params string[] names)
		{
			this.names = names;
		}
	}
}
