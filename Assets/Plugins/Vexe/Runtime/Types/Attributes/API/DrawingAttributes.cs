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
	/// Inherit from this attribute to define per-element drawing for your collections
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class DefinesElementDrawingAttribute : Attribute { }

	/// <summary>
	/// Annotate sequences (array/list) with this to signify that you want 
	/// your attributes to be applied on each element instead of the sequence itself
	/// </summary>
	[DefinesElementDrawing] public class PerItemAttribute   : Attribute { }

	/// <summary>
	/// Annotate dictionaries with this to signify that you want 
	/// your attributes to be applied on each key instead of the dictionary itself
	/// </summary>
	[DefinesElementDrawing] public class PerKeyAttribute    : Attribute { }

	/// <summary>
	/// Annotate dictionaries with this to signify that you want 
	/// your attributes to be applied on each value instead of the dictionary itself
	/// </summary>
	[DefinesElementDrawing] public class PerValueAttribute  : Attribute { }

	/// <summary>
	/// Annotate composite drawers with this to explictly state that this drawer will never be used on the types to ignore
	/// Currently used on ReadonlyAttributeDrawer teling it to ignore Lists, Arrays, Dictionaries and Stacks
	/// why? because Readonly is handled within the the code of those drawers in a custom way
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class IgnoreOnTypes : Attribute
	{
		public readonly Type[] types;

		public IgnoreOnTypes(params Type[] types)
		{
			this.types = types;
		}
	}

	/// <summary>
	/// Customize member display. Available patterns: $name, $type. ex:
	/// public [DisplayMember("$name: $type")] Dictionary<string, int> dict; -- will show in the inspector as:
	/// Dict: Dictionary<string, int>
	/// </summary>
	public class FormatMemberAttribute : Attribute
	{
		public readonly string pattern;

		public FormatMemberAttribute(string pattern)
		{
			this.pattern = pattern;
		}

		public string Format(string name, string type)
		{
			string result = Regex.Replace(pattern, @"\$name", name);
			result = Regex.Replace(result, @"\$type", type);
			return result;
		}
	}

	/// <summary>
	/// Similar to FormatMemberAttribute except this gets applied to the display of dictionary pairs
	/// Available patterns: $keyType, $key, $valueType, $value
	/// </summary>
	public class FormatPairAttribute : Attribute
	{
		public string Pattern { get; set; }

		/// <summary>
		/// A method that returns a string and takes TKey and TValue arguments to format the dictionary pairs
		/// </summary>
		public string Method  { get; set; }

		public FormatPairAttribute(string pattern)
		{
			this.Pattern = pattern;
		}

		public FormatPairAttribute()
		{
		}
	}

	/// <summary>
	/// Annotate members with this attribute to make then visible only when a certain condition is met
	/// The name of the condition method must be passed as an argument to the attribute constructor
	/// The method should return a boolean, and take no parameters
	/// </summary>
	public class VisibleWhenAttribute : Attribute
	{
		public readonly string conditionMethod;

		public VisibleWhenAttribute(string conditionMethod)
		{
			this.conditionMethod = conditionMethod;
		}
	}
}
