using Vexe.Runtime.Types;

namespace VFWExamples
{
	public class ParagraphExample : BaseBehaviour
	{
		[Paragraph] public string p1;
		[Paragraph(3, 15)] public string p2;
	}
}
