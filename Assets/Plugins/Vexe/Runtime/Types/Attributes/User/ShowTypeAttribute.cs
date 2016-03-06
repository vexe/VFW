using System;

namespace Vexe.Runtime.Types
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
	public class ShowTypeAttribute : DrawnAttribute
	{
		/// <summary>
		/// The base Type to start from. All non-abstract child types of this type will be shown
		/// </summary>
		public readonly Type baseType;

		/// <summary>
		/// Show only component types attached to the game object?
		/// </summary>
		public bool FromThisGo { get; set; }

		public ShowTypeAttribute(Type baseType)
		{
			this.baseType = baseType;
		}
	}
}