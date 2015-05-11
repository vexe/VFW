using System;
using System.Text.RegularExpressions;

namespace Vexe.Runtime.Types
{
	/// <summary>
	/// Inherit this attribute if you want to have attributes that you could composite/decorate
	/// Composite attributes are used by CompositeDrawers
	/// Note that in your composite drawers you should only add decorations and modify the member value
	/// you shouldn't manipulate the way the member is drawn, this is what the DrawnAttribute is for
	/// Example of composite attributes: Comment, Whitespace, Min, Max, Regex, etc
	/// </summary>
	public abstract class CompositeAttribute : Attribute
	{
		public int id { get; set; }

		public CompositeAttribute()
		{
		}

		public CompositeAttribute(int id)
		{
			this.id = id;
		}
	}

	/// <summary>
	/// Inherit this attribute if you want to have attributes that lets you draw your members in a custom way
	/// These attributes are not composed - they are picked by AttributeDrawers to draw your members
	/// Example of those: BetterVectorAttribute, PopupAttribute, TagsAttribute, etc
	/// </summary>
	public abstract class DrawnAttribute : Attribute
	{
	}

	/// <summary>
	/// Annotate sequences (array/list) with this to signify that you want 
	/// your attributes to be applied on each element instead of the sequence itself
	/// </summary>
	public class PerItemAttribute : Attribute { }

	/// <summary>
	/// Annotate dictionaries with this to signify that you want 
	/// your attributes to be applied on each key instead of the dictionary itself
	/// </summary>
	public class PerKeyAttribute : Attribute { }

	/// <summary>
	/// Annotate dictionaries with this to signify that you want 
	/// your attributes to be applied on each value instead of the dictionary itself
	/// </summary>
	public class PerValueAttribute : Attribute { }

    /// <summary>
    /// Annotate a field with this to let it be drawn by Unity's Layout system
    /// Note: fields marked with this attribute will have the lowest display order
    /// so they will appear last after all other members are drawn
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class DrawByUnityAttribute : Attribute { }
}