namespace Vexe.Runtime.Types
{
    /// <summary>
    /// Used in cateogry definitions to determine whether members matching the definintion
    /// are intersected (only the common results between filters are picked) or united (results are combined)
    /// </summary>
	public enum CategorySetOp
	{
        Union,
        Intersection
	};
}