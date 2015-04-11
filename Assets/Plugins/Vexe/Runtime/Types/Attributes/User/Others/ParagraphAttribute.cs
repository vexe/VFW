using System;

namespace Vexe.Runtime.Types
{
	/// <summary>
	/// Similar to Unity's TextAreaAttribute
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
	public class ParagraphAttribute : DrawnAttribute
	{
		public readonly int minNumLines, maxNumLines;

		public ParagraphAttribute(int minNumLines, int maxNumLines)
		{
			this.minNumLines = minNumLines;
			this.maxNumLines = maxNumLines;
		}

		public ParagraphAttribute() : this(1, 10)
		{
		}
	}
}
