using System;

namespace Vexe.Runtime.Types
{
	/// <summary>
	/// Annotate any member with this attribute to expose it even if it wasn't serializable
	/// This is the only way to expose method and properties with side effects (i.e. not auto-properties)
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
	public class ShowAttribute : Attribute
	{
        /// <summary>
        /// The editor category the annotaed member is added to
        /// </summary>
        public string Category;

        public ShowAttribute()
        {
        }

        public ShowAttribute(string category)
        {
            this.Category = category;
        }
	}

	/// <summary>
	/// A shorter alternative to HideInInspector - applicable to fields and properties
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
	public class HideAttribute : Attribute
	{
	}

	/// <summary>
	/// Annotate members with this attribute to make them visible only when certain (a) condition(s) is/are met
    /// The result is evaluated by performing 'Operator' on the return expression of 'ConditionMembers'
    /// A condition member could either be a field/property that returns bool, or a method that returns bool and take no parameters
    /// Possible values for 'Operator' are '&' (AND), '|' (OR) (Defaults to '&')
    /// Member names could also be prefixed with '!' (negation operator) e.g. "!IsDead"
	/// </summary>
	public class VisibleWhenAttribute : Attribute
	{
		public readonly string[] ConditionMembers;
        public readonly char Operator;

		public VisibleWhenAttribute(params string[] conditionMembers):
            this('&', conditionMembers)
        {
        }

		public VisibleWhenAttribute(char operand, params string[] conditionMembers)
		{
			this.ConditionMembers = conditionMembers;
            this.Operator = operand;
		}
	}
}
